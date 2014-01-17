using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
using Assets.ThirdParty.Spriter2Unity.Editor;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class Bone : KeyElem
    {
        public static const string XmlKey = "bone";

        public SpatialInfo Spatial { get; private set; }

        public Color Tint { get; private set; }

        public Bone(XmlElement element)
            : base(element)
        {
        }

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

        public override string ToString()
        {
            return string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "Bone (x: {0:f}, y: {1:f}, sx: {2:f}, sy: {3:f}, angle: {4:f} [deg: {5:f}])",
                Spatial.Position.x, Spatial.Position.y,
                Spatial.Scale.x, Spatial.Scale.y,
                Spatial.Angle, Spatial.Angle_Deg);
        }
    }
}
