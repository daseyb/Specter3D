using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class Ref : KeyElem
    {
        protected SpatialInfo unmapped;
        public TimelineKey Referenced { get; private set; }
        public BoneRef Parent { get; private set; }
        public SpatialInfo Unmapped
        {
            get
            {
                if (unmapped == null)
                {
                    unmapped = ComputeUnmapped();
                }
                return unmapped;
            }
        }

        private SpatialInfo ComputeUnmapped()
        {
            SpatialInfo spatialInfo = null;
            var spatial = Referenced as SpatialTimelineKey;
            if (spatial != null)
            {
                spatialInfo = spatial.Spatial;
            }
            else
            {
                Debug.Log("Non-Spatial Ref type!!");
            }

            if (Parent != null)
			{
				spatialInfo = spatialInfo.Unmap (Parent.Unmapped);
            }

            return spatialInfo;
        }

        public Ref(XmlElement element, Animation animation, MainlineKey parentKey)
            :base(element)
        {
            Parse(element, animation, parentKey);
        }

        private void Parse(XmlElement element, Animation animation, MainlineKey parentKey)
        {
            Referenced = GetTimelineKey(element, animation);

            int parentId = element.GetInt("parent", -1);
            if(parentId >= 0)
            {
                Parent = parentKey.GetBoneRef(parentId);
            }
        }

        protected TimelineKey GetTimelineKey(XmlElement element, Animation animation)
        {
            int timeline = element.GetInt("timeline", 0);
            int key = element.GetInt("key", 0);

            var timelineObj = animation.GetTimeline(timeline);
            if (timelineObj == null)
            {
                Debug.Log(String.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "Unable to find timeline {0} in animation {1}",
                    timeline,
                    animation.Id));
            }
            return timelineObj.GetKey(key);
        }
    }
}
