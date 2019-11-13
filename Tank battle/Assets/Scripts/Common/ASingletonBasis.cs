using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LS.Common
{
    /*
     * 参考文档：https://blog.csdn.net/ycl295644/article/details/49487361/
     */

    /// <summary>
    /// 单例类基类，继承MonoBehaviour,切换场景后不会自动删除
    /// </summary>
    public abstract class ASingletonBasis<T> : MonoBehaviour where T : ASingletonBasis<T>//where 约束子类
    {

        protected bool _IsDestroy = false;

        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)//如果不存在
                {
                    _instance = FindObjectOfType(typeof(T)) as T;//在目前已加载的脚本中查找该单例
                    if (_instance == null)//创建单例
                    {
                        GameObject obj = new GameObject("Main :" + typeof(T).Name);
                        _instance = obj.AddComponent<T>();
                        DontDestroyOnLoad(obj);
                    }
                }
                return _instance;
            }
        }
        
        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (_instance == null)
            {
                _instance = this as T;
                Init();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected virtual void Start()
        {

        }

        /// <summary>
        /// 初始化该单例
        /// </summary>
        public virtual void Init()
        {
            return;
        }

        /// <summary>
        /// 销毁该单例
        /// </summary>
        public virtual void Release()
        {
            return;
        }
    }
}