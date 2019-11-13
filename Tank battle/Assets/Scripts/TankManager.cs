using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TankManager
{
    public Color PlayerColor;
    public Transform SpawnPoint;
    [HideInInspector] public int PlayerNum;
    [HideInInspector] public string ColoredPlayerText;//存储富文本信息的字符串，用于显示对应的player每局对战信息
    [HideInInspector] public GameObject Instance;
    [HideInInspector] public int Wins;

    TankMovement _movement;
    TankShooting _shooting;
    GameObject _canvasGameObjcet;

    public void Setup()
    {
        _movement = Instance.GetComponent<TankMovement>();
        _shooting = Instance.GetComponent<TankShooting>();
        _canvasGameObjcet = Instance.GetComponentInChildren<Canvas>().gameObject;

        _movement.PlayerNumber = PlayerNum;
        _shooting.PlayerNum = PlayerNum;

        //类似html的文本表达方式，<color=# 00FFAA>着色字体</color>
        ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(PlayerColor) + ">PLAYER " + PlayerNum + "</color>";

        MeshRenderer[] renderers = Instance.GetComponentsInChildren<MeshRenderer>();
        for(int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = PlayerColor;
        }
    }

    public void DisableControl()
    {
        _movement.enabled = false;
        _shooting.enabled = false;

        _canvasGameObjcet.SetActive(false);
    }

    public void EnableControl()
    {
        _movement.enabled = true;
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
