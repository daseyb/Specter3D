using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Assets.ThirdParty.Spriter2Unity.Editor.Spriter;

namespace Assets.ThirdParty.Spriter2Unity.Editor
{
    public static class Spriter2Unity
    {
        public static void ImportScml(string baseAssetPath, string scmlFilePath)
        {
            var doc = new XmlDocument();
            doc.Load(scmlFilePath);

            //Parse the SCML file
            var scml = new ScmlObject(doc);

            //Verify that all files/folders exist

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
