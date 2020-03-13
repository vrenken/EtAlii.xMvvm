// 
// RecyclableAppDomain.cs
//  
// Author:
//       Michael Hutchinson <mhutch@xamarin.com>
// 
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com)
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com_
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

// ReSharper disable all

using System;
using System.Collections.Generic;

namespace Mono.TextTemplating
{
	public class TemplatingAppDomainRecycler
	{
		private const int DefaultTimeoutMs = 2 * 60 * 1000;
		private const int DefaultMaxUses = 20;

		private readonly string _name;
		private readonly object _lockObj = new object();

		private RecyclableAppDomain _domain;

		public TemplatingAppDomainRecycler(string name)
		{
			_name = name;
		}

		public Handle GetHandle()
		{
			lock (_lockObj)
			{
				if (_domain == null || _domain.Domain == null || _domain.UnusedHandles == 0)
				{
					_domain = new RecyclableAppDomain(_name);
				}
				return _domain.GetHandle();
			}
		}

		internal class RecyclableAppDomain
		{
			//TODO: implement timeout based recycling
			//DateTime lastUsed;

			private AppDomain _domain;
			private DomainAssemblyLoader _assemblyMap;

			private int _liveHandles;
			private int _unusedHandles = DefaultMaxUses;

			public RecyclableAppDomain(string name)
			{
				var info = new AppDomainSetup()
				{
					//appbase needs to allow loading this assembly, for remoting
					ApplicationBase = System.IO.Path.GetDirectoryName(typeof(TemplatingAppDomainRecycler).Assembly.Location),
					DisallowBindingRedirects = false,
					DisallowCodeDownload = true,
					DisallowApplicationBaseProbing = false,
					ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
				};
				_domain = AppDomain.CreateDomain(name, null, info);
				var t = typeof(DomainAssemblyLoader);
				AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
				_assemblyMap = (DomainAssemblyLoader)_domain.CreateInstanceFromAndUnwrap(t.Assembly.Location, t.FullName);
				AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
				_domain.AssemblyResolve += _assemblyMap.Resolve; // new DomainAssemblyLoader(assemblyMap).Resolve;
			}

			private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
			{
				var a = typeof(RecyclableAppDomain).Assembly;
				if (args.Name == a.FullName)
					return a;
				return null;
			}

			public int UnusedHandles
			{
				get { return _unusedHandles; }
			}

			public int LiveHandles
			{
				get { return _liveHandles; }
			}

			public AppDomain Domain
			{
				get { return _domain; }
			}

			public void AddAssembly(System.Reflection.Assembly assembly)
			{
				_assemblyMap.Add(assembly.FullName, assembly.Location);
			}

			public Handle GetHandle()
			{
				lock (this)
				{
					if (_unusedHandles <= 0)
					{
						throw new InvalidOperationException("No handles left");
					}
					_unusedHandles--;
					_liveHandles++;
				}
				return new Handle(this);
			}

			public void ReleaseHandle()
			{
				int lh;
				lock (this)
				{
					_liveHandles--;
					lh = _liveHandles;
				}
				//We must unload domain every time after using it for generation
				//Otherwise we could not load new version of the project-generated 
				//assemblies into it. So remove checking for unusedHandles == 0
				if (lh == 0)
				{
					UnloadDomain();
				}
			}

			private void UnloadDomain()
			{
				AppDomain.Unload(_domain);
				_domain = null;
				_assemblyMap = null;
				GC.SuppressFinalize(this);
			}

			~RecyclableAppDomain()
			{
				if (_liveHandles != 0)
					Console.WriteLine("WARNING: recyclable AppDomain's handles were not all disposed");
			}
		}

		public class Handle : IDisposable
		{
			private RecyclableAppDomain _parent;

			internal Handle(RecyclableAppDomain parent)
			{
				_parent = parent;
			}

			public AppDomain Domain => _parent.Domain;

			public void Dispose()
			{
				if (_parent == null)
					return;
				var p = _parent;
				lock (this)
				{
					if (_parent == null)
						return;
					_parent = null;
				}
				p.ReleaseHandle();
			}

			public void AddAssembly(System.Reflection.Assembly assembly)
			{
				_parent.AddAssembly(assembly);
			}
		}

		[Serializable]
		private class DomainAssemblyLoader : MarshalByRefObject
		{
			private readonly Dictionary<string, string> _map = new Dictionary<string, string>();

			public System.Reflection.Assembly Resolve(object sender, ResolveEventArgs args)
			{
				var assemblyFile = ResolveAssembly(args.Name);
				if (assemblyFile != null)
					return System.Reflection.Assembly.LoadFrom(assemblyFile);
				return null;
			}

			public string ResolveAssembly(string name)
			{
				string result;
				if (_map.TryGetValue(name, out result))
					return result;
				return null;
			}

			public void Add(string name, string location)
			{
				_map[name] = location;
			}

			//keep this alive as long as the app domain is alive
			public override object InitializeLifetimeService()
			{
				return null;
			}
		}
	}
}
