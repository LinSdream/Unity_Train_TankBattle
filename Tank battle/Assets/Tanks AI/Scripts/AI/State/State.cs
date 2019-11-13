using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TanksAI
{
    /// <summary>
    /// 状态机
    /// </summary>
    [CreateAssetMenu(menuName ="Tanks AI/State")]
    public class State : ScriptableObject
    {
        public Action[] Actions;//事件列表
        public Transition[] Transitions;//状态转化器列表

        public Color SceneGizomColor = Color.white;

        public void UpdateState(StateController controller)
        {
            DoActions(controller);
            CheckTransition(controller);
        }

        //执行事件列表
        void DoActions(StateController controller)
        {
            for(int i = 0; i < Actions.Length; i++)
            {
                Actions[i].Act(controller);
            }
        }

        //检查状态转换器中的所有过渡条件
        void CheckTransition(StateController controller)
        {
            for (int i = 0; i < Transitions.Length; i++)
            {
                bool decisionSucceeded = Transitions[i].Condition.Decide(controller);
                if (decisionSucceeded)
                {
                    controller.TransitionToState(Transitions[i].TrueState);
                }
                else
                {
                    controller.TransitionToState(Transitions[i].FalseState);
                }
            }
        }
        

    }
}