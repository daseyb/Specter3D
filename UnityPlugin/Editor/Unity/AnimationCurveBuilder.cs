using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Unity
{
    public class AnimationCurveBuilder
    {
        Dictionary<string, AnimationCurve[]> curveCache = new Dictionary<string, AnimationCurve[]>();

        public void AddCurves(AnimationClip animClip)
        {
            Debug.Log(string.Format("curveCache length:{0}", curveCache.Count));
            foreach(var kvp in curveCache)
            {
                //Position curves
                animClip.SetCurve(kvp.Key, typeof(Transform), "localPosition.x", kvp.Value[(int)AnimationCurveIndex.LocalPositionX]);
                animClip.SetCurve(kvp.Key, typeof(Transform), "localPosition.y", kvp.Value[(int)AnimationCurveIndex.LocalPositionY]);
                animClip.SetCurve(kvp.Key, typeof(Transform), "localPosition.z", kvp.Value[(int)AnimationCurveIndex.LocalPositionZ]);

                //Rotation curves
                animClip.SetCurve(kvp.Key, typeof(Transform), "localRotation.x", kvp.Value[(int)AnimationCurveIndex.LocalRotationX]);
                animClip.SetCurve(kvp.Key, typeof(Transform), "localRotation.y", kvp.Value[(int)AnimationCurveIndex.LocalRotationY]);
                animClip.SetCurve(kvp.Key, typeof(Transform), "localRotation.z", kvp.Value[(int)AnimationCurveIndex.LocalRotationZ]);
                animClip.SetCurve(kvp.Key, typeof(Transform), "localRotation.w", kvp.Value[(int)AnimationCurveIndex.LocalRotationW]);

                //Scale curves
                animClip.SetCurve(kvp.Key, typeof(Transform), "localScale.x", kvp.Value[(int)AnimationCurveIndex.LocalScaleX]);
                animClip.SetCurve(kvp.Key, typeof(Transform), "localScale.y", kvp.Value[(int)AnimationCurveIndex.LocalScaleY]);
                animClip.SetCurve(kvp.Key, typeof(Transform), "localScale.z", kvp.Value[(int)AnimationCurveIndex.LocalScaleZ]);

                //IsActive curve
                animClip.SetCurve(kvp.Key, typeof(GameObject), "m_IsActive", kvp.Value[(int)AnimationCurveIndex.IsActive]);
            }
        }

        public void SetCurveRecursive(Transform root, float time)
        {
            SetCurveRecursive(root, root, time);
        }

        private void SetCurveRecursive(Transform root, Transform current, float time)
        {
            SetCurve(root, current, time);
            foreach(Transform child in current.transform)
            {
                SetCurveRecursive(root, child, time);
            }
        }

        public void SetCurve(Transform root, Transform current, float time)
        {
            var path = AnimationUtility.CalculateTransformPath(current, root);
            var curves = GetOrCreateAnimationCurves(path);
            UpdateTransformCurve(curves, current, time);
        }

        private void UpdateTransformCurve(AnimationCurve[] curves, Transform current, float time)
        {
            float val;
            //IsActive curve
            val = (current.gameObject.activeSelf) ? 1.0f : 0.0f;
            curves[(int)AnimationCurveIndex.IsActive].AddKeyIfChanged(new Keyframe(time, val, float.PositiveInfinity, float.PositiveInfinity) { tangentMode = 0 });

            //Position curves
            curves[(int)AnimationCurveIndex.LocalPositionX].AddKeyIfChanged(new Keyframe(time, current.localPosition.x) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalPositionY].AddKeyIfChanged(new Keyframe(time, current.localPosition.y) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalPositionZ].AddKeyIfChanged(new Keyframe(time, current.localPosition.z, float.PositiveInfinity, float.PositiveInfinity));

            //Rotation curves
            var quat = Quaternion.Euler(current.localEulerAngles);
            curves[(int)AnimationCurveIndex.LocalRotationX].AddKeyIfChanged(new Keyframe(time, quat.x) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalRotationY].AddKeyIfChanged(new Keyframe(time, quat.y) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalRotationZ].AddKeyIfChanged(new Keyframe(time, quat.z) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalRotationW].AddKeyIfChanged(new Keyframe(time, quat.w) { tangentMode = 0 });

            //Scale curves
            curves[(int)AnimationCurveIndex.LocalScaleX].AddKeyIfChanged(new Keyframe(time, current.localScale.x) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalScaleY].AddKeyIfChanged(new Keyframe(time, current.localScale.y) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalScaleZ].AddKeyIfChanged(new Keyframe(time, current.localScale.z) { tangentMode = 0 });
        }

        private AnimationCurve[] GetOrCreateAnimationCurves(string path)
        {
            AnimationCurve[] curves;
            if (!curveCache.TryGetValue(path, out curves))
            {
                curveCache[path] = curves = new AnimationCurve[(int)AnimationCurveIndex.ENUM_COUNT];
                for (int i = 0; i < (int)AnimationCurveIndex.ENUM_COUNT; i++)
                    curves[i] = new AnimationCurve();
            }
            return curves;
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
    }
}
