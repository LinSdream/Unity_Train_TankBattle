using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LS.Helper.Timer
{
    /**
     * 简单的计时器(3种实现方法，Update中跑Time，利用协程挂起来跑，Invoke来跑)
     * 在Update中跑，其它的不是很清楚= =
     * 参考文献：https://blog.csdn.net/u012565990/article/details/78082324
     * */
     
    public class Timer 
    {
        public float Duration;
        public float LeftTime;

        Action _updateAction;
        Action _callAction;

        private bool _isPause;
        public bool TimePause { set; private get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="duration">持续时间</param>
        /// <param name="initAction">初始化状态</param>
        /// <param name="updateAction">每帧的事件</param>
        /// <param name="callAction">结束后的回调事件</param>
        public Timer(float duration, Action initAction = null,Action updateAction=null,Action callAction=null)
        {
            LeftTime = duration;
            Duration = duration;
            initAction?.Invoke();
            _updateAction = updateAction;
            _callAction = callAction;
        }

        public  void OnUpdate(float deltaTime)
        {
            LeftTime -= deltaTime;
            if (LeftTime <= 0)
            {
                _callAction?.Invoke();
            }
            else
            {
                if (!_isPause && _updateAction != null)
                {
                    _updateAction.Invoke();
                }
            }
        }
        
    }

}
