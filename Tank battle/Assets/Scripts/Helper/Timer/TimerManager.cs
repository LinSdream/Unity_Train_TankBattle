using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LS.Common;

namespace LS.Helper.Timer
{

    /**
     * 计时器管理器
     */
    public class TimerManager : ASingletonBasis<TimerManager>
    {
        private List<Timer> _timers;
        private Dictionary<string, Timer> _timersDict;

        protected override void Awake()
        {
            base.Awake();
            _timers = new List<Timer>();
            _timersDict = new Dictionary<string, Timer>();
        }

        private void Update()
        {
            foreach(Timer t in _timers)
            {
                t.OnUpdate(Time.deltaTime);
            }
        }

        public void AddTimer(string name,Timer timer)
        {
            if (_timersDict.ContainsKey(name))
            {
                _timersDict[name].LeftTime += _timersDict[name].Duration;
            }
            else
            {
                _timersDict.Add(name, timer);
                _timers.Add(timer);
            }
        }

        public void RemoveTimer(string name)
        {
            Timer timer = _timersDict[name];
            if (_timers != null)
            {
                _timers.Remove(timer);
                _timersDict.Remove(name);
            }
            else
            {
                Debug.LogError($"RemoveTimer Error : can't have the timer whitch name is {name}");
                return;
            }
        }

    }

}