using System;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    Queue<Action> actionQueue = new Queue<Action>();
    List<Action> actionList = new List<Action>();
    // Start is called before the first frame update
    void Start()
    {
        actionList.Add(TestAction);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            actionQueue.Enqueue(actionList[0]);
        if (Input.GetKeyDown(KeyCode.W))
            actionQueue.Dequeue().Invoke();
    }

    private void AddAction()
    {

    }

    private void TestAction()
    {
        Debug.Log("Test");
    }
}
