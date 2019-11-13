using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TanksAI
{
    public class AITankManger
    {
        public Color PlayerColor;
        public Transform SpawnPoint;
        [HideInInspector] public int PlayerNum;
        [HideInInspector] public string ColoredPlayerText;//存储富文本信息的字符串，用于显示对应的player每局对战信息
        [HideInInspector] public GameObject Instance;
       // [HideInInspector] public int Wins;

        StateController _controller;
        TankShooting _shooting;
        TankHealth _tankHealth;
        GameObject _canvasGameObjcet;

        public void SetupAI(List<Transform> pointsForPatrol,System.Action action=null)
        {
            _controller = Instance.GetComponent<StateController>();
            _shooting = Instance.GetComponent<TankShooting>();
            _canvasGameObjcet = Instance.GetComponentInChildren<Canvas>().gameObject;
            _tankHealth = Instance.GetComponent<TankHealth>();
            _shooting.PlayerNum = PlayerNum;

            _tankHealth.DoActionAfterDead = action;
            _tankHealth.StartingHealth = _controller.EnemyTankInfo.Health;

            _shooting.MaxLaunchForce = _controller.EnemyTankInfo.MaxAttackForce;
            _shooting.MinLaunchForce = _controller.EnemyTankInfo.MinAttackForce;
            _shooting.Damage = _controller.EnemyTankInfo.MaxDamage;
            
            PlayerColor = _controller.EnemyTankInfo.TankColor;
            _controller.WayPointsForPatrol = pointsForPatrol;
            ////设置代理信息
            //_controller.navMeshAgent.speed = _controller.EnemyTankInfo.MoveSpeed;
            //_controller.navMeshAgent.angularSpeed = _controller.EnemyTankInfo.TurnSpeed;
            //_tankHealth.StartingHealth = _controller.EnemyTankInfo.Health;
            //_controller.WayPointsForPatrol = pointsForPatrol;
            //_controller.nextPointForPatrol = 0;

            //类似html的文本表达方式，<color=# 00FFAA>着色字体</color>
            ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(PlayerColor) + ">PLAYER " + PlayerNum + "</color>";

            //MeshRenderer[] renderers = Instance.GetComponentsInChildren<MeshRenderer>();
            //for (int i = 0; i < renderers.Length; i++)
            //{
            //    renderers[i].material.color = PlayerColor;
            //}
        }

        public void DisableControl()
        {

            _shooting.enabled = false;

            _canvasGameObjcet.SetActive(false);
        }

        public void EnableControl()
        {
            _shooting.enabled = true;
            _canvasGameObjcet.SetActive(true);
        }

        public void Reset()
        {
            Instance.transform.position = SpawnPoint.position;
            Instance.transform.rotation = SpawnPoint.rotation;

            Instance.SetActive(false);
            Instance.SetActive(true);
        }
        
    }
}
