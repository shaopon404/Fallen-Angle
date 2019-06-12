using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HTC.UnityPlugin.Vive;
using DG.Tweening;

public class ItemPanel
{
    public Text text;
    public Image image;
    public int num;
}
public class PlayerUIPanel : MonoBehaviour
{
    public Transform[] panels = new Transform[3];
    private string[] pn = { "LifePotion", "ManaPotion", "PhoenixFeather" };
    public List<ItemPanel> itemPanels = new List<ItemPanel>();
    public Dictionary<string, ItemPanel> itemPanelDict = new Dictionary<string, ItemPanel>();
    public int curFocus = 0;
    // Start is called before the first frame update
    void Start()
    {
        panels[curFocus].DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.3f);
        int i = 0;
        foreach (var panel in panels)
        {
            ItemPanel itemPanel = new ItemPanel
            {
                text = panel.GetComponentInChildren<Text>(),
                image = panel.GetComponentInChildren<Image>(),
                num = 0,
            };
            itemPanel.text.text = "0";
            itemPanels.Add(itemPanel);
            itemPanelDict.Add(pn[i], itemPanel);
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ViveInput.GetPressDown(HandRole.LeftHand,ControllerButton.Pad))
        {
            Vector2 dir = ViveInput.GetPadAxis(HandRole.LeftHand);
            if (dir.y > 0)
                curFocus++;
            else
                curFocus--;

            if (curFocus == panels.Length)
                curFocus = 0;
            if (curFocus < 0)
                curFocus = 2;

            for (int i = 0; i < panels.Length; i++)
            {
                if (i == curFocus)
                {
                    panels[i].DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.3f);
                    itemPanels[i].image.color = new Color(0, 255, 255, 136);
                }
                else
                {
                    panels[i].DOScale(Vector3.one, 0.3f);
                    itemPanels[i].image.color = new Color(255, 255, 255, 136);
                }
            }
        }
    }

    public void AddItem(string name)
    {
        if (itemPanelDict.ContainsKey(name))
        {
            itemPanelDict[name].num++;
            itemPanelDict[name].text.text = itemPanelDict[name].num.ToString();
        }
            
    }

    public void ConsumeItem(string name)
    {
        if (itemPanelDict.ContainsKey(name))
        {
            itemPanelDict[name].num--;
            itemPanelDict[name].text.text = itemPanelDict[name].num.ToString();
        }
    }

    public string GetFocousPanelName()
    {
        return panels[curFocus].tag;
    }
}
