using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TanksAI
{
    /// <summary>
    /// 状态过渡器
    /// </summary>
    [System.Serializable]
    public class Transition
    {
        /// <summary> 过渡条件判断 </summary>
        public Decision Condition;
        /// <summary> 满足条件状态</summary>
        public State TrueState;
        /// <summary> 不满足条件状态</summary>
        public State FalseState;
    }

}