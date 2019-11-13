using LS.Helper.Prop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{

    public string BuffName;

    public object BeforeStatus;

    public float Duration;

    public bool Once;

    public bool Superimposed;

    public System.Action<GameObject,Buff> ReturnAction;

    public Buff() { }

    public Buff(PropInfo info)
    {
        BuffName = info.PropName;
        Duration = info.Duration;
        Once = info.Once;
        Superimposed = info.Superimposed;
    }

    public Buff(string name,object beforeStatus,bool Once=false,float duration=0f,bool superimposed=false)
    {
        BuffName = name;
        BeforeStatus = beforeStatus;
        Duration = duration;
        Superimposed = superimposed;
    }

}
