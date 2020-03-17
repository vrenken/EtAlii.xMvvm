namespace EtAlii.xMvvm
{
	using System;
	using System.CodeDom.Compiler;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Mono.TextTemplating;
	using UnityEngine;

    public class T4FileGenerator : TemplateGenerator, ITextTemplatingSessionHost
    {
	    public T4FileGenerator()
	    {
		    var unityEngineAssembly = typeof(Debug).Assembly;
		    Refs.Add(Path.GetFileName(unityEngineAssembly.Location));
		    ReferencePaths.Add(Path.GetDirectoryName(unityEngineAssembly.Location));
			
		    var monoTextTemplatingAssembly = typeof(TemplateGenerator).Assembly;
		    Refs.Add(Path.GetFileName(monoTextTemplatingAssembly.Location));
		    ReferencePaths.Add(Path.GetDirectoryName(monoTextTemplatingAssembly.Location));

		    var xMvvmAssembly = typeof(View).Assembly;
		    Refs.Add(Path.GetFileName(xMvvmAssembly.Location));
		    ReferencePaths.Add(Path.GetDirectoryName(xMvvmAssembly.Location));
	    }

        public void Generate(string outputFileName, string templateFileName, Dictionary<string, object> data)
        {
			var templateName = Path.GetFileNameWithoutExtension(templateFileName);
			var templateNamespace = UnityEditor.EditorSettings.projectGenerationRootNamespace;

			var templateContent = File.ReadAllText(templateFileName);
			templateContent = DebugHelper.AddLineNumbers(templateContent);

			if (PreprocessTemplate(templateFileName, templateName, templateNamespace, templateContent, out _, out _, out _) == false)
			{
				ShowErrors($"Failed to preprocess template '{templateFileName}'", templateContent);
				return;
			}

			var session  = new TextTemplatingSession();
			foreach (var kvp in data)
			{
				session.Add(kvp.Key, kvp.Value);
			}
			var sessionHost = (ITextTemplatingSessionHost) this;
			sessionHost.Session = session;			
			
			if (ProcessTemplate(templateFileName, ref outputFileName) == false)
			{
				ShowErrors($"Failed to process template '{templateFileName}'", templateContent);
			}
        }

        private void ShowErrors(string header, string templateContent)
        {
	        // And throw all errors.
	        var errors = (from CompilerError error in Errors select $"[{error.Line}, {error.Column}] {error.ErrorText}").ToList();
	        Debug.LogError(header + ": " + Environment.NewLine + string.Join(Environment.NewLine, errors) + Environment.NewLine + Environment.NewLine + templateContent);
	        WindowsHelper.GiveConsoleWindowFocus();
        }

		public ITextTemplatingSession CreateSession()
		{
			return Session;
		}

		public ITextTemplatingSession Session { get; set; }
    }
}