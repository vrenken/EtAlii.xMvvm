namespace EtAlii.xMvvm
{
    using System;
    using System.IO;
    using UnityEngine;

    public class Variant1ViewCodeManager : IViewCodeManager
    {
        private const string XamlFileExtension = ".v1xaml";

        public bool CanManage(string asset) => string.Compare(Path.GetExtension(asset), XamlFileExtension, StringComparison.OrdinalIgnoreCase) == 0;

        public void Delete(string asset)
        {
        }

        public void Create(string asset)
        {
            var fullFilename = Path.Combine(Application.dataPath, "..", asset);
                
            var content = File.ReadAllText(fullFilename);
            
            // Debug.Log(content);
        }
    }
}
