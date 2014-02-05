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
using Assets.ThirdParty.Spriter2Unity.Editor.Spriter;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Unity
{
    using Animation = Spriter.SpriterAnimation;
    public class AnimationBuilder
    {
        Dictionary<Timeline, GameObject> lastGameObjectCache = new Dictionary<Timeline, GameObject>(); //Used to determine active/inactive toggle
        Dictionary<Timeline, TimelineKey> lastKeyframeCache = new Dictionary<Timeline, TimelineKey>();

        List<AnimationEvent> animationEvents = new List<AnimationEvent>();
        AnimationCurveBuilder acb;

        public void BuildAnimationClips(GameObject root, Entity entity, string scmlAssetPath)
        {
            var allAnimClips = AssetDatabase.LoadAllAssetRepresentationsAtPath(scmlAssetPath).OfType<AnimationClip>().ToList();
            //Debug.Log(string.Format("Found {0} animation clips at {1}", allAnimClips.Length, scmlAssetPath));
            
            foreach (var animation in entity.Animations)
            {
                var animClip = MakeAnimationClip(root, animation);
                //Debug.Log(string.Format("Added animClip({0}) to asset path ({1}) WrapMode:{2}", animClip.name, scmlAssetPath, animClip.wrapMode));
                
                var originalAnimClip = allAnimClips.Where(clip => clip.name == animClip.name).FirstOrDefault();
                if (originalAnimClip != null)
                {
                    Debug.Log("Replacing animation clip " + animClip.name);
                    EditorUtility.CopySerialized(animClip, originalAnimClip);
                    allAnimClips.Remove(originalAnimClip);
                }
                else
                    AssetDatabase.AddObjectToAsset(animClip, scmlAssetPath);
            }

            //Remove any animation clips that are no longer present in the SCML
            foreach(var clip in allAnimClips)
            {
                //This may be a bad idea
                UnityEngine.Object.DestroyImmediate(clip, true);
            }
        }

        public AnimationClip MakeAnimationClip(GameObject root, Animation animation)
        {
            //Clear local caches
            lastGameObjectCache.Clear();
            animationEvents.Clear();
            lastKeyframeCache.Clear();

            var animClip = new AnimationClip();
            animClip.name = animation.Name;

            //Set clip to Generic type
            AnimationUtility.SetAnimationType(animClip, ModelImporterAnimationType.Generic);

            //Populate the animation curves & events
            MakeAnimationCurves(root, animClip, animation);

            //Add events to the clip
            AnimationUtility.SetAnimationEvents(animClip, animationEvents.ToArray());

            return animClip;
        }

        private void MakeAnimationCurves(GameObject root, AnimationClip animClip, Animation animation)
        {
            acb = new AnimationCurveBuilder();

            //Set all gameobjects to inactive in first frame
            SetActiveRecursive(root.transform, false);
            root.SetActive(true);

            foreach (var mainlineKey in animation.MainlineKeys)
            {
                //Debug.Log(string.Format("Starting MainlineKey for {0} at {1} seconds", animation.Name, mainlineKey.Time));
                SetGameObjectForKey(root, animClip, mainlineKey);

                //Take a snapshot for our animation
                //AnimationMode.SampleAnimationClip(root, animClip, mainlineKey.Time);
                //acb.SetCurveRecursive(root.transform, mainlineKey.Time);
            }

            switch(animation.LoopType)
            {
                case LoopType.True:
                    //Cycle back to first frame
                    SetGameObjectForKey(root, animClip, animation.MainlineKeys.First());
                    acb.SetCurveRecursive(root.transform, animation.Length);
                    break;
                case LoopType.False:
                    //Duplicate the last key at the end time of the animation
                    acb.SetCurveRecursive(root.transform, animation.Length);
                    break;
                default:
                    Debug.LogWarning("Unsupported loop type: " + animation.LoopType.ToString());
                    break;
            }

            //Add the curves to our animation clip
            acb.AddCurves(animClip);

            //Set the loop/wrap settings for the animation clip
            var animSettings = AnimationUtility.GetAnimationClipSettings(animClip);
            switch (animation.LoopType)
            {
                case LoopType.True:
                    animClip.wrapMode = WrapMode.Loop;
                    animSettings.loopTime = true;
                    break;
                case LoopType.False:
                    animClip.wrapMode = WrapMode.ClampForever;
                    break;
                case LoopType.PingPong:
                    animClip.wrapMode = WrapMode.PingPong;
                    animSettings.loopTime = true;
                    break;
                default:
                    animClip.wrapMode = WrapMode.Once;
                    break;
            }
            animClip.SetAnimationSettings(animSettings);
            //Debug.Log(string.Format("Setting animation {0} to {1} loop mode (WrapMode:{2}  LoopTime:{3}) ", animClip.name, animation.LoopType, animClip.wrapMode, animSettings.loopTime));
        }

        private void SetGameObjectForKey(GameObject root, AnimationClip animClip, MainlineKey mainlineKey)
        {
            //Could do this recursively - this is easier
            Stack<Ref> toProcess = new Stack<Ref>(mainlineKey.GetChildren(null));

            while (toProcess.Count > 0)
            {
                var next = toProcess.Pop();

                SetGameObjectForRef(root, next);
                SetSpriteEvent(animClip, mainlineKey.Time, next);

                var children = mainlineKey.GetChildren(next);
                foreach (var child in children) toProcess.Push(child);
            }
        }

        private void SetGameObjectForRef(GameObject root, Ref childRef)
        {
            TimelineKey key = childRef.Referenced;

            TimelineKey lastKey;
            //Early out - if the key hasn't changed
            if (lastKeyframeCache.TryGetValue(key.Timeline, out lastKey) && key == lastKey)
            {
                return;
            }

            //Get the relative path based on the current hierarchy
            var relativePath = childRef.RelativePath;

            //If this is the root, skip it
            if (string.IsNullOrEmpty(relativePath))
            {
                Debug.Log("Skipping root node in SetGameObjectForRef (SHOULD NEVER HAPPEN)");
                return;
            }


            //Find the gameObject based on relative path
            var transform = root.transform.Find(relativePath);
            if (transform == null)
            {
                Debug.LogError("ERROR: Unable to find GameObject at relative path " + relativePath);
                return;
            }

            var gameObject = transform.gameObject;
            gameObject.SetActive(true);

            //Get transform data from ref
            Vector3 localPosition;
            Vector3 localScale;
            Vector3 localEulerAngles;

            childRef.BakeTransforms(out localPosition, out localEulerAngles, out localScale);

            //Set the current GameObject's transform data
            transform.localPosition = localPosition;
            transform.localScale = localScale;

            //Spriter limits rotation between keyframes to 180 degrees (ie: 5 degrees to 350 degrees is a -15 degree rotation, NOT a 345 degree one)
            var oldEulerAngles = transform.localEulerAngles;
            if (oldEulerAngles.z - localEulerAngles.z > 180) localEulerAngles.z += 360;
            else if (localEulerAngles.z - oldEulerAngles.z > 180) localEulerAngles.z -= 360;
            transform.localEulerAngles = localEulerAngles;

            acb.SetCurve(root.transform, transform, key.Time, lastKey);

            //Get last-used game object for this Timeline - needed to clean up reparenting
            GameObject lastGameObject;
            if (lastGameObjectCache.TryGetValue(key.Timeline, out lastGameObject) && gameObject != lastGameObject)
            {
                //Let Unity handle the global->local position cruft for us
                lastGameObject.transform.position = transform.position;
                lastGameObject.transform.eulerAngles = transform.eulerAngles;

                //TODO: Also need to do something about scale - this is a little more tricky
                lastGameObject.transform.localScale = localScale;

                //Deactivate the old object
                lastGameObject.SetActive(false);

                acb.SetCurve(root.transform, lastGameObject.transform, key.Time, lastKey);
            }

            //Set cached value for last keyframe
            lastKeyframeCache[key.Timeline] = key;
        }

        /// <summary>
        /// Recursively calls SetActive on transform and all children
        /// </summary>
        private void SetActiveRecursive(Transform root, bool isActive)
        {
            foreach (Transform child in root.transform)
            {
                SetActiveRecursive(child, isActive);
            }
            root.gameObject.SetActive(isActive);
        }

        /// <summary>
        /// Creates an event to change the sprite for the specified Ref (if applicable)
        /// </summary>
        /// <param name="clip">Target AnimationClip for Event</param>
        /// <param name="time">Time at which event should be triggered</param>
        /// <param name="reference"></param>
        private void SetSpriteEvent(AnimationClip clip, float time, Ref reference)
        {
            //Bump any events at t=0 up slightly
            if (time < float.Epsilon) time = 0.001f;
            var spriteKey = reference.Referenced as SpriteTimelineKey;
            //Only add event for SpriteTimelineKey objects
            if (spriteKey != null)
            {
                //Pack parameters into a string - simplest way to pass multiple parameters currently
                string packedParam = string.Format("{0};{1};{2}",
                    reference.RelativePath,
                    spriteKey.File.Folder.Id,
                    spriteKey.File.Id);

                //Debug.Log(string.Format("Adding event: ChangeSprite(\"{0}\") at t={1}", packedParam, time));

                //Add events to a list - Unity forces us to set the entire array at once
                animationEvents.Add(new AnimationEvent() { functionName = "ChangeSprite", stringParameter = packedParam, time = time });
            }
        }
    }
}
