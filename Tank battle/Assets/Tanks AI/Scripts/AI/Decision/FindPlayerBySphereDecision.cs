using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TanksAI
{
    [CreateAssetMenu(menuName ="Tanks AI/Decisions/Find Player By Sphere")]
    public class FindPlayerBySphereDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            return FindPlayerBySphere(controller);
        }

         bool FindPlayerBySphere(StateController controller)
        {
            Collider[] colliders = Physics.OverlapSphere(controller.Eyes.position,controller.EnemyTankInfo.LookSphereCastRadius,1<<9);
            for(int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].CompareTag("Player"))
                {
                    controller.targetObj = colliders[i].transform;
                    return true;
                }
            }
            return false;
        }
    }

}