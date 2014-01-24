using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Assets.ThirdParty.Spriter2Unity.Editor.Spriter;
using Assets.ThirdParty.Spriter2Unity.Editor.Unity;
using System.IO;

namespace Assets.ThirdParty.Spriter2Unity.Editor
{
    public static class Spriter2Unity
    {
        [MenuItem("My Menu/Test SCML Import")]
        public static void TestImport()
        {
            const string scmlPath = "C:/Users/andre_000/Documents/Spriter/BasicPlatfortmerPack_Essentials/PlatformerPack/player.scml";
            ImportScml("Assets/", scmlPath);
            AssetDatabase.SaveAssets();
        }

        public static void ImportScml(string folderPath, string scmlFilePath)
        {
            var doc = new XmlDocument();
            doc.Load(scmlFilePath);

            //Parse the SCML file
            var scml = new Spriter.ScmlObject(doc);

            //TODO: Verify that all files/folders exist
            var pb = new PrefabBuilder();
            foreach (var entity in scml.Entities)
            {
                var go = pb.MakePrefab(entity);

                var prefabPath = Path.Combine(folderPath, go.name + ".prefab");
                //Change to forward slash for asset database friendliness
                prefabPath = prefabPath.Replace('\\', '/');
                PrefabUtility.CreatePrefab(prefabPath, go);

                var anim = new AnimationBuilder();
                anim.BuildAnimationClips(go, entity, prefabPath);
            }
        }

        private static void CheckFiles(string baseAssetPath, ScmlObject scml)
        {
            foreach(var folder in scml.Folders)
            {
                foreach(var file in folder.Files)
                {
                    //Ensure we have an asset at this relative path
                    string fullAssetPath = baseAssetPath + file.Name;
                    var metaPath = AssetDatabase.GetTextMetaDataPathFromAssetPath(fullAssetPath);
                    if(string.IsNullOrEmpty(metaPath))
                    {
                        Debug.LogWarning("META file not found for asset " + file.Name);
                    }
                }
            }
        }
    }
}
