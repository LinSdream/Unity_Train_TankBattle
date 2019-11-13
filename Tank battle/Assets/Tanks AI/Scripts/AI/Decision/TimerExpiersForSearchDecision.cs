using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TanksAI
{
    [CreateAssetMenu(menuName ="Tanks AI/Decisions/Timer Expiers For Search")]
    public class TimerExpiersForSearchDecision : Decision
    {
        public float SearchTime = 15f;

        float _timer;

        public override bool Decide(StateController controller)
        {
            return TimerExpier(controller);
        }

        bool TimerExpier(StateController controller)
        {
            _timer += Time.deltaTime;
            if (_timer >= SearchTime)
            {
                _timer = 0;
                return true;
            }

            return false;
        }
    }
}

