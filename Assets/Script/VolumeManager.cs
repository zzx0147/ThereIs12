using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [SerializeField] private Slider m_BGMSlider;
    [SerializeField] private Slider m_SFXSlider;
    [SerializeField] private AudioSource[] m_BGM_AudioSources;
    [SerializeField] private AudioSource[] m_SFX_AudioSources;

    void Start()
    {
        m_BGMSlider.value = DataManager.GetBGMVolume();
        m_SFXSlider.value = DataManager.GetSFXVolume();
    }

    public void BGMSet(float value)
    {
        Debug.Log("BGM: " + value);
        DataManager.SetBGMVolume(value);
        
        foreach(var v in m_BGM_AudioSources)
        {
            v.volume = value;
        }

    }

    public void SFXSet(float value)
    {
        Debug.Log("SFX: " + value);
        DataManager.SetSFXVolume(value);
        foreach (var v in m_SFX_AudioSources)
        {
            v.volume = value;
        }
    }
}
