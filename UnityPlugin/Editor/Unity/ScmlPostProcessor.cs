using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Unity
{
    public class ScmlPostProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets (
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach(var path in importedAssets)
            {
                if (!path.EndsWith(".scml"))
                    continue;

                ImportScml(path);
            }
        }

        static void ImportScml(string assetPath)
        {
            string folderPath = Path.GetDirectoryName(assetPath);

            var doc = new XmlDocument();
            doc.Load(assetPath);

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

                //Create prefab and destroy the object in scene
                PrefabUtility.CreatePrefab(prefabPath, go);

                //Add animations to prefab object
                var anim = new AnimationBuilder();
                anim.BuildAnimationClips(go, entity, prefabPath);

                GameObject.DestroyImmediate(go);
            }
        }
    }
}
