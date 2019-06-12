using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;

public class AutoUIScaler : MonoBehaviour
{
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<VRCameraHook>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
    }
}
