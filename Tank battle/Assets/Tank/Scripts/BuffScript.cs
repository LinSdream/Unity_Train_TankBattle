using LS.Helper.Test;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffScript : MonoBehaviour
{

    #region Fields

    public Buff Info;
    public Slider BuffTimerSlider;
    public Image BuffIco;

    float _currentValue = 0f;
    float _initValue = 0f;
    bool _lock = false;
    public float Value => _currentValue / _initValue;
    #endregion

    #region MonoBehaviour Callbacks
    private void Update()
    {
        Debug.Log("!!!"+Value);
        BuffTimerSlider.value = Value;
    }

    private void OnDisable()
    {
        BuffIco.color = Color.white;
        BuffTimerSlider.value = 1;
    }

    #endregion

    #region Public Methods

    public void InitInfo(Buff info)
    {
        Debug.Log("Into InitInfo" + info.BuffName);
        Info = info;
        switch (info.BuffName)
        {
            case "AttackBuff":
                BuffIco.color = Color.red;
                break;
            default:
                BuffIco.color = Color.black;
                break;
        }
        if (!Info.Once)
        {
            BuffTimerSlider.value = 1;
            _currentValue = _initValue = info.Duration;
        }
        StartCoroutine(WaitPerSeconds());
    }

    #endregion

    IEnumerator WaitPerSeconds()
    {
        while (_currentValue <= 0)
        {
            _currentValue--;
            yield return new WaitForSeconds(1);
        }
        Destroy(gameObject);
    }
}
