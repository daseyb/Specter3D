using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class BoneRef : Ref
    {
        public const string XmlKey = "bone_ref";

        public BoneRef(XmlElement element, Animation animation, MainlineKey parentKey)
            : base(element, animation, parentKey)
        { }
    }
}
