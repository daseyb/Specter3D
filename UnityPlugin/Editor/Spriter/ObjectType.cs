using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class ObjectType
    {
        public string Name { get; private set; }

        public static readonly ObjectType INVALID = new ObjectType("INVALID");
        public static readonly ObjectType Point = new ObjectType("Point");
        public static readonly ObjectType Box = new ObjectType("Box");
        public static readonly ObjectType Sprite = new ObjectType("Sprite");
        public static readonly ObjectType Sound = new ObjectType("Sound");
        public static readonly ObjectType Entity = new ObjectType("Entity");
        public static readonly ObjectType Variable = new ObjectType("Variable");

        private ObjectType(string name)
        {
            Name = name;
        }
        public static ObjectType Parse(XmlElement element)
        {
            ObjectType value;
            string objectType = element.GetString("object_type", "sprite");
            switch (objectType)
            {
                case "point":
                    value = ObjectType.Point;
                    break;
                case "box":
                    value = ObjectType.Box;
                    break;
                case "sprite":
                    value = ObjectType.Sprite;
                    break;
                case "sound":
                    value = ObjectType.Sound;
                    break;
                case "entity":
                    value = ObjectType.Entity;
                    break;
                case "variable":
                    value = ObjectType.Variable;
                    break;
                default:
                    value = ObjectType.INVALID;
                    break;
            }

            return value;
        }
    }
}
