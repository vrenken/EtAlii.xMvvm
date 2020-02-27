namespace EtAlii.xMvvm
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    public class Variant1ViewCodeManager : IViewCodeManager
    {
        private const string XamlFileExtension = ".v1xaml";
        private const string GeneratedCsFileExtension = ".v1xaml.g.cs";
        private const string PartialCsFileExtension = ".v1xaml.cs";

        public bool CanManage(string asset) => string.Compare(Path.GetExtension(asset), XamlFileExtension, StringComparison.OrdinalIgnoreCase) == 0;

        private const string GeneratedCsTemplateFileName = "GeneratedCsTemplate.template";
        private const string PartialCsTemplateFileName = "PartialCsTemplate.template";
        private const string TemplateFolder = "xMvvm/Editor/XamlVariant1/Templates";
            
        private readonly FileGenerator _fileGenerator = new FileGenerator();
        //private readonly XamlViewCompiler _xamlViewCompiler = new XamlViewCompiler();
        public void Delete(string asset)
        {
            BuildRelevantFileNames(asset, out _, out var generatedFileName, out _);

            // We always delete the generated file and recreate it.
            if (File.Exists(generatedFileName))
            {
                File.Delete(generatedFileName);
            }
        }

        public void Create(string asset)
        {
            // Let's fetch our templates.
            // This could be optimized by moving it to the constructor and only done once
            // but this makes development frustrating as the editor classes need to be touched in order to test template changes.  
            var generatedCsTemplateFileName = Path.Combine(Application.dataPath, TemplateFolder, GeneratedCsTemplateFileName);
            var generatedCsTemplate = File.ReadAllText(generatedCsTemplateFileName);
            var partialCsTemplateFileName = Path.Combine(Application.dataPath, TemplateFolder, PartialCsTemplateFileName);
            var partialCsTemplate = File.ReadAllText(partialCsTemplateFileName);

            // Also we need a few files and 
            BuildRelevantFileNames(asset, out var xamlFileName, out var generatedFileName, out var partialFileName);
            
            var xamlContent = File.ReadAllText(xamlFileName);
            Debug.Log(xamlContent);
            
            var xamlViewCompiler = new XamlViewCompiler();
            var compilation = xamlViewCompiler.Compile(xamlContent);
            
            var view = new View();
            //compilation.populate(null, view);
            //var view = compilation.create(null);
            
            Debug.Log(view);
            
            var data = new Dictionary<string, object>
            {
                ["now"] = DateTime.Now,
                ["className"] = Path.GetFileNameWithoutExtension(asset), 
                ["classNamespace"] = UnityEditor.EditorSettings.projectGenerationRootNamespace,
                ["viewModelType"] = Path.GetFileNameWithoutExtension(asset) + "Model",
            };
            
            // We always delete the generated file and recreate it.
            if (File.Exists(generatedFileName))
            {
                File.Delete(generatedFileName);
            }
            _fileGenerator.Generate(generatedFileName, generatedCsTemplate, data);
            
            // We only generated the partial file when it doesn't exist yet.
            if (!File.Exists(partialFileName))
            {
                _fileGenerator.Generate(partialFileName, partialCsTemplate, data);
            }
        }
        
        private void BuildRelevantFileNames(
            string asset, 
            out string xamlFileName, 
            out string generatedFileName,
            out string partialFileName)
        {
            xamlFileName = Path.Combine(Application.dataPath, "..", asset);
            generatedFileName = Path.ChangeExtension(xamlFileName, GeneratedCsFileExtension);
            partialFileName = Path.ChangeExtension(xamlFileName, PartialCsFileExtension);
        }
    }
}
