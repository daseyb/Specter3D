using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class InputHelper
{
    private class DoubleTapTimer
    {
        public KeyCode Key;
        public Action Callback;
        public float DoubleTapTime;

        private float timer = 0;

        public void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                if(timer <= 0)
                {
                    timer = 0;
                }
            }

            if(Input.GetKeyDown(Key))
            {
                if(timer > 0)
                {
                    Callback();
                    timer = 0;
                }
                else
                {
                    timer = DoubleTapTime;
                }
            }
        }
    }

    List<DoubleTapTimer> doubleTapTimers = new List<DoubleTapTimer>();

    public void RegisterDoubleTap(KeyCode key, Action callback, float time)
    {
        doubleTapTimers.Add(new DoubleTapTimer() { Key = key, Callback = callback, DoubleTapTime = time });
    }

    public void UnregisterDoubleTap(KeyCode key, Action callback)
    {
        doubleTapTimers.RemoveAll(timer => timer.Key == key && timer.Callback == callback);
    }

    public void Update()
    {
        foreach(var timer in doubleTapTimers)
        {
            timer.Update();
        }
    }
}
