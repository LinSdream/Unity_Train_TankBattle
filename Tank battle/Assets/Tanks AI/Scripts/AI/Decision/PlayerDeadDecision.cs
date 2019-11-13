using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TanksAI
{
    [CreateAssetMenu(menuName ="Tanks AI/Decisions/Player Dead")]
    public class PlayerDeadDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            return PlayerDead(controller);
        }

        bool PlayerDead(StateController controller)
        {
            if (controller.targetObj == null)
                return true;
            return false;
        }


    }

}