/*
Copyright (c) 2014 Andrew Jones
 Based on 'Spriter2Unity' python code by Malhavok

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
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
            curves[(int)AnimationCurveIndex.IsActive].AddKey(new Keyframe(time, val, float.PositiveInfinity, float.PositiveInfinity) { tangentMode = 0 });

            //Position curves
            curves[(int)AnimationCurveIndex.LocalPositionX].AddKey(new Keyframe(time, current.localPosition.x) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalPositionY].AddKey(new Keyframe(time, current.localPosition.y) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalPositionZ].AddKey(new Keyframe(time, current.localPosition.z, float.PositiveInfinity, float.PositiveInfinity));

            //Rotation curves
            var quat = Quaternion.Euler(current.localEulerAngles);
            curves[(int)AnimationCurveIndex.LocalRotationX].AddKey(new Keyframe(time, quat.x) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalRotationY].AddKey(new Keyframe(time, quat.y) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalRotationZ].AddKey(new Keyframe(time, quat.z) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalRotationW].AddKey(new Keyframe(time, quat.w) { tangentMode = 0 });

            //Scale curves
            curves[(int)AnimationCurveIndex.LocalScaleX].AddKey(new Keyframe(time, current.localScale.x) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalScaleY].AddKey(new Keyframe(time, current.localScale.y) { tangentMode = 0 });
            curves[(int)AnimationCurveIndex.LocalScaleZ].AddKey(new Keyframe(time, current.localScale.z) { tangentMode = 0 });
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
