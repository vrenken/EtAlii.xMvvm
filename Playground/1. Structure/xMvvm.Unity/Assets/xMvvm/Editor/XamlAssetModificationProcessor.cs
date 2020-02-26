// namespace EtAlii.xMvvm
// {
//     using System.IO;
//     using UnityEditor;
//     using UnityEngine;
//
//     public class XamlAssetModificationProcessor : AssetModificationProcessor
//     {
//         private static readonly ViewCodeManager CodeManager = new ViewCodeManager();
//         
//         static void OnWillCreateAsset(string assetName)
//         {
//              Debug.Log("OnWillCreateAsset is being called with the following asset: " + assetName + ".");
//
//             CodeManager.Create(assetName);
//         }
//
//         static AssetDeleteResult OnWillDeleteAsset(string assetName, RemoveAssetOptions options)
//         {
//             Debug.Log("OnWillDeleteAsset is being called with the following asset: " + assetName + ".");
//
//             CodeManager.Delete(assetName);
//
//             return AssetDeleteResult.DidNotDelete;
//         }
//         
//         private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
//         {
//             Debug.Log("Source path: " + sourcePath + ". Destination path: " + destinationPath + ".");
//
//             CodeManager.Delete(sourcePath);
//             CodeManager.Create(destinationPath);
//
//             return AssetMoveResult.DidNotMove;
//         }
//         
//         static string[] OnWillSaveAssets(string[] paths)
//         {
//             Debug.Log("OnWillSaveAssets");
//             
//             foreach (string path in paths)
//             {
//                 Debug.Log(path);
//                 CodeManager.Update(path);
//
//                 var extension = Path.GetExtension(path);
//                 Debug.Log(extension);
//             }
//             return paths;
//         }
//     }
// }
