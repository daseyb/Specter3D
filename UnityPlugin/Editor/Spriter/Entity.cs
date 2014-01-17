using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class Entity : KeyElem
    {
        public static const string XmlKey = "entity";

        public string Name { get; private set; }

        public Entity(XmlElement element)
            : base(element)
        { }

        protected override void Parse(XmlElement element)
        {
            base.Parse(element);

            Name = element.GetString("name", "");

            LoadAnimations(element);
        }

        private void LoadAnimations(XmlElement element)
        {
            var animElements = element.GetElementsByTagName(Animation.XmlKey);
            foreach (XmlElement animElement in animElements)
            {
                animations.Add(new Animation(element));
            }
        }

        private List<Animation> animations = new List<Animation>();
    }
}
