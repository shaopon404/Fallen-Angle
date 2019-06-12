using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoenixFeather : Iitem
{
    public override void Init()
    {
        base.Init();
        item_name = "PhoenixFeather";
    }
    public override void Use()
    {
        owner.cur_hp = owner.hp;
        particle.Play();
        meshRenderer.enabled = false;
        hitbox.enabled = false;
        Destroy(gameObject, 2f);
    }
}
