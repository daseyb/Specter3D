using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class TimelineKey : Key
    {
        public Timeline Timeline { get; private set; }
        public CurveType CurveType { get; private set; }

        public TimelineKey(XmlElement element, Timeline timeline)
            : base(element)
        {
            Parse(element, timeline);
        }

        protected virtual void Parse(XmlElement element, Timeline timeline)
        {
            Timeline = timeline;
        }

        protected override void Parse(System.Xml.XmlElement element)
        {
            base.Parse(element);

            string curveString = element.GetString("curve_type", "linear");
            switch (curveString)
            {
                case "instant":
                    CurveType = Spriter.CurveType.Instant;
                    break;
                case "linear":
                    CurveType = Spriter.CurveType.Linear;
                    break;
                case "quadratic":
                    CurveType = Spriter.CurveType.Quadratic;
                    break;
                case "cubic":
                    CurveType = Spriter.CurveType.Cubic;
                    break;
                default:
                    CurveType = Spriter.CurveType.INVALID;
                    break;
            }
        }
    }
}
