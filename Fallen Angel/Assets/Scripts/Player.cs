using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;
using DG.Tweening;
using UnityEngine.UI;

public class Player : IChara
{
    public Transform head;
    public PlayerWeapon eq_weapon;
    public List<PlayerWeapon> weaponList = new List<PlayerWeapon>();
    public bool teleporting = false;
    public bool show_weapon = false;
    public float warp_speed;
    public float dodge_speed;
    private float startTime;
    public Vector3 startPos, warpPos = Vector3.zero;
    private Vector3 offset;
    public float traveledDistance;

    public ParticleSystem handParticle;
    public ParticleSystem[] body_trials;

    public Transform floating_platform;
    public bool show_floating_platform;
    private float t = 0;
    private float recover_t = 0;
    public Queue<int> attackQueue = new Queue<int>();

    public Transform status_panel;
    public bool show_status_panel = false;

    public Image HP_Bar;
    public Image MP_Bar;

    void Start()
    {
        cur_hp = hp;
        cur_mp = mp;
        head = FindObjectOfType<VRCameraHook>().transform;
        ViveInput.AddPressUp(HandRole.RightHand, ControllerButton.Grip, EquiptWeapon);
        ViveInput.AddPressDown(HandRole.RightHand, ControllerButton.Trigger, ShiftBreak);
        ViveInput.AddPressUp(HandRole.LeftHand, ControllerButton.Grip, ShowStatusPanel);
        ViveInput.AddPressDown(HandRole.LeftHand, ControllerButton.Pad, ShiftDodge);
        floating_platform.localScale = Vector3.zero;
        status_panel.transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        Recover();

        if (!teleporting)
        {
            DequeueAttackCommand();

            traveledDistance = 0;

            WeaponTracking();
        }
        else
        {
            if (eq_weapon.isHit || eq_weapon.isBlock)
            {
                CancelInvoke();
                Warp();
            }
            if (warpPos != Vector3.zero)
                if (Vector3.Distance(transform.position, warpPos) < 0.01f)
                {
                    eq_weapon.StopTrails();
                    warpPos = Vector3.zero;
                    teleporting = false;
                }
        }

        Float();

        HP_Bar.fillAmount = ((float)cur_hp / hp);
        MP_Bar.fillAmount = ((float)cur_mp / mp);
    }

    private void Recover()
    {
        if (Time.time - recover_t > 1f)
        {
            if (cur_hp < hp)
                cur_hp += 1;

            if (cur_mp < mp)
                cur_mp += 1;

            recover_t = Time.time;
        }
    }

    private void WeaponTracking()
    {
        if (show_weapon)
        {
            if (eq_weapon != null)
            {
                eq_weapon.gameObject.transform.position = transform.TransformPoint(VivePose.GetPose(HandRole.RightHand).pos);
                eq_weapon.gameObject.transform.rotation = VivePose.GetPose(HandRole.RightHand).rot;
                eq_weapon.rigid.freezeRotation = false;
                eq_weapon.rigid.isKinematic = true;
                eq_weapon.isHit = false;
                eq_weapon.isBlock = false;
            }
        }
    }

    private void Float()
    {
        if (show_floating_platform)
        {
            if (Time.time - t > 0.5f)
            {
                cur_mp -= 1;
                t = Time.time;
            }
        }
    }

    private void DequeueAttackCommand()
    {
        if (attackQueue.Count > 0)
        {
            int dmg = attackQueue.Dequeue();
            FindObjectOfType<Enemy>().ReceiveDamage(Mathf.RoundToInt(dmg + dmg * traveledDistance / 5));
            eq_weapon.hit_particle.Play();
            ViveInput.TriggerHapticPulse(HandRole.RightHand, 1000);
        }
    }

    public void CollectWeapon(PlayerWeapon weapon)
    {
        if (!weaponList.Contains(weapon))
        {
            weaponList.Add(weapon);
            weapon.Init();
            weapon.gameObject.SetActive(false);
        }
    }

    private void EquiptWeapon()
    {
        if (!teleporting)
        {
            if (weaponList.Count > 0)
                show_weapon = !show_weapon;

            if (show_weapon)
            {
                eq_weapon = weaponList[0];
                eq_weapon.gameObject.SetActive(true);
            }
            handParticle.Play();

            if (!show_weapon)
            {
                if (eq_weapon != null)
                {
                    eq_weapon.gameObject.SetActive(false);
                    eq_weapon = null;
                }
            }
        }
    }

    public void ShiftBreak()
    {
        if (show_weapon)
            if (!eq_weapon.isBlock)
            {
                teleporting = true;
                startTime = Time.time;
                startPos = eq_weapon.transform.position;
                eq_weapon.rigid.isKinematic = false;
                eq_weapon.rigid.freezeRotation = true;
                eq_weapon.rigid.velocity = eq_weapon.transform.forward * warp_speed;
                eq_weapon.PlayTrails();
                CancelInvoke();
                Invoke("Warp", 0.5f);
            }
    }

    private void Warp()
    {
        if (show_weapon)
        {
            eq_weapon.rigid.isKinematic = true;

            if (eq_weapon.transform.position.y >= 1)
            {
                offset = new Vector3(0, head.position.y - transform.position.y, 0);
                warpPos = eq_weapon.transform.position + offset;
            }
            else
                warpPos = new Vector3(eq_weapon.transform.position.x, 0, eq_weapon.transform.position.z);

            traveledDistance = Vector3.Distance(startPos, warpPos);
            float t = traveledDistance / warp_speed;
            transform.DOMove(warpPos, t);
        }

        if (warpPos.y > 0)
        {
            show_floating_platform = true;
            floating_platform.gameObject.SetActive(true);
            floating_platform.DOScale(Vector3.one, 1f);
        }
        else
        {
            show_floating_platform = false;
            floating_platform.gameObject.SetActive(false);
            floating_platform.localScale = Vector3.zero;
        }
    }

    public void ShiftDodge()
    {
        if (!teleporting)
        {
            Vector2 padDir = ViveInput.GetPadTouchAxis(HandRole.LeftHand);
            Vector2 warpDir = (head.right * padDir.x + head.forward * padDir.y) * dodge_speed;
            Vector3 newPos = new Vector3(transform.position.x + warpDir.x, transform.position.y, transform.position.z + warpDir.y);

            Debug.Log(padDir);
            transform.DOMove(newPos, 0.2f);
            PlayTrails();
            CancelInvoke();
            Invoke("StopTrails", 0.2f);
        }
    }

    private void PlayTrails()
    {
        if (body_trials.Length > 0)
        {
            foreach (var trail in body_trials)
            {
                trail.Play();
            }
        }
    }

    private void StopTrails()
    {
        if (body_trials.Length > 0)
        {
            foreach (var trail in body_trials)
            {
                trail.Stop();
            }
        }
    }

    private void ShowStatusPanel()
    {
        show_status_panel = !show_status_panel;

        if (show_status_panel)
        {
            status_panel.DOScale(Vector3.one, 0.3f);
        }
        else
        {
            status_panel.DOScale(Vector3.zero, 0.3f);
        }

    }
}
