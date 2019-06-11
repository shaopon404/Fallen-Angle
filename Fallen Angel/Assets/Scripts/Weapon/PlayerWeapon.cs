using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;

public class PlayerWeapon : Weapon
{
    public Player owner;
    public Rigidbody rigid;
    public bool isHit;
    public bool isBlock;
    public Vector3 hitSpot;
    public ParticleSystem hit_particle;
    public ParticleSystem[] trail_particles;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            DamageHandler handler = other.GetComponent<DamageHandler>();
            if (!handler.is_cooling)
            {
                isHit = true;
                owner.attackQueue.Enqueue(Mathf.RoundToInt(damage * handler.mutiply));
                handler.CoolDown();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            isBlock = true;
        }
        else
        {
            isBlock = false;
        }

        if (other.gameObject.GetComponentInParent<ViveColliderEventCaster>())
        {
            if (ViveInput.GetPress(HandRole.RightHand, ControllerButton.Grip) || ViveInput.GetPress(HandRole.LeftHand, ControllerButton.Grip))
            {
                owner = FindObjectOfType<Player>();
                owner.CollectWeapon(this);
            }
        }
    }

    public void PlayTrails()
    {
        if (trail_particles.Length > 0)
        {
            foreach (var trail in trail_particles)
            {
                trail.Play();
            }
        }
    }

    public void StopTrails()
    {
        if (trail_particles.Length > 0)
        {
            foreach (var trail in trail_particles)
            {
                trail.Stop();
            }
        }
    }
}
