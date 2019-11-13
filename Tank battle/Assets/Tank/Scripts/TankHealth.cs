using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HealthStatus
{
    FULL,//满血
    HEALTH,//健康
    NORMAL,//一般
    URGENT,//告急
    DEATH,//死亡
    NULL,//不明
}
public class TankHealth : MonoBehaviour
{
    #region attributes

    public Slider HealthSlider;//血条Slider
    public float StartingHealth = 100f;//初始生命值
    public Image FillImg;
    public Color FullHealthColor = Color.green;
    public Color MidHealthColor = Color.yellow;
    public Color ZeroHealthColor = Color.red;
    public GameObject ExplosionPrefab;//死亡后，粒子预设体

    //-----------------------------------  AI 受到伤害标记位 start   ---------------------------------------------//

    [HideInInspector] public bool AIflag = false;

    //------------------------------------ AI 受到伤害标记位   end   ---------------------------------------------//

    //------------------------------------- 死亡后的委托事件 start -----------------------------------------------//

    public Action DoActionAfterDead = null;


    //---------------------------------------- 死亡后的委托事件 end-------------------------------------------------//


    [HideInInspector]
    public HealthStatus _healthStatus//根据生命值百分比来设置tank状态，扩展功能
    {
        get
        {
            float healthPercentage = _currentHealth / StartingHealth;
            if (healthPercentage == 1f)
                return HealthStatus.FULL;
            if (healthPercentage > 0.8f && healthPercentage < 1f)
                return HealthStatus.HEALTH;
            if (healthPercentage < 0.8f && healthPercentage > 0.2f)
                return HealthStatus.NORMAL;
            if (healthPercentage < 0.2f && healthPercentage > 0f)
                return HealthStatus.URGENT;
            if (healthPercentage <= 0f)
                return HealthStatus.DEATH;
            return HealthStatus.NULL;
        }
    }

    public float CurrentHealth {
        get
        {
            return _currentHealth;
        }
    }

    AudioSource _explosionAudio;//死亡音效
    ParticleSystem _explosionParticles;//粒子系统
    float _currentHealth;//当前血量

    #endregion

    #region private methods for Unity

    private void Awake()
    {
        _explosionParticles = Instantiate(ExplosionPrefab).GetComponent<ParticleSystem>();
        _explosionAudio = _explosionParticles.GetComponent<AudioSource>();
        _explosionParticles.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _currentHealth = StartingHealth;
        SetHealthUI();
    }

    #endregion

    #region private methods 

    void SetHealthUI()
    {
        HealthSlider.value = (_currentHealth / StartingHealth) * 100;//将生命值按照百分比计算， 然后转为100进制
        switch (_healthStatus)
        {
            case HealthStatus.FULL:
            case HealthStatus.HEALTH:
                FillImg.color = FullHealthColor;
                break;

            case HealthStatus.NORMAL:
                FillImg.color = MidHealthColor;
                break;

            case HealthStatus.URGENT:
                FillImg.color = ZeroHealthColor;
                break;
        }
        //FillImg.color = Color.Lerp(ZeroHealthColor, FullHealthColor, _currentHealth / StartingHealth);
    }

    void OnDeath()
    {

        DoActionAfterDead?.Invoke();

        _explosionParticles.transform.position = transform.position;
        _explosionParticles.gameObject.SetActive(true);

        _explosionParticles.Play();
        _explosionAudio.Play();

       
        gameObject.SetActive(false);
        
    }

    #endregion

    #region public methods AI and Player all used

    public void ChangeHealth(float value)
    {
        float buf = _currentHealth;
        buf += _currentHealth;
        if (buf >= StartingHealth)
            buf = StartingHealth;
        else if (buf <= 0)
            buf = 0;
        _currentHealth = buf;
        SetHealthUI();
    }

    public void TakeDamage(float amount)
    {
        //伤害计算
        _currentHealth -= amount;

        SetHealthUI();

        if (_healthStatus == HealthStatus.DEATH)
        {
            OnDeath();
        }

        //use to judge ai tank has be attacked
        if (!AIflag)
        {
            AIflag = true;
        } 
    }
    #endregion

    #region Coroutines

    IEnumerator ClearTankAfterDead()
    {
        yield return new WaitForSeconds(_explosionParticles.time + 1);
        Destroy(_explosionParticles);
    }

    #endregion

}
