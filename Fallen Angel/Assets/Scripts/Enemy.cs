using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using HTC.UnityPlugin.Vive;

public enum State
{
    None,
    Search,
    Attack,
}
public class Enemy : IChara
{
    public State state;
    private NavMeshAgent meshAgent;
    private Transform player;
    public float _radius;
    public float distance;
    public float angle;
    private Animator animator;
    public bool weak = false;
    private Collider[] _hitColliders = new Collider[0];
    private Vector3 offset = new Vector3(0, 2f, 0);
    private LayerMask _playerMask;
    private EnemyWeapon[] weapons;

    public List<Action> shortAttack = new List<Action>();
    public List<Action> middleAttack = new List<Action>();
    public List<Action> longAttack = new List<Action>();

    public Queue<Action> actionQueue = new Queue<Action>();
    public Action curAction;
    public bool has_choice = false;
    public Transform hp_bar;

    void Start()
    {
        meshAgent = GetComponent<NavMeshAgent>();
        meshAgent.destination = transform.position;
        player = FindObjectOfType<VRCameraHook>().transform;
        animator = GetComponent<Animator>();
        _playerMask = LayerMask.GetMask("Chara");
        weapons = FindObjectsOfType<EnemyWeapon>();
        state = State.Search;
        shortAttack.Add(Slash);
        middleAttack.Add(DoubleSlash);
        middleAttack.Add(HeavySlash);
        longAttack.Add(Dash);
    }

    void Update()
    {
            distance = GetDistance();
            angle = GetAngle();

            if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Stand") || animator.GetCurrentAnimatorStateInfo(0).IsTag("Walk"))
            {
                switch (state)
                {
                    case State.Search:
                        if (curAction == null)
                        {
                            meshAgent.enabled = true;
                            float d = Vector3.Distance(meshAgent.transform.position, player.position);
                            if (!has_choice || angle > 60)
                            {
                                animator.SetBool("walk", true);
                                Vector3 destination = Vector3.Lerp(transform.position, player.position, 0.6f);
                                meshAgent.SetDestination(new Vector3(destination.x, 0, destination.z));
                                ChooseAttack(distance);
                            }
                            else
                            {
                                meshAgent.destination = transform.position;
                                animator.SetBool("walk", false);
                                state = State.Attack;
                            }
                        }
                        break;
                    case State.Attack:
                        meshAgent.enabled = false;
                        if (actionQueue.Count > 0)
                        {
                            animator.SetBool("attack", true);
                            if (curAction == null)
                            {
                                meshAgent.enabled = false;
                                curAction = actionQueue.Dequeue();
                                curAction.Invoke();
                            }
                        }
                        else
                        {
                            has_choice = false;
                            state = State.Search;
                        }
                        break;
                }
            }
    }

    private void FixedUpdate()
    {
        _hitColliders = Physics.OverlapSphere(transform.position + offset, _radius, _playerMask);
    }

    public override void ReceiveDamage(int dmg)
    {
        base.ReceiveDamage(dmg);
        if (weak)
            animator.SetTrigger("damage");
    }

    private float GetDistance()
    {
        float d = Mathf.Infinity;
        for (int i = 0; i < _hitColliders.Length; i++)
        {
            if (_hitColliders[i].gameObject.CompareTag("Player"))
            {
                d = Vector3.Distance(gameObject.transform.position, _hitColliders[i].gameObject.transform.position);
                break;
            }
        }
        //Debug.Log(distance);
        return d;
    }

    public float GetAngle()
    {
        float a = Mathf.Infinity;
        for (int i = 0; i < _hitColliders.Length; i++)
        {
            if (_hitColliders[i].gameObject.CompareTag("Player"))
            {
                Vector3 playerPos = new Vector3(_hitColliders[i].transform.position.x, 0, _hitColliders[i].transform.position.z);
                Vector3 myPos = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 dirToTarget = (playerPos - myPos);
                //Debug.Log(transform.rotation.eulerAngles + " / "+ dirToTarget);
                a = Vector3.Angle(transform.forward, dirToTarget);
                break;
            }
        }
        //Debug.Log(angle);
        return a;
    }

    private void ChooseAttack(float distace)
    {
        //Debug.Log("ChooseAttack");
        has_choice = false;

        if (angle > 60)
        {
            actionQueue.Clear();
            state = State.Search;
        }
        else
        {
            if (distace > 6f)
            {
                int dash = UnityEngine.Random.Range(1, 100);
                Debug.Log(dash);
                if (dash > 30)
                    if (longAttack.Count > 0)
                    {
                        int ranNum = UnityEngine.Random.Range(0, longAttack.Count);
                        actionQueue.Enqueue(longAttack[ranNum]);
                        Debug.Log("Enqueue " + longAttack[ranNum].Method.Name);
                        has_choice = true;
                    }
            }

            int ranTime = UnityEngine.Random.Range(1, 3);
            for (int i = 0; i < ranTime; i++)
            {
                if (distace > 2 && distace <= 3)
                {
                    if (middleAttack.Count > 0)
                    {
                        int ranNum = UnityEngine.Random.Range(0, middleAttack.Count);
                        actionQueue.Enqueue(middleAttack[ranNum]);
                        Debug.Log("Enqueue " + middleAttack[ranNum].Method.Name);
                        has_choice = true;
                    }
                }
                else if (distace <= 2f)
                {
                    if (shortAttack.Count > 0)
                    {
                        int ranNum = UnityEngine.Random.Range(0, shortAttack.Count);
                        actionQueue.Enqueue(shortAttack[ranNum]);
                        Debug.Log("Enqueue " + shortAttack[ranNum].Method.Name);
                        has_choice = true;
                    }
                }

            }
        }
        if (!has_choice)
        {
            state = State.Search;
        }
    }

    #region short attack
    private void Slash()
    {
        //Debug.Log("slash");
        animator.SetTrigger("slash");
    }
    #endregion

    #region middle attack
    private void DoubleSlash()
    {
        //Debug.Log("double_slash");
        animator.SetTrigger("double_slash");
    }

    private void HeavySlash()
    {
        //Debug.Log("double_slash");
        animator.SetTrigger("heavy_slash");
    }
    #endregion

    #region long attack
    private void Dash()
    {
        Debug.Log("Dash");
        animator.SetTrigger("dash");
    }
    #endregion

    #region animation event
    public void ClearAction()
    {
        curAction = null;
    }

    public void AttackStart()
    {
        foreach (var weapon in weapons)
        {
            weapon.isAttacking = true;
        }
    }

    public void AttackEnd()
    {
        foreach (var weapon in weapons)
        {
            weapon.isAttacking = false;
        }
    }

    public void WeakStart()
    {
        weak = true;
    }

    public void WeakEnd()
    {
        weak = false;
    }
    #endregion

    public void Dead()
    {
        animator.SetBool("dead", true);
        hp_bar.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + offset, _radius);
    }
}