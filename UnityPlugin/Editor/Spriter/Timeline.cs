using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class Timeline : KeyElem
    {
        public const string XmlKey = "timeline";

        public string Name { get; private set; }
        public ObjectType ObjectType { get; private set; }
        public IEnumerable<TimelineKey> Keys { get{return keys;} }

        public Timeline(XmlElement element)
            :base(element)
        { }

        protected override void Parse(XmlElement element)
        {
            base.Parse(element);

            Name = element.GetString("name", "");

            ObjectType = ObjectType.Parse(element);

            var children = element.GetElementsByTagName(TimelineKey.XmlKey);
            foreach (XmlElement childElement in children)
            {
                keys.Add(GetKey(childElement));
            }
        }

        private TimelineKey GetKey(XmlElement element)
        {
            //Check if key is sprite or bone
            var bone = element[BoneTimelineKey.XmlKey];
            if(bone != null)
            {
                return new BoneTimelineKey(element);
            }
            else
            {
                var obj = element[SpriteTimelineKey.XmlKey];
                if(obj != null)
                {
                    var objType = ObjectType.Parse(obj);
                    if (objType == ObjectType.Sprite)
                    {
                        return new SpriteTimelineKey(element);
                    }
                }
            }
            return null;
        }
        private List<TimelineKey> keys = new List<TimelineKey>();
    }
}
