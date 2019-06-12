using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePotion : Iitem
{
    public override void Init()
    {
        base.Init();
        item_name = "LifePotion";
    }
    public override void Use()
    {
        if(owner.cur_hp< owner.hp)
            owner.cur_hp += 25;
        if (owner.cur_hp > owner.hp)
            owner.cur_hp = owner.hp;
        particle.Play();
        meshRenderer.enabled = false;
        hitbox.enabled = false;
        Destroy(gameObject, 2f);
    }
}
