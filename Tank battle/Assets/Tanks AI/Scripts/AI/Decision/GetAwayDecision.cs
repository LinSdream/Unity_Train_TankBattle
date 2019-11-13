using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TanksAI
{
    public class GetAwayDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            return GetAway(controller);
        }

        bool GetAway(StateController controller)
        {
            return false;
        }
    }

}