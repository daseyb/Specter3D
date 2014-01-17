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
        public IEnumerable<BoneRef> BoneRefs { get { return boneRefs; } }
        public IEnumerable<ObjectRef> ObjectRefs { get { return objectRefs; } }

        public MainlineKey(XmlElement element)
            : base(element)
        { }
        
        protected override void Parse(XmlElement element)
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
                            boneRefs.Add(new BoneRef(childElement, this));
                            break;
                        case ObjectRef.XmlKey:
                            objectRefs.Add(new ObjectRef(childElement, this));
                            break;
                    }
                }
            }
        }

        public BoneRef GetBoneRef(int id)
        {
            return boneRefs.Where(bone => bone.Id == id).FirstOrDefault();
        }

        public ObjectRef GetObjectRef(int id)
        {
            return objectRefs.Where(obj => obj.Id == id).FirstOrDefault();
        }

        private List<BoneRef> boneRefs = new List<BoneRef>();
        private List<ObjectRef> objectRefs = new List<ObjectRef>();
    }
}
