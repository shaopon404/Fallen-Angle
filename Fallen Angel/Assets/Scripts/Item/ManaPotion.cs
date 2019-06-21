using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPotion : Iitem
{
    public override void Init()
    {
        base.Init();
        item_name = "ManaPotion";
    }
    public override void Use()
    {
        if (owner.cur_mp < owner.mp)
            owner.cur_mp += 50;
        if (owner.cur_mp > owner.mp)
            owner.cur_mp = owner.mp;
        particle.Play();
        meshRenderer.enabled = false;
        hitbox.enabled = false;
        Destroy(gameObject, 2f);
    }
}
