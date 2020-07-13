using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetContentsPosition : MonoBehaviour
{
    [SerializeField] private RectTransform m_Content;

    private void OnEnable()
    {
        m_Content.anchoredPosition = new Vector2(m_Content.anchoredPosition.x,0);
    }
}
