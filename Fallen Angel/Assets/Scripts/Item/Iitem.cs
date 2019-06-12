using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;

public abstract class Iitem : MonoBehaviour
{
    public string item_name;
    public Collider hitbox;
    public Player owner;
    public ParticleSystem particle;
    public MeshRenderer meshRenderer;
    // Start is called before the first frame update

    private void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        hitbox = GetComponent<Collider>();
        hitbox.isTrigger = true;
        particle = GetComponentInChildren<ParticleSystem>();
        particle.playOnAwake = false;
        particle.loop = false;
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponentInParent<ViveColliderEventCaster>())
        {
            if (ViveInput.GetPress(HandRole.RightHand, ControllerButton.Grip) || ViveInput.GetPress(HandRole.LeftHand, ControllerButton.Grip))
            {
                owner = FindObjectOfType<Player>();
                owner.CollectItem(this);
            }
        }
    }

    public bool HasOwner()
    {
        if (owner != null)
            return true;
        else
            return false;
    }

    public abstract void Use();
}
