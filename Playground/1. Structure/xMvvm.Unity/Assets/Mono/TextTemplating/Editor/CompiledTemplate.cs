// 
// CompiledTemplate.cs
//  
// Author:
//       Nathan Baulch <nathan.baulch@gmail.com>
// 
// Copyright (c) 2009 Nathan Baulch
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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Mono.TextTemplating
{
	public sealed class CompiledTemplate : MarshalByRefObject, IDisposable
	{
		private ITextTemplatingEngineHost _host;
		private object _textTransformation;
		private readonly CultureInfo _culture;
		private readonly string[] _assemblyFiles;

		public CompiledTemplate(ITextTemplatingEngineHost host, CompilerResults results, string fullName, CultureInfo culture,
			string[] assemblyFiles)
		{
			AppDomain.CurrentDomain.AssemblyResolve += ResolveReferencedAssemblies;
			_host = host;
			_culture = culture;
			_assemblyFiles = assemblyFiles;
			Load(results, fullName);
		}

		private void Load(CompilerResults results, string fullName)
		{
			var assembly = results.CompiledAssembly;
			var transformType = assembly.GetType(fullName);
			//MS Templating Engine does not look on the type itself, 
			//it checks only that required methods are exists in the compiled type 
			_textTransformation = Activator.CreateInstance(transformType);

			//set the host property if it exists
			Type hostType = null;
			if (_host is TemplateGenerator gen)
			{
				hostType = gen.SpecificHostType;
			}
			var hostProp = transformType.GetProperty("Host", hostType ?? typeof(ITextTemplatingEngineHost));
			if (hostProp != null && hostProp.CanWrite)
				hostProp.SetValue(_textTransformation, _host, null);

			if (_host is ITextTemplatingSessionHost sessionHost)
			{
				//FIXME: should we create a session if it's null?
				var sessionProp = transformType.GetProperty("Session", typeof(IDictionary<string, object>));
				sessionProp.SetValue(_textTransformation, sessionHost.Session, null);
			}
		}

		public string Process()
		{
			var ttType = _textTransformation.GetType();

			var errorProp = ttType.GetProperty("Errors", BindingFlags.Instance | BindingFlags.NonPublic);
			if (errorProp == null)
				throw new ArgumentException("Template must have 'Errors' property");
			var errorMethod = ttType.GetMethod("Error", new[] { typeof(string) });
			if (errorMethod == null)
			{
				throw new ArgumentException("Template must have 'Error(string message)' method");
			}

			var errors = (CompilerErrorCollection)errorProp.GetValue(_textTransformation, null);
			errors.Clear();

			//set the culture
			if (_culture != null)
				ToStringHelper.FormatProvider = _culture;
			else
				ToStringHelper.FormatProvider = CultureInfo.InvariantCulture;

			string output = null;

			var initMethod = ttType.GetMethod("Initialize");
			var transformMethod = ttType.GetMethod("TransformText");

			if (initMethod == null)
			{
				errorMethod.Invoke(_textTransformation, new object[] { "Error running transform: no method Initialize()" });
			}
			else if (transformMethod == null)
			{
				errorMethod.Invoke(_textTransformation, new object[] { "Error running transform: no method TransformText()" });
			}
			else
				try
				{
					initMethod.Invoke(_textTransformation, null);
					output = (string)transformMethod.Invoke(_textTransformation, null);
				}
				catch (Exception ex)
				{
					errorMethod.Invoke(_textTransformation, new object[] { "Error running transform: " + ex });
				}

			_host.LogErrors(errors);

			ToStringHelper.FormatProvider = CultureInfo.InvariantCulture;
			return output;
		}

		private Assembly ResolveReferencedAssemblies(object sender, ResolveEventArgs args)
		{
			var asmName = new AssemblyName(args.Name);
			foreach (var asmFile in _assemblyFiles)
			{
				if (asmName.Name == System.IO.Path.GetFileNameWithoutExtension(asmFile))
					return Assembly.LoadFrom(asmFile);
			}

			var path = _host.ResolveAssemblyReference(asmName.Name + ".dll");
			if (System.IO.File.Exists(path))
				return Assembly.LoadFrom(path);

			return null;
		}

		public void Dispose()
		{
			if (_host != null)
			{
				_host = null;
				AppDomain.CurrentDomain.AssemblyResolve -= ResolveReferencedAssemblies;
			}
		}
	}
}
