/*
Copyright (c) 2014 Andrew Jones
 Based on 'Spriter2Unity' python code by Malhavok

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
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
                //TODO: Settings file to customize prefab location
                var prefabPath = Path.Combine(folderPath, entity.Name + ".prefab");

                GameObject go;
                //Update prefab if it exists, otherwise create a new one
                go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
                if (go == null)
                    go = PrefabUtility.CreateEmptyPrefab(prefabPath) as GameObject;

                //Build the prefab based on the supplied entity
                pb.MakePrefab(entity, go);

                //Change to forward slash for asset database friendliness
                prefabPath = prefabPath.Replace('\\', '/');

                //Add animations to prefab object
                var anim = new AnimationBuilder();
                anim.BuildAnimationClips(go, entity, prefabPath);

                //Add a generic avatar - because why not?
                //TODO: May need to eventually break this into a separate class
                //  ie: if we want to look for a root motion node by naming convention
                var avatar = AvatarBuilder.BuildGenericAvatar(go, "");
                avatar.name = go.name;
                AssetDatabase.AddObjectToAsset(avatar, prefabPath);

                GameObject.DestroyImmediate(go);
            }
        }
    }
}
