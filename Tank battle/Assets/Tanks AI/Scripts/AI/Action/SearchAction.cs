using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TanksAI
{
    public class SearchAction : Action
    {

        public float Offset = 5f;
        public override void Act(StateController controller)
        {
            Search(controller);
        }

        void Search(StateController controller)
        {


            if (controller.navMeshAgent.remainingDistance <= (controller.navMeshAgent.stoppingDistance + Offset)
                 && !controller.navMeshAgent.pathPending)
            {
                controller.navMeshAgent.destination = Helper.GetRandomLocaion(controller);
            }
        }
        
    }

}