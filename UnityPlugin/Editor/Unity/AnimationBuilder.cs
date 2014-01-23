using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using Assets.ThirdParty.Spriter2Unity.Editor.Spriter;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Unity
{
    using Animation = Spriter.Animation;
    public class AnimationBuilder
    {
        public AnimationClip MakeAnimationClip(Animation animation)
        {
            var animClip = new AnimationClip();
            animClip.name = animation.Name;

            //Set clip to Generic type
            UnityEditor.AnimationUtility.SetAnimationType(animClip, ModelImporterAnimationType.Generic);

            //Populate the animation curves & events
            MakeAnimationCurves(animClip, animation);

            //Save the animation
            AssetDatabase.CreateAsset(animClip, "Assets/TestAnim.anim");
            return animClip;
        }

        private enum AnimationCurveIndex
        {
            LocalPositionX,
            LocalPositionY,
            LocalPositionZ,
            LocalRotationX,
            LocalRotationY,
            LocalRotationZ,
            LocalRotationW,
            LocalScaleX,
            LocalScaleY,
            LocalScaleZ,
            IsActive,
            ENUM_COUNT,
        }

        private AnimationClip MakeAnimationCurves(AnimationClip animClip, Animation animation)
        {
            var animEvents = new List<AnimationEvent>();
            var curveDict = new Dictionary<string, AnimationCurve[]>();
            //TODO: Once functional refactor into something more presentable
            var lastRelativePaths = new Dictionary<Timeline, string>(animation.Timelines.Count()); //Used to determine active/inactive toggle
            var lastRefs = new Dictionary<Timeline, TimelineKey>(animation.Timelines.Count()); //Used for key reduction

            Vector3 localPosition;
            Vector3 localScale;
            Vector3 localEulerAngles;

            AnimationCurve[] curves;

            foreach (var timeline in animation.Timelines)
            {
                lastRelativePaths[timeline] = null;
                lastRefs[timeline] = null;
            }

            foreach (var mainKey in animation.MainlineKeys)
            {
                bool firstFrame = mainKey.Time == 0;
                foreach (var r in mainKey.Refs)
                {
                    //If we're still on the same key for this timeline, don't bother adding another keyframe
                    if (lastRefs[r.Referenced.Timeline] == r.Referenced)
                        continue;

                    //Get baked local transforms
                    PrefabUtils.BakeTransforms(r, out localPosition, out localEulerAngles, out localScale);

                    string relativePath = r.RelativePath;
                    string lastRelativePath;
                    lastRelativePath = lastRelativePaths[r.Referenced.Timeline];

                    //Retrieve or create curves array
                    if (!curveDict.TryGetValue(relativePath, out curves))
                    {
                        curves = curveDict[relativePath] = CreateAnimationCurves();
                        if(!firstFrame)
                        {
                            SetCurves(curves, 0, false, ref localPosition, ref localScale, ref localEulerAngles);
                        }
                    }

                    if (relativePath != lastRelativePath)
                    {
                        if (!string.IsNullOrEmpty(lastRelativePath))
                        {
                            AnimationCurve[] lastCurves;
                            if (curveDict.TryGetValue(lastRelativePath, out lastCurves))
                            {
                                SetCurves(lastCurves, mainKey.Time, false, ref localPosition, ref localScale, ref localEulerAngles);
                                //TODO: Get new global coords for position, inverse transform those back into old parent's local coords
                                //lastCurves[(int)AnimationCurveIndex.IsActive].AddKey(GetActiveKeyframe(mainKey.Time, false)); //Disable old object
                            }
                        }
                    }

                    //Set the curves for the current key
                    SetCurves(curves, mainKey.Time, true, ref localPosition, ref localScale, ref localEulerAngles);

                    //Set the sprite for the current key
                    //TODO: Only add an event if the sprite actually changes
                    SetSpriteEvent(animClip, mainKey.Time, r, animEvents);

                    //Set our temp variables for next time
                    lastRelativePaths[r.Referenced.Timeline] = relativePath;
                    lastRefs[r.Referenced.Timeline] = r.Referenced;
                }
            }

            //Build AnimationClip
            foreach (var curveData in curveDict)
            {
                //Position curves
                animClip.SetCurve(curveData.Key, typeof(Transform), "localPosition.x", curveData.Value[(int)AnimationCurveIndex.LocalPositionX]);
                animClip.SetCurve(curveData.Key, typeof(Transform), "localPosition.y", curveData.Value[(int)AnimationCurveIndex.LocalPositionY]);
                animClip.SetCurve(curveData.Key, typeof(Transform), "localPosition.z", curveData.Value[(int)AnimationCurveIndex.LocalPositionZ]);

                //Rotation curves
                animClip.SetCurve(curveData.Key, typeof(Transform), "localRotation.x", curveData.Value[(int)AnimationCurveIndex.LocalRotationX]);
                animClip.SetCurve(curveData.Key, typeof(Transform), "localRotation.y", curveData.Value[(int)AnimationCurveIndex.LocalRotationY]);
                animClip.SetCurve(curveData.Key, typeof(Transform), "localRotation.z", curveData.Value[(int)AnimationCurveIndex.LocalRotationZ]);
                animClip.SetCurve(curveData.Key, typeof(Transform), "localRotation.w", curveData.Value[(int)AnimationCurveIndex.LocalRotationW]);

                //Scale curves
                animClip.SetCurve(curveData.Key, typeof(Transform), "localScale.x", curveData.Value[(int)AnimationCurveIndex.LocalScaleX]);
                animClip.SetCurve(curveData.Key, typeof(Transform), "localScale.y", curveData.Value[(int)AnimationCurveIndex.LocalScaleY]);
                animClip.SetCurve(curveData.Key, typeof(Transform), "localScale.z", curveData.Value[(int)AnimationCurveIndex.LocalScaleZ]);

                //IsActive curve
                animClip.SetCurve(curveData.Key, typeof(GameObject), "m_IsActive", curveData.Value[(int)AnimationCurveIndex.IsActive]);
            }

            //Set animation clip events
            UnityEditor.AnimationUtility.SetAnimationEvents(animClip, animEvents.ToArray());
            return null;
        }

        private void SetSpriteEvent(AnimationClip clip, float time, Ref reference, List<AnimationEvent> events)
        {
            var spriteKey = reference.Referenced as SpriteTimelineKey;
            if (spriteKey != null)
            {
                string packedParam = string.Format("{0};{1};{2}",
                    reference.RelativePath,
                    spriteKey.File.Folder.Id,
                    spriteKey.File.Id);
                
                events.Add(new AnimationEvent() { functionName = "ChangeSprite", stringParameter = packedParam });
            }
        }

        private void SetCurves(AnimationCurve[] curves, float time, bool isActive, ref Vector3 localPosition, ref Vector3 localScale, ref Vector3 localEulerAngles)
        {
            float val;
            //TODO: Consider doing some key reduction
            //IsActive curve
            val = (isActive) ? 1.0f : 0.0f;
            curves[(int)AnimationCurveIndex.IsActive].AddKey(new Keyframe(time, val, float.PositiveInfinity, float.PositiveInfinity) { tangentMode = 0 });

            //Position curves
            curves[(int)AnimationCurveIndex.LocalPositionX].AddKey(new Keyframe(time, localPosition.x) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalPositionY].AddKey(new Keyframe(time, localPosition.y) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalPositionZ].AddKey(new Keyframe(time, localPosition.z, float.PositiveInfinity, float.PositiveInfinity));

            //Rotation curves
            var quat = Quaternion.Euler(localEulerAngles);
            curves[(int)AnimationCurveIndex.LocalRotationX].AddKey(new Keyframe(time, quat.x) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalRotationY].AddKey(new Keyframe(time, quat.y) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalRotationZ].AddKey(new Keyframe(time, quat.z) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalRotationW].AddKey(new Keyframe(time, quat.w) { tangentMode = 0 });

            //Scale curves
            curves[(int)AnimationCurveIndex.LocalScaleX].AddKey(new Keyframe(time, localScale.x) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalScaleY].AddKey(new Keyframe(time, localScale.y) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalScaleZ].AddKey(new Keyframe(time, localScale.z) { tangentMode = 0 });
        }

        private Keyframe GetActiveKeyframe(float time, bool isActive)
        {
            if (isActive) return new Keyframe(time, 1.0f, float.PositiveInfinity, float.PositiveInfinity);
            return new Keyframe(time, 0.0f, float.PositiveInfinity, float.PositiveInfinity);
        }

        private AnimationCurve[] CreateAnimationCurves()
        {
            var curves = new AnimationCurve[(int)AnimationCurveIndex.ENUM_COUNT];
            for (int i = 0; i < (int)AnimationCurveIndex.ENUM_COUNT; i++)
                curves[i] = new AnimationCurve();
            return curves;
        }
    }
}
