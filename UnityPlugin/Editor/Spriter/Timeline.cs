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

		public Animation Animation { get; private set; }
        public string Name { get; private set; }
        public ObjectType ObjectType { get; private set; }
        public IEnumerable<TimelineKey> Keys { get{return keys;} }

        public Timeline(XmlElement element, Animation animation)
            :base(element)
        {		
			Parse (element, animation);
		}

        protected virtual void Parse(XmlElement element, Animation animation)
		{
			Animation = animation;
            Name = element.GetString("name", "");

            ObjectType = ObjectType.Parse(element);

            var children = element.GetElementsByTagName(TimelineKey.XmlKey);
            foreach (XmlElement childElement in children)
            {
                keys.Add(GetKey(childElement));
            }
        }

        public TimelineKey GetKey(int id)
        {
            return Keys.Where(key => key.Id == id).FirstOrDefault();
        }

        private TimelineKey GetKey(XmlElement element)
        {
            //Check if key is sprite or bone
            var bone = element[BoneTimelineKey.XmlKey];
            if(bone != null)
            {
                return new BoneTimelineKey(element, this);
            }
            else
            {
                var obj = element[SpriteTimelineKey.XmlKey];
                if(obj != null)
                {
                    var objType = ObjectType.Parse(obj);
                    if (objType == ObjectType.Sprite)
                    {
                        return new SpriteTimelineKey(element, this);
                    }
                }
            }
            return null;
        }
        private List<TimelineKey> keys = new List<TimelineKey>();
    }
}
