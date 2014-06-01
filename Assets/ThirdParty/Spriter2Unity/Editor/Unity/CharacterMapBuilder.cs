/*
Copyright (c) 2014 Andrew Jones, Dario Seyb
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
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Unity
{
    public class CharacterMapBuilder
    {
        //1. Parse out all used file/folder ids
        public CharacterMap BuildMap(Spriter.Entity entity, GameObject root, string spritePath = null)
        {
            var files = new HashSet<Spriter.File>();
            GetUsedFiles(entity, files);

            //Build a character map from the used sprites
            var charMap = root.AddComponent<CharacterMap>();
            foreach(var file in files)
            {
                Sprite sprite = GetSpriteAtPath(file.Name, spritePath);
                var fileMap = new FileMap { FilePath = file.Name, Sprite = sprite };
                charMap.SetFile(file.Folder.Id, file.Id, fileMap);
            }
            return charMap;
        }

        /// <summary>
        /// Finds the correct sprite for the given file path and sprite folder.
        /// </summary>
        /// <param name="filePath">The relative path of the sprite</param>
        /// <param name="spriteFolder">Root folder for sprite path. If null, the entire project is searched.</param>
        private Sprite GetSpriteAtPath(string filePath, string spriteFolder)
        {
            Sprite sprite = null;

            if(string.IsNullOrEmpty(spriteFolder))
            {
                var assetPath = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(filePath)).FirstOrDefault();
                if (!string.IsNullOrEmpty(assetPath))
                {
                    sprite = (Sprite)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite));
                }
            }
            else
            {
                var assetPath = System.IO.Path.Combine(spriteFolder, filePath);
                sprite = (Sprite)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite));
            }
            return sprite;
        }

        private void GetUsedFiles(Spriter.Entity entity, HashSet<Spriter.File> files)
        {
            foreach(var animation in entity.Animations)
            {
                GetUsedFiles(animation, files);
            }
        }

        private void GetUsedFiles(Spriter.SpriterAnimation animation, HashSet<Spriter.File> files)
        {
            foreach(var timeline in animation.Timelines)
            {
                GetUsedFiles(timeline, files);
            }
        }

        private void GetUsedFiles(Spriter.Timeline timeline, HashSet<Spriter.File> files)
        {
            files.UnionWith(timeline.Keys.OfType<Spriter.SpriteTimelineKey>().Select(k => k.File));
        }
    }
}
