namespace EtAlii.xMvvm
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    public class Variant1ViewCodeManagerT4 : IViewCodeManager
    {
        private const string XamlFileExtension = ".v1xaml";
        private const string GeneratedCsFileExtension = ".v1xaml.g.cs";
        private const string PartialCsFileExtension = ".v1xaml.cs";

        public bool CanManage(string asset) => string.Compare(Path.GetExtension(asset), XamlFileExtension, StringComparison.OrdinalIgnoreCase) == 0;

        private const string GeneratedCsTemplateFileName = "GeneratedCsTemplate.t4";
        private const string PartialCsTemplateFileName = "PartialCsTemplate.t4";
        private const string TemplateFolder = "EtAlii/xMvvm/Editor/XamlVariant1/Templates";
            
        private readonly T4FileGenerator _fileGenerator = new T4FileGenerator();
        private readonly XamlViewCompiler _xamlViewCompiler = new XamlViewCompiler();
        
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
            // Also we need a few files and folders. 
            BuildRelevantFileNames(asset, out var xamlFileName, out var generatedFileName, out var partialFileName);

            // Let's fetch the XAML and convert it into a View hierarchy.
            var xamlContent = File.ReadAllText(xamlFileName);
            var compilation = _xamlViewCompiler.Compile(xamlContent);
            if (compilation == default)
            {
                return;
            }
            var view = (View)compilation.create(null);

            // Some values might not be set. Let's add a few default ones.
            view.ViewModelType = view.ViewModelType ?? Path.GetFileNameWithoutExtension(asset) + "Model";
            view.Prefab = view.Prefab ?? Path.GetFileNameWithoutExtension(asset) + ".prefab";

            var data = new Dictionary<string, object>
            {
                ["view"] = view,
                ["now"] = DateTime.Now,
                ["toolVersion"] = "TBD",
                ["className"] = Path.GetFileNameWithoutExtension(asset), 
                ["classNamespace"] = UnityEditor.EditorSettings.projectGenerationRootNamespace,
                ["variantNamespace"] = "EtAlii.xMvvm.XamlVariant1",
            };
            
            // Let's fetch our templates.
            // This could be optimized by moving it to the constructor and only done once
            // but this makes development frustrating as the editor classes need to be touched in order to test template changes.  
            var generatedCsTemplateFileName = Path.Combine(Application.dataPath, TemplateFolder, GeneratedCsTemplateFileName);
            var partialCsTemplateFileName = Path.Combine(Application.dataPath, TemplateFolder, PartialCsTemplateFileName);

            // We always delete the generated file and recreate it.
            if (File.Exists(generatedFileName))
            {
                File.Delete(generatedFileName);
            }
            _fileGenerator.Generate(generatedFileName, generatedCsTemplateFileName, data);
            
            // We only generated the partial file when it doesn't exist yet.
            if (!File.Exists(partialFileName))
            {
                _fileGenerator.Generate(partialFileName, partialCsTemplateFileName, data);
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
