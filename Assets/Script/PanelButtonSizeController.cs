using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelButtonSizeController : MonoBehaviour
{
    public void SetSize(bool isOn)
    {
        if(isOn)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(0,62);
        }
        else
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        }
    }
}
