using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSShower : MonoBehaviour
{
    [SerializeField] Text m_text;

    private void Start()
    {
        StartCoroutine(UpdateFPS());
    }

    IEnumerator UpdateFPS()
    {
        while(true)
        {
            m_text.text = ((int)(1 / Time.unscaledDeltaTime)).ToString();
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
    
}
