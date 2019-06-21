using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerMsgPanel : MonoBehaviour
{
    public Text Msg;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.zero;
        Msg.text = "";
    }

    public void ShowMessege(string text)
    {
        Msg.text = text;
        transform.DOScale(new Vector3(0.005f, 0.005f, 0.005f), 0.3f);
        Invoke("Release", 3f);
    }

    private void Release()
    {
        transform.DOScale(Vector3.zero, 0.3f);
        Msg.text = "";
    }
}
