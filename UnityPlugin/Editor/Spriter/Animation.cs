using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class LoopType
    {
        public string Name { get; private readonly set; }

        public static readonly LoopType INVALID = new LoopType() { Name = "INVALID" };
        public static readonly LoopType True = new LoopType() { Name = "true" };
        public static readonly LoopType False = new LoopType() { Name = "false" };
        public static readonly LoopType PingPong = new LoopType() { Name = "ping_pong" };

        public static LoopType Parse(XmlElement element)
        {
            var looping = element.GetString("looping", "true");
            switch (looping)
            {
                case "true":
                    return LoopType.True;
                case "false":
                    return LoopType.False;
                case "ping_pong":
                    return LoopType.PingPong;
            }
            return LoopType.INVALID;
        }
    }

    public class Animation : KeyElem
    {
        public static const string XmlKey = "animation";

        public string Name { get; private set; }
        public int Length_Ms { get; private set; }
        public float Length { get { return ((float)Length_Ms) / 1000; } }
        public LoopType LoopType { get; private set; }
        public int LoopTo { get; private set; }
        public IEnumerable<MainlineKey> MainlineKeys { get { return mainlineKeys; } }
        public IEnumerable<Timeline> Timelines { get { return timelines; } }

        public Animation(XmlElement element)
            : base(element)
        { }

        protected override void Parse(XmlElement element)
        {
            base.Parse(element);

            Name = element.GetString("name", "");
            Length_Ms = element.GetInt("length", -1);
            LoopType = LoopType.Parse(element);
            LoopTo = element.GetInt("loop_to", 0);

            LoadMainline(element);
        }

        private void LoadMainline(XmlElement element)
        {
            var mainlineElem = element["mainline"];
            var mainlineKeyElems = mainlineElem.GetElementsByTagName(Key.XmlKey);
            foreach (XmlElement mainlineKeyElem in mainlineKeyElems)
            {
                mainlineKeys.Add(new MainlineKey(mainlineKeyElem));
            }
        }

        private void LoadTimelines(XmlElement element)
        {
            var timelineElems = element.GetElementsByTagName(Timeline.XmlKey);
            foreach(XmlElement timelineElem in timelineElems)
            {
                timelines.Add(new Timeline(element));
            }
        }

        private List<MainlineKey> mainlineKeys = new List<MainlineKey>();
        private List<Timeline> timelines = new List<Timeline>();
    }
}
