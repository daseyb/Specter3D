using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class BoneTimelineKey : SpatialTimelineKey
    {
        new public const string XmlKey = "bone";

        public Color Tint { get; private set; }

        public BoneTimelineKey(XmlElement element)
            :base(element["bone"])
        {        }

        protected override void Parse(XmlElement element)
        {
            base.Parse(element);

            Spatial = new SpatialInfo(element);

            Color tint = Color.white;
            tint.r = element.GetFloat("r", 1.0f);
            tint.g = element.GetFloat("g", 1.0f);
            tint.b = element.GetFloat("b", 1.0f);
            tint.a = element.GetFloat("a", 1.0f);
            Tint = tint;
        }
    }
}
