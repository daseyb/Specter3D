using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class BoneRef : KeyElem
    {
        public const string XmlKey = "bone_ref";

        public BoneTimelineKey Referenced { get; private set; }
        public BoneRef Parent { get; private set; }

        public BoneRef(XmlElement element, MainlineKey parentKey)
            : base(element)
        {
            Parse(element, parentKey);
        }

        private void Parse(XmlElement element, MainlineKey parentKey)
        {
            //TODO: Referenced

            int parentId = element.GetInt("parent", -1);
            if(parentId >= 0)
            {
                Parent = parentKey.GetBoneRef(parentId);
            }
        }
    }
}
