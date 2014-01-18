using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.ThirdParty.Spriter2Unity.Editor.Spriter;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Unity
{
    //Name clash between Spriter and Unity - use the Spriter version by default
    using Animation = Assets.ThirdParty.Spriter2Unity.Editor.Spriter.Animation;

    public class PrefabBuilder
    {
        public float PixelScale = 0.01f;
        public float ZSpacing = 0.1f;

        public GameObject MakePrefab(Entity entity)
        {
            GameObject root = new GameObject(entity.Name);
            foreach (var animation in entity.Animations)
            {
                MakePrefab(animation, root);
            }
            return root;
        }

        private void MakePrefab(Animation animation, GameObject root)
        {
            foreach(var mainKey in animation.MainlineKeys)
            {
                MakePrefab(mainKey, root);
            }
        }

        private struct RefParentInfo
        {
            public Ref Ref;
            public GameObject Root;
        }
        private void MakePrefab(MainlineKey mainKey, GameObject root)
        {
            Stack<RefParentInfo> toProcess = new Stack<RefParentInfo>(mainKey.GetChildren(null).Select(child => new RefParentInfo{Ref = child, Root = root}));
            while(toProcess.Count > 0)
            {
                var next = toProcess.Pop();
                var go = MakePrefab(next.Ref, next.Root);
                foreach(var child in mainKey.GetChildren(next.Ref))
                {
                    toProcess.Push(new RefParentInfo{Ref = child, Root = go});
                }
            }
        }
        
        private GameObject MakePrefab(Ref childRef, GameObject root)
        {
            var timeline = childRef.Referenced.Timeline;
            var transform = root.transform.Find(timeline.Name);
            GameObject go;
            if(transform == null)
            {
                go = MakeGameObject(childRef, root);
            }
            else
            {
                go = transform.gameObject;
            }
            return go;
        }

        private GameObject MakeGameObject(Ref childRef, GameObject parent)
        {
            TimelineKey key = childRef.Referenced;
            GameObject go = new GameObject(key.Timeline.Name);
            if (parent != null)
            {
                go.transform.parent = parent.transform;
            }

            //Any objects that show up only after t=0 begin inactive
            if(key.Time_Ms > 0)
            {
                go.SetActive(false);
            }

            var unmapped = childRef.Unmapped;
            var spatial = childRef.Referenced as SpatialTimelineKey;
            if(spatial != null)
            {
                Vector3 localPosition = unmapped.Position;

                Vector3 localScale = Vector3.one;

                var spriteKey = key as SpriteTimelineKey;
                if (spriteKey != null)
                {
                    //Set sprite information
                    var sprite = go.AddComponent<SpriteRenderer>();
                    sprite.sprite = spriteKey.File.GetSprite();

                    var sinA = Mathf.Sin(unmapped.Angle);
                    var cosA = Mathf.Cos(unmapped.Angle);

                    var pvt = spriteKey.File.GetPivotOffetFromMiddle();

                    pvt.x *= unmapped.Scale.x;
                    pvt.y *= unmapped.Scale.y;

                    var rotPX = pvt.x * cosA - pvt.y * sinA;
                    var rotPY = pvt.x * sinA + pvt.y * cosA;

                    localPosition.x += rotPX;
                    localPosition.y += rotPY;

                    localScale.x = unmapped.Scale.x;
                    localScale.y = unmapped.Scale.y;
                    localPosition.z = ((ObjectRef)childRef).ZIndex * -ZSpacing;                    
                }
                localPosition *= PixelScale;
                go.transform.localPosition = localPosition;
                go.transform.localEulerAngles = new Vector3(0, 0, unmapped.Angle_Deg);
                go.transform.localScale = localScale;
            }

            return go;
        }

        public AnimationClip MakeAnimationClip(Animation animation)
        {
            var animClip = new AnimationClip();
            animClip.name = animation.Name;
            return null;
        }
    }
}
