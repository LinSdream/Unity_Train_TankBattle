using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TanksAI
{
    [CreateAssetMenu(menuName = "Tanks AI/Actions/Track Action")]
    public class TrackAction : Action
    {
        public override void Act(StateController controller)
        {
            Track(controller);
        }

        void Track(StateController controller)
        {
            
            controller.navMeshAgent.destination = controller.targetObj.position;
            if (controller.targetObj == null)
            {
                controller.TransitionToState(controller.PreviousState);
            }
            //Vector3 offest = controller.transform.position - controller.targetObj.position;
            //if (offest.magnitude <= controller.EnemyTankInfo.AttackRange)
            //{
            //    controller.navMeshAgent.stoppingDistance = controller.EnemyTankInfo.AttackRange / 2;
            //    controller.transform.LookAt(controller.targetObj);
            //}
            //else
            //{
            //    controller.navMeshAgent.stoppingDistance = 0;
            //}
                
        }
    }

}