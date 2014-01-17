using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public enum ObjectType
    {
        INVALID,
        Point,
        Box,
        Sprite,
        Sound,
        Entity,
        Variable,
    }

    public static class ObjectTypeUtil
    {
        public static ObjectType Parse(XmlElement element)
        {
            ObjectType value;
            string objectType = element.GetString("object_type", "sprite");
            switch (objectType)
            {
                case "point":
                    value = Spriter.ObjectType.Point;
                    break;
                case "box":
                    value = Spriter.ObjectType.Box;
                    break;
                case "sprite":
                    value = Spriter.ObjectType.Sprite;
                    break;
                case "sound":
                    value = Spriter.ObjectType.Sound;
                    break;
                case "entity":
                    value = Spriter.ObjectType.Entity;
                    break;
                case "variable":
                    value = Spriter.ObjectType.Variable;
                    break;
                default:
                    value = Spriter.ObjectType.INVALID;
                    break;
            }

            return value;
        }
    }

    public class SpriterObject : KeyElem
    {
        public static const string XmlKey = "object";

        public Vector2 Pivot { get; private set; }
        public SpatialInfo Spatial { get; private set; }
        public float Alpha { get; private set; }

        public SpriterObject(XmlElement element)
            :base(element)
        { }

        protected override void Parse(XmlElement element)
        {
            base.Parse(element);

            Spatial = new SpatialInfo(element);
            //TODO: Folder
            //TODO: File
            float tmp;
            Vector2 tmpVector2 = Vector2.zero;
            if (element.TryGetFloat("pivot_x", out tmp)) tmpVector2.x = tmp;
            if (element.TryGetFloat("pivot_y", out tmp)) tmpVector2.y = tmp;
            Pivot = tmpVector2;

            if (element.TryGetFloat("alpha", out tmp)) Alpha = tmp;
        }

        public override string ToString()
        {
            return string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "Object (folder: 0, file: 0, x: {0:f}, y: {1:f}, px: {2:f}, py: {3:f}, sx: {4:f}, sy: {5:f}, a: {6:f}, angle: {7:f} [deg: {8:f}])",
                Spatial.Position.x, Spatial.Position.y,
                Pivot.x, Pivot.y,
                Spatial.Scale.x, Spatial.Scale.y,
                Alpha,
                Spatial.Angle, Spatial.Angle_Deg);
        }
    }
}
