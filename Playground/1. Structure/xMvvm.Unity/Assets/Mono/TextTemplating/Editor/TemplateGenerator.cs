// 
// TemplatingHost.cs
//  
// Author:
//       Michael Hutchinson <mhutchinson@novell.com>
// 
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com)
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
using System.IO;
using System.Reflection;
using System.Text;

namespace Mono.TextTemplating
{
	public class TemplateGenerator : MarshalByRefObject, ITextTemplatingEngineHost
	{
		//re-usable
		private TemplatingEngine _engine;

		//per-run variables
		private string _inputFile;
		private Encoding _encoding;

		public string OutputFile => _outputFile;
		private string _outputFile;

		//host fields
		public CompilerErrorCollection Errors => _errors;
		private readonly CompilerErrorCollection _errors = new CompilerErrorCollection();

		public List<string> Refs => _refs;
		private readonly List<string> _refs = new List<string>();

		public List<string> Imports => _imports;
		private readonly List<string> _imports = new List<string>();

		public List<string> IncludePaths => _includePaths;
		private readonly List<string> _includePaths = new List<string>();

		public List<string> ReferencePaths => _referencePaths;
		private readonly List<string> _referencePaths = new List<string>();

		private readonly Dictionary<ParameterKey, string> _parameters = new Dictionary<ParameterKey, string>();
		private readonly Dictionary<string, KeyValuePair<string, string>> _directiveProcessors = new Dictionary<string, KeyValuePair<string, string>>();

		public bool UseRelativeLinePragmas { get; set; }

		public TemplateGenerator()
		{
			Refs.Add(typeof(TextTransformation).Assembly.Location);
			Refs.Add(typeof(Uri).Assembly.Location);
			Imports.Add("System");
		}

		public CompiledTemplate CompileTemplate(string content)
		{
			if (String.IsNullOrEmpty(content))
				throw new ArgumentNullException("content");

			_errors.Clear();
			_encoding = Encoding.UTF8;

			return Engine.CompileTemplate(content, this);
		}

		protected TemplatingEngine Engine
		{
			get
			{
				if (_engine == null)
					_engine = new TemplatingEngine();
				return _engine;
			}
		}

		public bool ProcessTemplate(string inputFile, ref string outputFile)
		{
			if (String.IsNullOrEmpty(inputFile))
				throw new ArgumentNullException("inputFile");
			if (String.IsNullOrEmpty(outputFile))
				throw new ArgumentNullException("outputFile");

			string content;
			try
			{
				content = File.ReadAllText(inputFile);
			}
			catch (IOException ex)
			{
				_errors.Clear();
				AddError("Could not read input file '" + inputFile + "':\n" + ex);
				return false;
			}

			ProcessTemplate(inputFile, content, ref outputFile, out var output);

			try
			{
				if (!_errors.HasErrors)
					File.WriteAllText(outputFile, output, _encoding);
			}
			catch (IOException ex)
			{
				AddError("Could not write output file '" + outputFile + "':\n" + ex);
			}

			return !_errors.HasErrors;
		}

		public bool ProcessTemplate(string inputFileName, string inputContent, ref string outputFileName, out string outputContent)
		{
			_errors.Clear();
			_encoding = Encoding.UTF8;

			_outputFile = outputFileName;
			_inputFile = inputFileName;
			outputContent = Engine.ProcessTemplate(inputContent, this);
			outputFileName = _outputFile;

			return !_errors.HasErrors;
		}

		public bool PreprocessTemplate(string inputFile, string className, string classNamespace,
			string outputFile, Encoding encoding, out string language, out string[] references)
		{
			language = null;
			references = null;

			if (string.IsNullOrEmpty(inputFile))
				throw new ArgumentNullException("inputFile");
			if (string.IsNullOrEmpty(outputFile))
				throw new ArgumentNullException("outputFile");

			string content;
			try
			{
				content = File.ReadAllText(inputFile);
			}
			catch (IOException ex)
			{
				_errors.Clear();
				AddError("Could not read input file '" + inputFile + "':\n" + ex);
				return false;
			}

			PreprocessTemplate(inputFile, className, classNamespace, content, out language, out references, out var output);

			try
			{
				if (!_errors.HasErrors)
					File.WriteAllText(outputFile, output, encoding);
			}
			catch (IOException ex)
			{
				AddError("Could not write output file '" + outputFile + "':\n" + ex);
			}

			return !_errors.HasErrors;
		}

		public bool PreprocessTemplate(string inputFileName, string className, string classNamespace, string inputContent,
			out string language, out string[] references, out string outputContent)
		{
			_errors.Clear();
			_encoding = Encoding.UTF8;

			_inputFile = inputFileName;
			outputContent = Engine.PreprocessTemplate(inputContent, this, className, classNamespace, out language, out references);

			return !_errors.HasErrors;
		}

		private CompilerError AddError(string error)
		{
			var err = new CompilerError();
			err.ErrorText = error;
			Errors.Add(err);
			return err;
		}

		#region Virtual members

		public virtual object GetHostOption(string optionName)
		{
			switch (optionName)
			{
				case "UseRelativeLinePragmas":
					return UseRelativeLinePragmas;
			}
			return null;
		}

		public virtual AppDomain ProvideTemplatingAppDomain(string content)
		{
			return null;
		}

		protected virtual string ResolveAssemblyReference(string assemblyReference)
		{
			if (Path.IsPathRooted(assemblyReference))
				return assemblyReference;
			foreach (var referencePath in ReferencePaths)
			{
				var path = Path.Combine(referencePath, assemblyReference);
				if (File.Exists(path))
					return path;

				path = Path.Combine(referencePath, assemblyReference) + ".dll";
				if (File.Exists(path))
					return path;

				path = Path.Combine(referencePath, assemblyReference) + ".exe";
				if (File.Exists(path))
					return path;
			}

			var assemblyName = new AssemblyName(assemblyReference);
			if (assemblyName.Version != null)
				return assemblyReference;

			if (!assemblyReference.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) && !assemblyReference.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
				return assemblyReference + ".dll";
			return assemblyReference;
		}

		protected virtual string ResolveParameterValue(string directiveId, string processorName, string parameterName)
		{
			var key = new ParameterKey(processorName, directiveId, parameterName);
			string value;
			if (_parameters.TryGetValue(key, out value))
				return value;
			if (processorName != null || directiveId != null)
				return ResolveParameterValue(null, null, parameterName);
			return null;
		}

		protected virtual Type ResolveDirectiveProcessor(string processorName)
		{
			KeyValuePair<string, string> value;
			if (!_directiveProcessors.TryGetValue(processorName, out value))
				throw new Exception(string.Format("No directive processor registered as '{0}'", processorName));
			var asmPath = ResolveAssemblyReference(value.Value);
			if (asmPath == null)
				throw new Exception(string.Format("Could not resolve assembly '{0}' for directive processor '{1}'", value.Value, processorName));
			var asm = Assembly.LoadFrom(asmPath);
			return asm.GetType(value.Key, true);
		}

		protected virtual string ResolvePath(string path)
		{
			path = Environment.ExpandEnvironmentVariables(path);
			if (Path.IsPathRooted(path))
				return path;
			var dir = Path.GetDirectoryName(_inputFile);
			var test = Path.Combine(dir, path);
			if (File.Exists(test) || Directory.Exists(test))
				return test;
			return path;
		}

		#endregion

		public void AddDirectiveProcessor(string name, string klass, string assembly)
		{
			_directiveProcessors.Add(name, new KeyValuePair<string, string>(klass, assembly));
		}

		public void AddParameter(string processorName, string directiveName, string parameterName, string value)
		{
			_parameters.Add(new ParameterKey(processorName, directiveName, parameterName), value);
		}

		protected virtual bool LoadIncludeText(string requestFileName, out string content, out string location)
		{
			content = "";
			location = ResolvePath(requestFileName);

			if (location == null || !File.Exists(location))
			{
				foreach (var path in _includePaths)
				{
					var f = Path.Combine(path, requestFileName);
					if (File.Exists(f))
					{
						location = f;
						break;
					}
				}
			}

			if (location == null)
				return false;

			try
			{
				content = File.ReadAllText(location);
				return true;
			}
			catch (IOException ex)
			{
				AddError("Could not read included file '" + location + "':\n" + ex);
			}
			return false;
		}

		#region Explicit ITextTemplatingEngineHost implementation

		bool ITextTemplatingEngineHost.LoadIncludeText(string requestFileName, out string content, out string location)
		{
			return LoadIncludeText(requestFileName, out content, out location);
		}

		void ITextTemplatingEngineHost.LogErrors(CompilerErrorCollection errors)
		{
			_errors.AddRange(errors);
		}

		string ITextTemplatingEngineHost.ResolveAssemblyReference(string assemblyReference)
		{
			return ResolveAssemblyReference(assemblyReference);
		}

		string ITextTemplatingEngineHost.ResolveParameterValue(string directiveId, string processorName, string parameterName)
		{
			return ResolveParameterValue(directiveId, processorName, parameterName);
		}

		Type ITextTemplatingEngineHost.ResolveDirectiveProcessor(string processorName)
		{
			return ResolveDirectiveProcessor(processorName);
		}

		string ITextTemplatingEngineHost.ResolvePath(string path)
		{
			return ResolvePath(path);
		}

		void ITextTemplatingEngineHost.SetFileExtension(string extension)
		{
			extension = extension.TrimStart('.');
			if (Path.HasExtension(_outputFile))
			{
				_outputFile = Path.ChangeExtension(_outputFile, extension);
			}
			else
			{
				_outputFile = _outputFile + "." + extension;
			}
		}

		void ITextTemplatingEngineHost.SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
		{
			_encoding = encoding;
		}

		IList<string> ITextTemplatingEngineHost.StandardAssemblyReferences
		{
			get { return _refs; }
		}

		IList<string> ITextTemplatingEngineHost.StandardImports
		{
			get { return _imports; }
		}

		string ITextTemplatingEngineHost.TemplateFile
		{
			get { return _inputFile; }
		}

		#endregion

		private struct ParameterKey : IEquatable<ParameterKey>
		{
			private readonly string _processorName;
			private readonly string _directiveName;
			private readonly string _parameterName;
			private readonly int _hashCode;

			public ParameterKey(string processorName, string directiveName, string parameterName)
			{
				_processorName = processorName ?? "";
				_directiveName = directiveName ?? "";
				_parameterName = parameterName ?? "";
				unchecked
				{
					_hashCode = _processorName.GetHashCode()
							   ^ _directiveName.GetHashCode()
							   ^ _parameterName.GetHashCode();
				}
			}

			public override bool Equals(object obj)
			{
				return obj is ParameterKey && Equals((ParameterKey)obj);
			}

			public bool Equals(ParameterKey other)
			{
				return _processorName == other._processorName && _directiveName == other._directiveName && _parameterName == other._parameterName;
			}

			public override int GetHashCode()
			{
				return _hashCode;
			}
		}

		/// <summary>
		///     If non-null, the template's Host property will be the full type of this host.
		/// </summary>
		public virtual Type SpecificHostType
		{
			get { return null; }
		}

		/// <summary>
		///     Gets any additional directive processors to be included in the processing run.
		/// </summary>
		public virtual IEnumerable<IDirectiveProcessor> GetAdditionalDirectiveProcessors()
		{
			yield break;
		}
	}
}
