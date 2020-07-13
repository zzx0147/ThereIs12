using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryListManager : MonoBehaviour
{
    [SerializeField] Button[] m_Buttons;
    [SerializeField] Image[] m_Images;

    private void OnEnable()
    {
        for(int i = 0; i < 6; ++i)
        {
            if(DataManager.GetMission(i))
            {
                m_Buttons[i].interactable = true;
                m_Images[i].enabled = true;
            }
        }
    }
}
