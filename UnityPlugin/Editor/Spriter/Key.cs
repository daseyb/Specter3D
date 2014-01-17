using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class Key : KeyElem
    {
        public static const string XmlKey = "key";

        public int Time_Ms { get; private set; }
        public float Time { get { return ((float)Time_Ms) / 1000; } }

        public Key(XmlElement element)
            : base(element)
        { }

        protected override void Parse(XmlElement element)
        {
            base.Parse(element);

            Time_Ms = element.GetInt("time", 0);
        }
    }
}
