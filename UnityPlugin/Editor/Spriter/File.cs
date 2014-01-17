using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public enum FileType
    {
        INVALID_TYPE,
        Image,
        SoundEffect,
        AtlasImage,
        Entity,
    }

    public class File : KeyElem
    {
        public static const string XmlKey = "file";

        public File(XmlElement element)
            :base(element)
        {
        }

        public FileType FileType { get; private set; }
        public string Name { get; private set; }
        public Vector2 Pivot { get; private set; }
        public Vector2 Size { get; private set; }
        public Vector2 Offset { get; private set; }
        public Vector2 OriginalSize { get; private set; }

        protected override void Parse(XmlElement element)
        {
            base.Parse(element);

            var type = element.GetString("type", "image");
            switch(type)
            {
                case "image":
                    FileType = FileType.Image;
                    break;
                case "atlas_image":
                    FileType = FileType.AtlasImage;
                    break;
                case "sound_effect":
                    FileType = FileType.SoundEffect;
                    break;
                case "entity":
                    FileType = FileType.Entity;
                    break;
                default:
                    FileType = FileType.INVALID_TYPE;
                    break;
            }

            Name = element.GetString("name", "");

            Vector2 pivot;
            pivot.x = element.GetFloat("pivot_x", 0.0f);
            pivot.y = element.GetFloat("pivot_y", 0.0f);
            Pivot = pivot;

            Vector2 size;
            size.x = element.GetInt("width", 0);
            size.y = element.GetInt("height", 0);
            Size = size;

            Vector2 offset;
            offset.x = element.GetInt("offset_x", 0);
            offset.y = element.GetInt("offset_y", 0);
            Offset = offset;

            Vector2 originalSize;
            originalSize.x = element.GetInt("original_width", 0);
            originalSize.y = element.GetInt("original_height", 0);
            OriginalSize = originalSize;
        }
    }
}
