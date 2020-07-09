using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinderSwitch : MonoBehaviour
{
    [SerializeField] GameObject m_Target;

    private void OnEnable()
    {
        if (m_Target != null)
        {
            m_Target.SetActive(true);
            m_Target.GetComponent<Button>().onClick.AddListener(() => gameObject.SetActive(false));
        }
    }

    private void OnDisable()
    {
        if (m_Target != null)
        {
            m_Target.SetActive(false);
            m_Target.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }
}
