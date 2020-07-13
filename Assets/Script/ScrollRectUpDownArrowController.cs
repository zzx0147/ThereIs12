using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectUpDownArrowController : MonoBehaviour
{
    [SerializeField] private Image m_UpArrow = null;
    [SerializeField] private Image m_DownArrow = null;

    public void OnValueChange(Vector2 vec)
    {
        //Debug.Log(vec);
        if (vec.y >= 0.99f)
        {
            m_UpArrow.enabled = false;
            m_DownArrow.enabled = true;
        }
        else if(vec.y <= 0.00f)
        {
            m_UpArrow.enabled = true;
            m_DownArrow.enabled = false;
        }
        else
        {
            m_UpArrow.enabled = true;
            m_DownArrow.enabled = true;
        }
    }
}
