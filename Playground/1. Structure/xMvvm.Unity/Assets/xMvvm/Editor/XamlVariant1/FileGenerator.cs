namespace EtAlii.xMvvm
{
    using System;
    using System.CodeDom.Compiler;
    using System.IO;
    using Mono.TextTemplating;
    using UnityEngine;

    public class FileGenerator 
    {
        public void Generate(string outputFileName, string className, string classNamespace, string template)
        {
            try
            {
                var generator = new TemplateGenerator
                {
                    UseRelativeLinePragmas = false,
                };

                // string outputFileName2 = null;
                // generator.ProcessTemplate (null, "<#@ template language=\"C#\" #>", ref outputFileName2, out var outputContent);
                //
                generator.PreprocessTemplate (
                    null, className, classNamespace, 
                    template,
                    out _, out _, out string outputContent);
                
                OutputErrors(generator.Errors);
                if (generator.Errors.HasErrors) return;

                File.WriteAllText(outputFileName, outputContent);

            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void OutputErrors(CompilerErrorCollection errors)
        {
            foreach (var error in errors)
            {
                if (error is CompilerError compilerError)
                {
                    if (compilerError.IsWarning)
                    {
                        Debug.Log(compilerError.ToString());
                    }
                    else
                    {
                        Debug.LogError(compilerError.ToString());
                    }
                }
            }
        }
    }
}
