using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tank 基本信息
/// </summary>
[CreateAssetMenu(menuName ="Tanks AI/EnemyInfo")]
public class EnemyInfo : ScriptableObject
{
    #region tank's base info
    /// <summary> tank颜色</summary>
    public Color TankColor = Color.red;

    /// <summary> 生命值</summary>
    public float Health=400f;
    /// <summary>移动距离 </summary>
    public float MoveSpeed = 1f;
    /// <summary>旋转速度</summary>
    public float TurnSpeed = 120f;
    /// <summary>防护值 </summary>
    public float Defense = 20f;

    #endregion

    #region tank's attack info

    /// <summary>直线警戒距离</summary>
    public float LookRange = 50f;
    /// <summary>球形警戒范围半径</summary>
    public float LookSphereCastRadius = 20f;
    ///<summary> 警戒角度</summary>
    public float AlertAngle = 90f;

    /// <summary>攻击范围</summary>
    public float AttackRange=40f;
    /// <summary>最大攻击频率</summary>
    public float AttackMaxRate = 1f;
    /// <summary>最小攻击频率</summary>
    public float AttackMinRate = 15f;
    /// <summary>最小子弹发射力度</summary>
    public float MinAttackForce = 15f;
    /// <summary>最大子弹发射力度</summary>
    public float MaxAttackForce = 30f;
    /// <summary>最大伤害值</summary>
    public float MaxDamage = 60f;

    /// <summary>查寻间隔时间</summary>
    public float SearchDuration = 4f;
    /// <summary> 搜索查找范围半径</summary>
    public float SearchCastRadius = 40f;
    #endregion
}
