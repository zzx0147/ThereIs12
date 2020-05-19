using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionCopyer : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_TargetPanel;

    private RectTransform m_RectTransform;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        m_RectTransform.position = m_TargetPanel.position;
    }
}
