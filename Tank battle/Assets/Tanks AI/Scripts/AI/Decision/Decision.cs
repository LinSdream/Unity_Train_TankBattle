using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TanksAI
{
    /// <summary>
    /// 状态转换条件
    /// </summary>
    public abstract class Decision : ScriptableObject
    {
        public abstract bool Decide(StateController controller);
    }
}