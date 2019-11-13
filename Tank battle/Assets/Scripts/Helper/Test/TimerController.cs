using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LS.Common;

namespace LS.Helper.Test
{

    public class TimerController : ASingletonBasis<TimerController>
    {
        Dictionary<string, Timer> _timerDic;

        protected override void Awake()
        {
            base.Awake();
            _timerDic = new Dictionary<string, Timer>();
        }

        private void Update()
        {
            List<string> end = new List<string>();
            foreach(KeyValuePair<string ,Timer> kv in _timerDic)
            {
                if (kv.Value.Status == 0)
                    end.Add(kv.Key);
            }

            foreach(string s in end)
            {
                _timerDic.Remove(s);
            }

        }

        public void StartTimerForSeconds(string name,Timer timer,bool repeat=false)
        {
            if (_timerDic.ContainsKey(name)){
                _timerDic[name].Duration += timer.Duration;
                return;
            }
            _timerDic.Add(name, timer);
            StartCoroutine(timer.TimerForSeconds(repeat));
        }


        public Timer GetTimer(string name)
        {
            return _timerDic[name];
        }

    }

    //public class TimerController:ASingletonBasis<TimerController>
    //{
    //    #region Fields

    //    Dictionary<string, Timer> _timer;
    //    Dictionary<string, Coroutine> _coroutines;

    //    #endregion

    //    #region MonoBehaviour Callbacks

    //    public override void Init()
    //    {
    //        _timer = new Dictionary<string, Timer>();
    //        _coroutines = new Dictionary<string, Coroutine>();
    //    }

    //    #endregion

    //    #region Public Methods
    //    public void AddTimer(string timerName,float duration=0f)
    //    {
    //        if (_timer.ContainsKey(timerName))
    //        {
    //            Debug.LogError("AddTimer Error : Already exist the timer");
    //            return;
    //        }
    //        Timer timer = new Timer(duration);
    //        timer.Clear = TimerClear;
    //        _timer.Add(timerName, timer);

    //    }

    //    public void StartTimerForSeconds(string timerName, float duration,bool repeat = false, Action beforeDo = null,
    //        Action afterDo=null, Action callback = null)
    //    {
    //        if (!_coroutines.ContainsKey(timerName))
    //        {
    //            _timer[timerName].Duration = duration;
    //            Coroutine coroutine = StartCoroutine(_timer[timerName].TimerForSeconds( repeat));
    //            _coroutines.Add(timerName, coroutine);
    //            return;
    //        }
    //        else
    //        {
    //            _timer[timerName].Duration += duration;
    //        }

    //    }

    //    public void StopTimer(string timerName)
    //    {
    //        StopCoroutine(_coroutines[timerName]);

    //    }

    //    #endregion


    //    #region Private Methods


    //   void TimerClear()
    //    {
    //        _timer.Remove()
    //    }

    //    #endregion


    //}

}