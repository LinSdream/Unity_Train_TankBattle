using LS.Helper.Prop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/**
 * 更改速度道具
 * */
public class SpeedProp : BaseProp
{

    public float Speed;

    public override void Excute(GameObject obj)
    {
    //    PlayerController script = obj.GetComponent<PlayerController>();
    //    TimerManager.Instance.AddTimer("AddSpeed", new Timer(Duration,
    //    () =>
    //    {
    //        InitTimer(script);
    //    },
    //    null,
    //    () =>
    //     {
    //         CompleteTimer(script);
    //     }
    //        ));
    //}

    //private void CompleteTimer(PlayerController script)
    //{
    //    if (script != null)
    //    {
    //        script.Speed = 3;
    //    }
    //}

    //private void InitTimer(PlayerController script)
    //{
    //    if (script != null)
    //    {
    //        script.Speed = Speed;
    //    }
    }
    
}