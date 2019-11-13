using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TanksAI
{
    [CreateAssetMenu(menuName ="Tanks AI/Actions/Random Walk")]
    public class RandomWalkAction : Action
    {
        public float Offset = 5f;
        
        float _timer = 0f;

        public override void Act(StateController controller)
        {
            RandomWalk(controller);
        }

        void RandomWalk(StateController controller)
        {
            if (controller.navMeshAgent.remainingDistance <= (controller.navMeshAgent.stoppingDistance+Offset)
                && !controller.navMeshAgent.pathPending)
            {
                controller.navMeshAgent.destination = Helper.GetRandomLocaion(controller);
            }
        }
        
    }

}