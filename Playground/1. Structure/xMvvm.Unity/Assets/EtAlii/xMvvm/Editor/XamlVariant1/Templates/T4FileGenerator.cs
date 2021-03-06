﻿namespace EtAlii.xMvvm
{
	using System;
	using System.CodeDom.Compiler;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
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
		    
		    var linqAssembly = typeof(Enumerable).Assembly;
		    Refs.Add(Path.GetFileName(linqAssembly.Location));
		    ReferencePaths.Add(Path.GetDirectoryName(linqAssembly.Location));
 	    } 

	    public void Generate(string outputFileName, string templateFileName, Dictionary<string, object> data)
	    {
		    var templateContent = File.ReadAllText(templateFileName);
		    try
		    {
			    GenerateInternal(outputFileName, templateFileName, templateContent, data);
		    }
		    catch (Exception e)
		    {
			    ShowErrors(e.Message, templateContent);
		    }
	    }

	    private void GenerateInternal(string outputFileName, string templateFileName, string templateContent, Dictionary<string, object> data)
        {
			var templateName = Path.GetFileNameWithoutExtension(templateFileName);
			templateName += "_" + Guid.NewGuid().ToString("n");
			var templateNamespace = UnityEditor.EditorSettings.projectGenerationRootNamespace;

			var temporaryOutputFile = Path.GetTempFileName();
			if (PreprocessTemplate(templateFileName, templateName, templateNamespace, temporaryOutputFile, Encoding.UTF8, out _, out _) == false)
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
			
			File.Delete(temporaryOutputFile);
        }

        private void ShowErrors(string header, string templateContent)
        {
	        // And throw all errors.
	        templateContent = DebugHelper.AddLineNumbers(templateContent);
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