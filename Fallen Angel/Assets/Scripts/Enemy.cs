using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using HTC.UnityPlugin.Vive;
public class Enemy : IChara
{
    private NavMeshAgent meshAgent;
    private Transform player;
    public float _radius;
    public float distance;
    private Animator animator;
    public bool searching = false;
    public bool in_range = false;
    public bool weak = false;
    public Collider[] _hitColliders = new Collider[0];
    public float rotSpeed;
    private Vector3 offset = new Vector3(0, 2f, 0);
    protected LayerMask _playerMask;
    public EnemyWeapon[] weapons;

    public List<Action> shortAttack = new List<Action>();
    public List<Action> middleAttack = new List<Action>();
    public List<Action> longAttack = new List<Action>();

    public Queue<Action> actionQueue = new Queue<Action>();
    public Action curAction;
    public bool is_attacking;

    void Start()
    {
        meshAgent = GetComponent<NavMeshAgent>();
        meshAgent.destination = transform.position;
        player = FindObjectOfType<VRCameraHook>().transform;
        animator = GetComponent<Animator>();
        _playerMask = LayerMask.GetMask("Chara");
        weapons = FindObjectsOfType<EnemyWeapon>();

        shortAttack.Add(Slash);
        shortAttack.Add(DoubleSlash);
        shortAttack.Add(HeavySlash);
        //shortAttack.Add(Wings);
        //middleAttack.Add();
        //longAttack.Add(Dash);

        StartCoroutine(StartSearching());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Debug.Log(actionQueue.Count);
        if (searching)
        {
            float distace = GetDistance();
            if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Stand") || animator.GetCurrentAnimatorStateInfo(0).IsTag("Walk"))
            {
                if (actionQueue.Count > 0)
                {
                    animator.SetBool("attack", true);
                    if (curAction == null)
                    {
                        curAction = actionQueue.Dequeue();
                        curAction.Invoke();
                        return;
                    }
                }
                else
                {
                    animator.SetBool("attack", false);
                    ChooseAttack(distace);
                }
                meshAgent.enabled = true;
                float d = Vector3.Distance(meshAgent.transform.position, player.position);
                if (d >= 2f)
                {
                    animator.SetBool("walk", true);
                    Vector3 destination = Vector3.Lerp(transform.position, player.position, 0.6f);
                    meshAgent.SetDestination(new Vector3(destination.x, 0, destination.z));
                }
                else
                {
                    meshAgent.destination = transform.position;
                    animator.SetBool("walk", false);
                }
            }
            else
            {
                meshAgent.enabled = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (searching)
        {
            _hitColliders = Physics.OverlapSphere(transform.position + offset, _radius, _playerMask);
        }
    }

    public override void ReceiveDamage(int dmg)
    {
        base.ReceiveDamage(dmg);
        if (weak)
            animator.SetTrigger("damage");
    }

    private float GetDistance()
    {
        float distance = Mathf.Infinity;
        for (int i = 0; i < _hitColliders.Length; i++)
        {
            if (_hitColliders[i].gameObject.CompareTag("Player"))
            {
                distance = Vector3.Distance(gameObject.transform.position, _hitColliders[i].gameObject.transform.position);
                in_range = true;
                break;
            }
            else
            {
                in_range = false;
            }
        }
        //Debug.Log(distance);
        return distance;
    }

    public float GetAngle()
    {
        float angle = Mathf.Infinity;
        for (int i = 0; i < _hitColliders.Length; i++)
        {
            if (_hitColliders[i].gameObject.CompareTag("Player"))
            {
                Vector3 playerPos = new Vector3(_hitColliders[i].transform.position.x, 0, _hitColliders[i].transform.position.z);
                Vector3 myPos = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 dirToTarget = (playerPos - myPos);
                angle = Vector3.Angle(transform.rotation.eulerAngles, dirToTarget);
                break;
            }
        }
        //Debug.Log(angle);
        return angle;
    }

    private void ChooseAttack(float distace)
    {
        float angle = GetAngle();
        if (angle < 0 || angle > 180)
        {
            if (angle < 0)
                transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime);
            if (angle > 180)
                transform.Rotate(Vector3.down * rotSpeed * Time.deltaTime);
        }
        else if (distace < 4f)
        {
            int ranTime = UnityEngine.Random.Range(1, 3);
            for (int i = 0; i < ranTime; i++)
            {
                int ranNum = UnityEngine.Random.Range(0, shortAttack.Count);
                actionQueue.Enqueue(shortAttack[ranNum]);
                Debug.Log("Enqueue " + shortAttack[ranNum].Method.Name);
            }
        }
    }

    #region short attack
    private void Slash()
    {
        Debug.Log("slash");
        animator.SetTrigger("slash");
    }

    private void DoubleSlash()
    {
        Debug.Log("double_slash");
        animator.SetTrigger("double_slash");
    }

    private void Wings()
    {
        animator.SetTrigger("wings");
    }
    #endregion

    #region middle attack
    private void HeavySlash()
    {
        animator.SetTrigger("heavy_slash");
    }
    #endregion

    #region long attack
    private void Dash()
    {
        animator.SetTrigger("dash");
    }

    private void Jump()
    {
        animator.SetTrigger("jump");
    }

    private void Bolt()
    {
        animator.SetTrigger("bolt");
    }

    private void Laser()
    {
        animator.SetTrigger("laser");
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

    private IEnumerator StartSearching()
    {
        yield return new WaitForSeconds(3.2f);
        searching = true;
        foreach (var weapon in weapons)
        {
            weapon.Init();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + offset, _radius);
    }
}
