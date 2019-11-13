using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TanksAI
{
    [CreateAssetMenu(menuName ="Tanks AI/Decisions/Being Attacked")]
    public class BeingAttackedDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            return IsAttacked(controller);
        }

        bool IsAttacked(StateController controller)
        {
            if (controller.tankHealth.AIflag)
            {
                controller.tankHealth.AIflag = false;
                return true;
            }
            return false;
        }
    }

}