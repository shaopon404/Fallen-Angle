using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItem : MonoBehaviour
{
    public Iitem[] items;
    public Iitem curItem;
    public float startTime;
    public float repeatTime = 30;
    private float nextTime;
    public bool first_spawn=false;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Random.Range(10, 60);
        //startTime = 10;
        Invoke("FirstSpawn", startTime);
    }

    // Update is called once per frame
    void Update()
    {
        if(first_spawn)
        {
            CancelInvoke();
            if (curItem == null)
            {
                if (Time.time > nextTime)
                {
                    curItem = Instantiate(items[Random.Range(0, items.Length)], transform);
                    curItem.transform.LookAt(Vector3.forward);
                }
            }
            else
            {
                if (curItem.HasOwner())
                {
                    curItem = null;
                    nextTime = Time.time + repeatTime;
                }
            }
        }
    }

    private void FirstSpawn()
    {
        if (curItem == null)
        {
            curItem = Instantiate(items[Random.Range(0, items.Length)], transform);
            curItem.transform.LookAt(Vector3.forward);
        }
        nextTime = Time.time + repeatTime;
        first_spawn = true;
    }
}
