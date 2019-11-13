using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TanksAI
{

    [CreateAssetMenu(menuName = "Tanks AI/Decisions/Find Player By Look")]
    public class FindPlayerDecision : Decision
    {
        [Range(1,50)]
        public int Accuracy = 3;

        public override bool Decide(StateController controller)
        {
            bool targetVisible = FindPlayerByFanShaped(controller);
            return targetVisible;
        }
        
        bool FindPlayerByFanShaped(StateController controller)
        {
            float subAngle = controller.EnemyTankInfo.AlertAngle / Accuracy;
            for (int i = 0; i < Accuracy; i++)
            {
                if (Helper.CheckRayByEulerToFindPlayer(controller,
                    Quaternion.Euler(0, -controller.EnemyTankInfo.AlertAngle / 2 + i * subAngle + Mathf.Repeat(Time.deltaTime, subAngle), 0)
                    , Color.green))
                {
                    return true;
                }

            }
            return false;
        }

        #region Test
        //bool FindPlayer(StateController controller)
        //{

        //    Debug.DrawRay(controller.Eyes.position, controller.Eyes.forward.normalized * controller.EnemyTankInfo.LookRange, Color.green);

        //    RaycastHit hit;

        //    if (Physics.Raycast(controller.Eyes.position, controller.Eyes.forward, out hit, controller.EnemyTankInfo.LookRange, 1 << 9)
        //        && hit.collider.CompareTag("Player"))
        //    {
        //        controller.targetObj = hit.transform;
        //        return true;
        //    }
        //    return false;
        //}
        #endregion
    }

}