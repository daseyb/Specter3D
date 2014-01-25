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
