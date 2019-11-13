using LS.Helper.Prop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyWork.Network.Tanks;

namespace MyWork
{
    public class BloodReturnProps : BaseProp
    {
        public override void Excute(GameObject obj)
        {
            obj.GetComponent<TankHealth>().ChangeHealth(Info.Value);
        }
    }

}