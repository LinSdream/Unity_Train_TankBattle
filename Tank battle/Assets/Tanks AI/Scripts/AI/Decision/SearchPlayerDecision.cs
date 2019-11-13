using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TanksAI
{
    [CreateAssetMenu(menuName ="Tanks AI/Decisions/Search Player")]
    public class SearchPlayerDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            return Search(controller);
        }

        bool Search(StateController controller)
        {
            
            Collider[] conlliders = Physics.OverlapSphere(controller.transform.position,
                controller.EnemyTankInfo.SearchCastRadius, 1 << 9);

            for (int i = 0; i < conlliders.Length; i++)
            {
                if (conlliders[i].CompareTag("Player"))
                {
                    controller.targetObj = conlliders[i].transform;
                    return true;
                }
                
            }
            return false;
        }
    }

}