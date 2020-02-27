namespace EtAlii.xMvvm
{
    using System;
    using System.IO;
    using UnityEngine;

    public class Variant1ViewCodeManager : IViewCodeManager
    {
        private const string XamlFileExtension = ".v1xaml";
        private const string GeneratedCsFileExtension = ".v1xaml.g.cs";
        private const string PartialCsFileExtension = ".v1xaml.cs";

        public bool CanManage(string asset) => string.Compare(Path.GetExtension(asset), XamlFileExtension, StringComparison.OrdinalIgnoreCase) == 0;

        private readonly string _generatedCsTemplate;
        private readonly string _partialCsTemplate;
        
        private readonly FileGenerator _fileGenerator = new FileGenerator();
        
        public Variant1ViewCodeManager()
        {
            var generatedCsTemplateFileName = Path.Combine(Application.dataPath, "xMvvm/Editor/XamlVariant1", "GeneratedCsTemplate.template");
            _generatedCsTemplate = File.ReadAllText(generatedCsTemplateFileName);

            var partialCsTemplateFileName = Path.Combine(Application.dataPath, "xMvvm/Editor/XamlVariant1", "PartialCsTemplate.template");
            _partialCsTemplate = File.ReadAllText(partialCsTemplateFileName);
        }
        
        public void Delete(string asset)
        {
            BuildRelevantFileNames(asset, out _, out var fullGeneratedFileName, out _);

            // We always delete the generated file and recreate it.
            if (File.Exists(fullGeneratedFileName))
            {
                File.Delete(fullGeneratedFileName);
            }
        }

        public void Create(string asset)
        {
            BuildRelevantFileNames(asset, out var filename, out var generatedFileName, out var partialFileName);
            var className = Path.GetFileNameWithoutExtension(asset);

            var classNamespace = UnityEditor.EditorSettings.projectGenerationRootNamespace;
            
            var xamlContent = File.ReadAllText(filename);
            
            // Debug.Log(content);

            // We always delete the generated file and recreate it.
            if (File.Exists(generatedFileName))
            {
                File.Delete(generatedFileName);
            }
            _fileGenerator.Generate(generatedFileName, className, classNamespace, _generatedCsTemplate);

            // We only generated the partial file when it doesn't exist yet.
            if (!File.Exists(partialFileName))
            {
                _fileGenerator.Generate(partialFileName, className, classNamespace, _partialCsTemplate);
            }
        }
        
        private void BuildRelevantFileNames(
            string asset, 
            out string fileName, 
            out string generatedFileName,
            out string partialFileName)
        {
            fileName = Path.Combine(Application.dataPath, "..", asset);
            generatedFileName = Path.ChangeExtension(fileName, GeneratedCsFileExtension);
            partialFileName = Path.ChangeExtension(fileName, PartialCsFileExtension);
        }
    }
}
