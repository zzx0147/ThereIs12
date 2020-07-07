using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionCopyer : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_TargetPanel = null;

    private RectTransform m_RectTransform;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        m_RectTransform.position = m_TargetPanel.position;
    }
}
