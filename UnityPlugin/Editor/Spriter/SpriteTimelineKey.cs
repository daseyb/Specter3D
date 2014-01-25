using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class SpriteTimelineKey : SpatialTimelineKey
    {
        new public const string XmlKey = "object";

        public File File { get; private set; }
        public Vector2 Pivot { get; private set; }
        public Color Tint { get; private set; }

        public SpriteTimelineKey(XmlElement element, Timeline timeline)
            : base(element, timeline)
        { }

        protected override void Parse(XmlElement element, Timeline timeline)
        {
            base.Parse(element, timeline);

            var objElement = element[XmlKey];

            File = GetFile(objElement);

            Spatial = new SpatialInfo(objElement);

            Vector2 pivot;
            pivot.x = objElement.GetFloat("pivot_x", 0);
            pivot.y = objElement.GetFloat("pivot_y", 0);

            Color tint = Color.white;
            tint.r = objElement.GetFloat("r", 1.0f);
            tint.g = objElement.GetFloat("g", 1.0f);
            tint.b = objElement.GetFloat("b", 1.0f);
            tint.a = objElement.GetFloat("a", 1.0f);
            Tint = tint;
        }

        File GetFile(XmlElement element)
        {
            var folderId = element.GetInt("folder", -1);
            var fileId = element.GetInt("file", -1);

            File file = null;
            var folder = Timeline.Animation.Entity.Scml.GetFolder(folderId);
            if (folder != null)
            {
                file = folder.GetFile(fileId);
                if (file == null)
                {
                    Debug.LogError(string.Format("File Not Found! folder: {0}   file: {1}", folderId, fileId));
                }
            }
            else
            {
                Debug.LogError(string.Format("Folder Not Found!  folder: {0}", folderId));
            }
            return file;
        }
    }
}
