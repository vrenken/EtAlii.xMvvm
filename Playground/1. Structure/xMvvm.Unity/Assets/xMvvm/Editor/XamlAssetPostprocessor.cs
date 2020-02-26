namespace EtAlii.xMvvm
{
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class XamlAssetPostprocessor : AssetPostprocessor
    {
        private static readonly IViewCodeManager[] ViewCodeManagers;
        
        static XamlAssetPostprocessor()
        {
            ViewCodeManagers = new IViewCodeManager[]
            {
                new Variant1ViewCodeManager(),
                new Variant2ViewCodeManager(),
                new Variant3ViewCodeManager()
            };
        }
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var assetToCreate in importedAssets)
            {
                Debug.Log("Reimported XAML Asset: " + assetToCreate);

                // We want to find one, and only one view code manager. Let's fail if we find more.
                var viewCodeManager = ViewCodeManagers.SingleOrDefault(manager => manager.CanManage(assetToCreate));
                viewCodeManager?.Delete(assetToCreate);
                viewCodeManager?.Create(assetToCreate);
            }
            foreach (string assetToDelete in deletedAssets)
            {
                Debug.Log("Deleted XAML Asset: " + assetToDelete);

                // We want to find one, and only one view code manager. Let's fail if we find more.
                var viewCodeManager = ViewCodeManagers.SingleOrDefault(manager => manager.CanManage(assetToDelete));
                viewCodeManager?.Delete(assetToDelete);
            }

            for (var i = 0; i < movedAssets.Length; i++)
            {
                var assetToDelete = movedFromAssetPaths[i];
                var assetToCreate = movedAssets[i]; 

                Debug.Log("Moved XAML Asset: " + assetToCreate + " from: " + assetToDelete);
                
                // We want to find one, and only one view code manager. Let's fail if we find more.
                var viewCodeManager = ViewCodeManagers.SingleOrDefault(manager => manager.CanManage(assetToDelete));
                viewCodeManager?.Delete(assetToDelete);
                
                // We want to find one, and only one view code manager. Let's fail if we find more.
                viewCodeManager = ViewCodeManagers.SingleOrDefault(manager => manager.CanManage(assetToCreate));
                viewCodeManager?.Create(assetToCreate);
            }
        }
    }
}
