﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class SpatialInfo
    {
        public Vector2 Position { get; private set; }
        public Vector2 Scale { get; private set; }
        public float Angle_Deg { get; private set; }
        public float Angle
        {
            get
            {
                return Mathf.Deg2Rad * Angle_Deg;
            }
        }
        public SpinDirection Spin { get; private set; }

        public SpatialInfo(XmlElement element)
        {
            Parse(element);
        }

        protected virtual void Parse(XmlElement element)
        {
            Vector2 position;
            position.x = element.GetFloat("x", 0.0f);
            position.y = element.GetFloat("y", 0.0f);
            Position = position;

            Vector2 scale = Vector2.one;
            scale.x = element.GetFloat("scale_x", 1.0f);
            scale.y = element.GetFloat("scale_y", 1.0f);
            Scale = scale;

            Angle_Deg = element.GetFloat("angle", 0.0f);

            Spin = (SpinDirection)element.GetInt("spin", 1);
        }
    }
}
