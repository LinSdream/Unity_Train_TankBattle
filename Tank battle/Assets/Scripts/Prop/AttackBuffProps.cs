using LS.Helper.Prop;
using LS.Helper.Timer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyWork
{
    public class AttackBuffProps : BaseProp
    {
        public override void Excute(GameObject obj)
        {
            var script = obj.GetComponent<TankBuffStatus>();

            Buff buff = new Buff(Info);
            buff.BuffName = Info.PropName;
            buff.BeforeStatus = obj.GetComponent<TankShooting>().Damage;
            buff.ReturnAction = (game, buf) =>
            {
                game.GetComponent<TankShooting>().Damage = (float)buf.BeforeStatus;
            };


            script.AddBuff(buff,(game)=>
            {
                game.GetComponent<TankShooting>().Damage += Info.Value;
            });
            
        }
    }

}