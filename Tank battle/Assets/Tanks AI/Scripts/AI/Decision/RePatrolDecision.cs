using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TanksAI
{
    [CreateAssetMenu(menuName = "Tanks AI/Decisions/RePatrol")]
    public class RePatrolDecision : Decision
    {
        [Range(1, 50)]
        public int Accuracy = 3;
        public float LosePlayerDurationCanReturnPatrol = 100f;

        public override bool Decide(StateController controller)
        {
            return RePatrol(controller);
        }

        bool RePatrol(StateController controller)
        {

            if (FindPlayerByFanShaped(controller))
            {
                controller.ResetLocalTimer();
                return false;
            }
            else
            {
                return controller.CheckLocalTimerExpires(LosePlayerDurationCanReturnPatrol);
            }

            
        }


        bool FindPlayerByFanShaped(StateController controller)
        {
            float subAngle = controller.EnemyTankInfo.AlertAngle / Accuracy;
            for (int i = 0; i < Accuracy; i++)
            {
                if (Helper.CheckRayByEulerToFindPlayer(controller,
                    Quaternion.Euler(0, -controller.EnemyTankInfo.AlertAngle / 2 + i * subAngle + Mathf.Repeat(Time.deltaTime, subAngle), 0)
                    , Color.red))
                {
                    return true;
                }

            }
            return false;
        }


    }
}
