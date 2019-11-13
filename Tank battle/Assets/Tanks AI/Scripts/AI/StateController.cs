using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TanksAI
{
    /// <summary>
    /// 状态控制器
    /// </summary>
    public class StateController : MonoBehaviour
    {

        #region attributes

        public Transform Eyes;
        public EnemyInfo EnemyTankInfo;//Tank AI 信息
        public State CurrentState;//当前状态
        public State RemainState;
        public List<Transform> WayPointsForPatrol;             //巡逻点集

        [HideInInspector] public State PreviousState = null;                     //上一个的状态
        [HideInInspector] public TankShooting tankShooting;                      //Tank 射击控制脚本
        [HideInInspector] public TankHealth tankHealth;                          //Tank 生命值脚本
        [HideInInspector] public NavMeshAgent navMeshAgent;                      //Tank 导航网格
        [HideInInspector] public Transform targetObj;                            //目标对象
        [HideInInspector] public int nextPointForPatrol = 0;                     //下一个巡逻点
        
        float _localTimer;                                     //局部计时器,PS：状态改变的时候，局部计时器会被重置
        float _globalTimer;                                    //全局计时器，PS:状态改变的时候，不被重置

        #endregion 

        #region private methods in Unity

        private void Awake()
        {
            tankShooting = GetComponent<TankShooting>();
            tankHealth = GetComponent<TankHealth>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        // Start is called before the first frame update
        void Start()
        {
            InitTankAiInfo();
        }

        // Update is called once per frame
        void Update()
        {
            CurrentState.UpdateState(this);
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = CurrentState.SceneGizomColor;
            Gizmos.DrawWireSphere(transform.position, EnemyTankInfo.LookSphereCastRadius);
        }

        #endregion

        #region public methods

        public void TransitionToState(State nextState)
        {
            if (nextState != this.RemainState)
            {
                OnExitState();                      //退出状态时候进行信息重置
                PreviousState = CurrentState;
                CurrentState = nextState;
            }
        }

        /// <summary>
        /// 检查局部计时器是否到达指定时间
        /// </summary>
        /// <param name="duration">计时器持续时间</param>
        public bool CheckLocalTimerExpires(double duration)
        {
            _localTimer += Time.deltaTime;
            return _localTimer >= duration;
        }

        /// <summary>
        /// 检查全局计时器是否到达指定时间
        /// </summary>
        /// <param name="duration">计时器持续时间</param>
        public bool CheckGlobalTimerExpiers(double duration)
        {
            _globalTimer += Time.deltaTime;
            return _globalTimer >= duration;
        }

        /// <summary> 重置局部计时器 </summary>
        public void ResetLocalTimer()
        {
            _localTimer = 0f;
        }

        /// <summary> 重置全部计时器 </summary>
        public void ResetGlobalTimer()
        {
            _globalTimer = 0f;
        }

        /// <summary>  重置计时器</summary>
        public void ResetTimer()
        {
            _localTimer = 0f;
            _globalTimer = 0f;
        }
        
        #endregion

        #region private methods

        /// <summary> 退出状态 </summary>
        void OnExitState()
        {
            ResetLocalTimer();
            tankHealth.AIflag = false;
        }

        /// <summary>
        /// 初始化AI代理的tank的数值
        /// </summary>
        void InitTankAiInfo()
        {
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = EnemyTankInfo.TankColor;
            }
            
            navMeshAgent.speed = EnemyTankInfo.MoveSpeed;
            navMeshAgent.angularSpeed = EnemyTankInfo.TurnSpeed;
        }
        
        #endregion

    }

}