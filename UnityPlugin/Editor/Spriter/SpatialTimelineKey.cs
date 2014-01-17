using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class SpatialTimelineKey : TimelineKey
    {
        public SpatialInfo Spatial { get; protected set; }

        public SpatialTimelineKey(XmlElement element)
            :base(element)
        { }
    }
}
