namespace EtAlii.xMvvm
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using DotLiquid;
    using DotLiquid.NamingConventions;
    using UnityEngine;

    public class FileGenerator 
    {
        static FileGenerator()
        {
            Template.NamingConvention = new CSharpNamingConvention();
            Template.RegisterFilter(typeof(CodeGenerationFilters));
            Template.RegisterSafeType(typeof(ElementType), elementType => elementType.ToString());
            Template.RegisterSafeType(typeof(BindingMode), bindingMode => bindingMode.ToString());
        }
        public void Generate(string outputFileName, string template, Dictionary<string, object> data)
        {
            try
            {
                var templateInstance = Template.Parse(template); // Parses and compiles the template
                var outputFileContent = templateInstance.Render(Hash.FromDictionary(data)); 
                File.WriteAllText(outputFileName, outputFileContent);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
