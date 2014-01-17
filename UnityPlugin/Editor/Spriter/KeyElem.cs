using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class KeyElem
    {
        public int Id { get; private set; }

        public KeyElem(XmlElement element)
        {
            Parse(element);
        }

        protected virtual void Parse(XmlElement element)
        {
            Id = element.GetInt("id", -1);
        }
    }
}
