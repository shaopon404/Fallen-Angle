using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    private Collider m_collider;
    public bool is_cooling = false;
    public float mutiply = 1;

    private void Start()
    {
        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = false;
    }

    public void CoolDown()
    {
        if(!is_cooling)
            StartCoroutine(Rest());
    }

    private IEnumerator Rest()
    {
        m_collider.enabled = false;
        yield return new WaitForSeconds(0.5f);
        m_collider.enabled = true;
        StopAllCoroutines();
    }
}
