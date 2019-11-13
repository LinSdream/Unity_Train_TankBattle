using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LS.Helper.Test
{
    public class Timer
    {
        /// <summary>
        /// 计时器时长
        /// </summary>
        public float Duration;
        /// <summary>
        /// 计时器初始化回调
        /// </summary>
        public Action InitDo;
        /// <summary>
        /// 计时器循环回调
        /// </summary>
        public Action AfterDo;
        /// <summary>
        /// 计时器结束后回调
        /// </summary>
        public Action Callback;

        public int Status = -1;

        public Timer()
        {
            Duration = 0f;
        }

        public Timer(float timerDuration)
        {
            Duration = timerDuration;
        }
        
        public Timer(float duration,Action initDo=null,Action afterDo=null,Action callback = null)
        {
            Duration = duration;
            InitDo = initDo;
            AfterDo = afterDo;
            Callback = callback;
        }

        public IEnumerator SimpleTimerForSeconds()
        {
            Status = 1;
            yield return new WaitForSeconds(Duration);
            Status = 0;
        }

        /// <summary>
        /// 正计时器, 可以指定每次停顿时执行的动作
        /// </summary>
        /// <param name="duration">计时时长</param>
        public IEnumerator TimerForSeconds(bool repeat=false)
        {
            Status = 1;
            InitDo?.Invoke();
            do
            {   
                yield return new WaitForSeconds(Duration);
                AfterDo?.Invoke();
            } while (repeat);
            Callback?.Invoke();
            Status = 0;
        }

        /// <summary>
        /// 每帧计时
        /// </summary>
        /// <param name="frameCount">计时总帧数</param>
        /// <param name="beforeDo">计时前操作</param>
        /// <param name="endFrameDo">每帧结束后操作</param>
        /// <param name="callback">计时结束后操作</param>
        public IEnumerator TimerForEndFrame()
        {
            Status = 1;
            if (Duration <= 0)
            {
                Debug.LogError("TimerForEndFrame Error : frameCount must more than zero !");
                yield break;
            }

            InitDo?.Invoke();
            int count = 0;
            do
            {
                count++;
                yield return new WaitForEndOfFrame();
                AfterDo?.Invoke();
            } while (count < Duration);
            Callback?.Invoke();
            Status = 0;
        }

        /// <summary>
        /// 从程序开始以来的真实时间的计时器
        /// </summary>
        /// <param name="time">计时时长</param>
        /// <param name="callback">回调</param>
        public IEnumerator TimerForRealTimeSinceStartup()
        {
            Status = 1;
            float start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < start + Duration)
            {
                yield return null;
                AfterDo();
            }
            Callback?.Invoke();
            Status = 0;
        }
        
        public IEnumerator NextFrame()
        {
            Status = 1;
            yield return new WaitForEndOfFrame();
            Callback?.Invoke();
            Status = 0;
        }

        public IEnumerator NextFixedFrame()
        {
            Status = 1;
            yield return new WaitForFixedUpdate();
            Callback?.Invoke();
            Status = 0;
        }
        
    }
}