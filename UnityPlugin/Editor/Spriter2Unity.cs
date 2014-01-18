using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Assets.ThirdParty.Spriter2Unity.Editor.Spriter;
using Assets.ThirdParty.Spriter2Unity.Editor.Unity;

namespace Assets.ThirdParty.Spriter2Unity.Editor
{
    public static class Spriter2Unity
    {
        [MenuItem("My Menu/Test SCML Import")]
        public static void TestImport()
        {
            const string scmlPath = "C:/Users/andre_000/Documents/Spriter/BasicPlatfortmerPack_Essentials/PlatformerPack/playerTest.scml";
            ImportScml("", scmlPath);
        }

        public static void ImportScml(string baseAssetPath, string scmlFilePath)
        {
            var doc = new XmlDocument();
            doc.Load(scmlFilePath);

            //Parse the SCML file
            var scml = new ScmlObject(doc);

            //TODO: Verify that all files/folders exist
            var pb = new PrefabBuilder();
            pb.MakePrefab(scml.Entities.FirstOrDefault());
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
