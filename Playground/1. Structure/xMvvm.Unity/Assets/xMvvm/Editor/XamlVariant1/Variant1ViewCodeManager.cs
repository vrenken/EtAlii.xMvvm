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
        
        public Variant1ViewCodeManager()
        {
            var generatedCsTemplateFileName = Path.Combine(Application.dataPath, "xMvvm/Editor/XamlVariant1", "GeneratedCsTemplate.txt");
            _generatedCsTemplate = File.ReadAllText(generatedCsTemplateFileName);

            var partialCsTemplateFileName = Path.Combine(Application.dataPath, "xMvvm/Editor/XamlVariant1", "PartialCsTemplate.txt");
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
            var name = Path.GetFileNameWithoutExtension(asset);

            var rootNamespace = UnityEditor.EditorSettings.projectGenerationRootNamespace;
            
            var xamlContent = File.ReadAllText(filename);
            
            // Debug.Log(content);

            CreateGeneratedFile(name, rootNamespace, generatedFileName);

            CreatePartialFile(name, rootNamespace, partialFileName);
        }

        private void CreatePartialFile(string name, string rootNamespace, string partialFileName)
        {
            // We only generated the partial file when it doesn't exist yet.
            if (File.Exists(partialFileName)) return;
            
            var partialCsContent = _partialCsTemplate
                .Replace("CLASS", name)
                .Replace("ROOT_NAMESPACE", rootNamespace); 
            File.WriteAllText(partialFileName, partialCsContent);
        }
        
        private void CreateGeneratedFile(string name, string rootNamespace, string generatedFileName)
        {
            // We always delete the generated file and recreate it.
            if (File.Exists(generatedFileName))
            {
                File.Delete(generatedFileName);
            }

            var generatedCsContent = _generatedCsTemplate
                .Replace("CLASS", name)
                .Replace("ROOT_NAMESPACE", rootNamespace); 
            File.WriteAllText(generatedFileName, generatedCsContent);
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
