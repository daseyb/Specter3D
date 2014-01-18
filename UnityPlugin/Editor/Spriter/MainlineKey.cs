using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public enum CurveType
    {
        INVALID,
        Instant,
        Linear,
        Quadratic,
        Cubic,
    }

    public enum SpinDirection{
        Clockwise = -1,
        CounterClockwise = 1
    }

    public class MainlineKey : Key
    {
        public IEnumerable<Ref> Refs { get { return refs; } }
        public IEnumerable<BoneRef> BoneRefs { get { return refs.OfType<BoneRef>(); } }
        public IEnumerable<ObjectRef> ObjectRefs { get { return refs.OfType<ObjectRef>(); } }

        public MainlineKey(XmlElement element, Animation animation)
            : base(element)
        {
            Parse(element, animation);
        }
        
        protected virtual void Parse(XmlElement element, Animation animation)
        {
            //Get elements
            //TODO: Ensure proper ordering of elements to prevent dependency errors
            var children = element.ChildNodes;
            foreach(XmlNode child in children)
            {
                XmlElement childElement = child as XmlElement;
                if(childElement != null)
                {
                    switch(childElement.Name)
                    {
                        case BoneRef.XmlKey:
                            refs.Add(new BoneRef(childElement, animation, this));
                            break;
                        case ObjectRef.XmlKey:
                            refs.Add(new ObjectRef(childElement, animation, this));
                            break;
                    }
                }
            }
        }

        public BoneRef GetBoneRef(int id)
        {
            return refs.Where(bone => bone.Id == id).OfType<BoneRef>().FirstOrDefault();
        }

        public ObjectRef GetObjectRef(int id)
        {
            return refs.Where(obj => obj.Id == id).OfType<ObjectRef>().FirstOrDefault();
        }

        public IEnumerable<Ref> GetChildren(Ref parent)
        {
            return refs.Where(obj => obj.Parent == parent);
        }

        private List<Ref> refs = new List<Ref>();
    }
}
