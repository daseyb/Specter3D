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

        public BoneTimelineKey(XmlElement element, Timeline timeline)
            :base(element, timeline)
        {        }

        protected override void Parse(XmlElement element)
        {
            base.Parse(element);

			var boneElem = element ["bone"];
			Spatial = new SpatialInfo(boneElem);

            Color tint = Color.white;
			tint.r = boneElem.GetFloat("r", 1.0f);
			tint.g = boneElem.GetFloat("g", 1.0f);
			tint.b = boneElem.GetFloat("b", 1.0f);
			tint.a = boneElem.GetFloat("a", 1.0f);
            Tint = tint;
        }
    }
}
