using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TanksAI
{
    [CreateAssetMenu(menuName = "Tanks AI/Actions/Attack Action")]
    public class AttackAction : Action
    {
        public float Gravity = 9.81f;//重力

        float _durationTime = 1f;
        float _timer = 0f;
        public override void Act(StateController controller)
        {
            Attack(controller);
        }

        void Attack(StateController controller)
        {
            float bufffTime = Random.Range(1, 4);//controller.EnemyTankInfo.AttackMaxRate, controller.EnemyTankInfo.AttackMinRate);
            
            _timer += Time.deltaTime;

            if (_timer >= _durationTime)
            {
                controller.tankShooting.FireByAI(CalculationForce(controller,controller.EnemyTankInfo.MaxAttackForce,
                    controller.EnemyTankInfo.MinAttackForce));
                _durationTime = bufffTime;
                _timer = 0f;
            }

            #region Test code has bug
            //if (controller.CheckLocalTimerExpires(1f))
            //{
            //    controller.tankShooting.FireByAI(controller.EnemyTankInfo.MinAttackForce);
            //    controller.ResetLocalTimer();
            //}

            //Debug.Log("!!!");
            //if (controller.CheckLocalTimerExpires(1f))
            //{
            //    Debug.Log("Into Attack");
            //    controller.tankShooting.FireByAI(controller.EnemyTankInfo.MinAttackForce);
            //    controller.ResetLocalTimer();
            //}
            //if (controller.CheckLocalTimerExpires(1))
            //{
            //    Debug.Log("Into Attack");
            //    //controller.tankShooting.FireByAI(controller.EnemyTankInfo.MinAttackForce);
            //    controller.ResetLocalTimer();
            //}
            #endregion
        }


        /// <summary>
        /// 计算水平方向的力（速度）
        /// </summary>
        /// <param name="controller">状态控制器</param>
        /// <param name="max">水平最大力</param>
        /// <param name="min">水平最小力</param>
        float CalculationForce(StateController controller,float max,float min)
        {
            float distance = Helper.CalculationDistance(controller.transform, controller.targetObj);

            //平抛公式计算：
            //y =(1/2)g*t^2;
            //x=vt;
            float y = controller.Eyes.position.y;//获取y的距离
            float t = Mathf.Sqrt((2 * y) / Gravity);//计算着地后的时间t
            float v = distance / t;//计算水平速度 v

            //Debug.Log(""+v);

            if (v > max)//如果v>最大的速度，返回最大速度
                return max;
            else if (v < min)//如果v<最小力，返回最小速度
                return min;
            else
                return v;

        }

    }

}