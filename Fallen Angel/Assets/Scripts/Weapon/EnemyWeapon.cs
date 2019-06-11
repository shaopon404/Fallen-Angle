using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : Weapon
{
    public bool isAttacking = false;

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(isAttacking)
                FindObjectOfType<Player>().ReceiveDamage(damage);
        }
    }
}
