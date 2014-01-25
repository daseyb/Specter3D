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
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Unity
{
    public class CharacterMapBuilder
    {
        //1. Parse out all used file/folder ids
        public CharacterMap BuildMap(Spriter.Entity entity, GameObject root)
        {
            var files = new HashSet<Spriter.File>();
            GetUsedFiles(entity, files);

            //Build a character map from the used sprites
            var charMap = root.AddComponent<CharacterMap>();
            foreach(var file in files)
            {
                var fileMap = new FileMap { FilePath = file.Name, Sprite = file.GetSprite() };
                charMap.SetFile(file.Folder.Id, file.Id, fileMap);
            }
            return charMap;
        }

        private void GetUsedFiles(Spriter.Entity entity, HashSet<Spriter.File> files)
        {
            foreach(var animation in entity.Animations)
            {
                GetUsedFiles(animation, files);
            }
        }

        private void GetUsedFiles(Spriter.Animation animation, HashSet<Spriter.File> files)
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
