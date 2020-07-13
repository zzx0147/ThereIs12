using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBGMVolume : MonoBehaviour
{
    [SerializeField] private AudioSource m_AudioSource = null;
    void Start()
    {
        m_AudioSource.volume = DataManager.GetBGMVolume();
    }
}
