using LS.Helper.Prop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LS.Helper.Timer;
using LS.Helper.Test;
using UnityEngine.UI;

public class TankBuffStatus : MonoBehaviour
{
    #region Fields
    public List<Buff> Buffs;
    public List<GameObject> BuffsImgs;
    public GameObject BuffPrefab;
    public GridLayoutGroup BuffGroup;
    #endregion

    #region MonoBehaviour Callbacks
    // Start is called before the first frame update
    void Start()
    {
        Buffs = new List<Buff>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Props"))
        {
            other.GetComponent<BaseProp>().Excute(gameObject);
            other.gameObject.SetActive(false);
        }

    }
    #endregion

    #region Public Methods
    public void AddBuff(Buff buff, Action<GameObject> init = null, Action<GameObject> update = null, Action<GameObject> callback = null)
    {
        if (buff.Once)
        {
            init?.Invoke(gameObject);
            return;
        }

        if (Buffs.Exists((value) =>
        {
            value.BuffName = buff.BuffName;
            return true;
        }))
        {
            return;
        }

        Buffs.Add(buff);
        SetBuffsPanel(buff);
        TimerController.Instance.StartTimerForSeconds(buff.BuffName, new LS.Helper.Test.Timer(buff.Duration,
        () =>
        {
            init?.Invoke(gameObject);
            
        },
        () =>
        {
            update?.Invoke(gameObject);
        },
        () =>
        {
            callback?.Invoke(gameObject);
            buff.ReturnAction(gameObject, buff);
            Buffs.Remove(buff);
        }
        ));
    }

    #endregion

    #region Private Methods

    void AddBuffCell(Buff info)
    {
        
    }

    // GameObject GetBuffImg(Buff info)
    //{
    //    Debug.Log("!");
    //    GameObject obj = null;
    //    foreach (GameObject o in BuffsImgs)
    //    {
    //        if (!o.activeInHierarchy)
    //        {
    //            obj = o;
    //            break;
    //        }
    //    }
    //    if (obj == null)
    //    {
    //        Debug.LogWarning("TankBuffStatus/GetBuffImf Warning : Can't get empty BuffImg !");
    //        return null;
    //    }
    //    obj.SetActive(true);
    //    SetBuffsPanel(obj, info);
    //    return obj;
    //}

    void SetBuffsPanel(Buff info)
    {
        var instance = Instantiate(BuffPrefab);
        instance.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
        instance.GetComponent<RectTransform>().SetParent(BuffGroup.transform);
        
        Debug.Log(instance.GetComponent<RectTransform>().position);
        var script = instance.GetComponent<BuffScript>();
        script.InitInfo(info);
        BuffGroup.transform.SetAsFirstSibling();
        BuffGroup.transform.SetAsLastSibling();
    }

    #endregion

}
