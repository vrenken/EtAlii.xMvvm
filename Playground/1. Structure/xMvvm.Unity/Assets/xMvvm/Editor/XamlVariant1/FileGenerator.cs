namespace EtAlii.xMvvm
{
    using System;
    using System.IO;
    using DotLiquid;
    using UnityEngine;

    public class FileGenerator 
    {
        public void Generate(string outputFileName, string className, string classNamespace, string template)
        {
            try
            {
                var templateInstance = Template.Parse(template); // Parses and compiles the template
                var outputFileContent = templateInstance.Render(Hash.FromAnonymousObject(new
                {
                    className = className,
                    classNamespace = classNamespace
                })); 
                File.WriteAllText(outputFileName, outputFileContent);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
