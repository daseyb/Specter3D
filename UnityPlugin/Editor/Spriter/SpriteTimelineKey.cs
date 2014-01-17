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

        public SpriteTimelineKey(XmlElement element)
            :base(element["object"])
        { }

        protected override void Parse(XmlElement element)
        {
            base.Parse(element);

            //TODO: Get File

            Spatial = new SpatialInfo(element);

            Vector2 pivot;
            pivot.x = element.GetFloat("pivot_x", 0);
            pivot.y = element.GetFloat("pivot_y", 0);

            Color tint = Color.white;
            tint.r = element.GetFloat("r", 1.0f);
            tint.g = element.GetFloat("g", 1.0f);
            tint.b = element.GetFloat("b", 1.0f);
            tint.a = element.GetFloat("a", 1.0f);
            Tint = tint;
        }
    }
}
