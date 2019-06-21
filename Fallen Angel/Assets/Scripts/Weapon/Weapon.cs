using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;

public abstract class Weapon : MonoBehaviour
{
    private Collider hitbox;
    public int damage;
    // Start is called before the first frame update
    public void Init()
    {
        hitbox = GetComponent<Collider>();
        hitbox.isTrigger = true;
    }
    protected abstract void OnTriggerEnter(Collider other);
}
