using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IChara : MonoBehaviour
{
    public int hp;
    public int cur_hp;
    public int mp;
    public int cur_mp;

    public virtual void ReceiveDamage(int dmg)
    {
        if (cur_hp > 0)
        {
            cur_hp -= dmg;
            //Debug.Log(gameObject.name + " ReceiveDamage : " + dmg);
        }          
    }
}
