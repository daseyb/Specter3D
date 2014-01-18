using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class ObjectRef : Ref
    {
        public const string XmlKey = "object_ref";

        public int ZIndex { get; private set; }

        public ObjectRef(XmlElement element, Animation animation, MainlineKey parentKey)
            : base(element, animation, parentKey)
        { }

        protected override void Parse(XmlElement element)
        {
            base.Parse(element);
            ZIndex = element.GetInt("z_index", Id);
        }
    }
}
