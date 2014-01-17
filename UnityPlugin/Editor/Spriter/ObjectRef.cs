using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class ObjectRef : KeyElem
    {
        public const string XmlKey = "object_ref";

        public TimelineKey Referenced { get; private set; }
        public BoneRef Parent { get; private set; }
        public int ZIndex { get; private set; }

        public ObjectRef(XmlElement element, MainlineKey parentKey)
            :base(element)
        {
            Parse(element, parentKey);
        }

        protected virtual void Parse(XmlElement element, MainlineKey parentKey)
        {
            //TODO: Referenced

            int parentId = element.GetInt("parent", -1);
            if (parentId >= 0)
            {
                Parent = parentKey.GetBoneRef(parentId);
            }
            //TODO: ZIndex
        }
    }
}
