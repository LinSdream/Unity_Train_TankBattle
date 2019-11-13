using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LS.Helper.Prop
{
    /// <summary>
    /// 道具基类
    /// </summary>
    public abstract class BaseProp : MonoBehaviour
    {
        public PropInfo Info;
        
        protected void Awake()
        {
            gameObject.tag = "Props";
            Init();
        }
        
        public abstract void Excute(GameObject obj);

        public virtual void Init() { }
    }
}
