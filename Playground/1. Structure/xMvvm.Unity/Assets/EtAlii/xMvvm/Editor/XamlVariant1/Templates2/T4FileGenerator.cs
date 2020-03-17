namespace EtAlii.xMvvm
{
	using System;
	using System.CodeDom.Compiler;
	using System.Collections.Generic;
	using System.IO;
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
	    }

        public void Generate(string outputFileName, string templateFileName, Dictionary<string, object> data)
        {
            try
            {
	            GenerateInternal(outputFileName, templateFileName, data);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

		private void GenerateInternal(string outputFileName, string templateFileName, IDictionary<string, object> data)
		{
			var templateName = Path.GetFileNameWithoutExtension(templateFileName);
			var templateNamespace = UnityEditor.EditorSettings.projectGenerationRootNamespace;

			var generatorOutputFile = Path.GetTempFileName();
			
			if (PreprocessTemplate(templateFileName, templateName, templateNamespace, generatorOutputFile, Encoding.UTF8, out _, out _) == false)
			{
				Debug.LogError($"Failed to PreProcess template '{templateFileName}'.");
				foreach (var error in Errors)
				{
					Debug.LogError(error);
				}
				WindowsHelper.GiveConsoleWindowFocus();
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
				Debug.LogError($"Failed to Process template '{templateFileName}'.");
				foreach (CompilerError error in Errors)
				{
					Debug.LogError(error);
				}
				WindowsHelper.GiveConsoleWindowFocus();
			}

			// if (File.Exists(generatorOutputFile))
			// {
			// 	File.Delete(generatorOutputFile);
			// }
		}

		public ITextTemplatingSession CreateSession()
		{
			return Session;
		}

		public ITextTemplatingSession Session { get; set; }
    }
}