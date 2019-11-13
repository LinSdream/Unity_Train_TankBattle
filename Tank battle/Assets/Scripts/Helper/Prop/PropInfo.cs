using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LS.Helper.Prop {

    [CreateAssetMenu(fileName ="Prop Info")]
    public class PropInfo : ScriptableObject
    {
        /// <summary> 道具名称 </summary>
        public string PropName = "";
        /// <summary>道具持续时间</summary>
        public float Duration = 0f;
        /// <summary> 是否一次性 </summary>
        public bool Once = true;
        /// <summary> 是否叠加 </summary>
        public bool Superimposed = false;
        /// <summary> 数值 </summary>
        public float Value = 0f;
    }
    
}
