using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TanksAI
{
    /// <summary>
    /// 巡逻事件
    /// </summary>
    [CreateAssetMenu(menuName = "Tanks AI/Actions/Patrol Action")]
    public class PatrolAction : Action
    {
        public override void Act(StateController controller)
        {
            Patrol(controller);
        }

        void Patrol(StateController controller)
        {

            controller.navMeshAgent.destination = controller.WayPointsForPatrol[controller.nextPointForPatrol].position;
            //如果当前代理距离小于停止距离并且计算好下一个路径
            if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance
                && !controller.navMeshAgent.pathPending)
            {
                controller.nextPointForPatrol = (controller.nextPointForPatrol + 1) % controller.WayPointsForPatrol.Count;
            }
        }
    }

}