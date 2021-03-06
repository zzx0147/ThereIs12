﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LibraryPlantCell : MonoBehaviour
{
    private LibraryState m_state = LibraryState.UNDEFINED;
    private string m_plantNameString = null;
    private int m_speciesId = -1;

    [SerializeField] private Image m_PlantImage = null;
    [SerializeField] private Image m_NewMark = null;
    [SerializeField] private Text m_PlantName = null;
    [SerializeField] private PlantLibraryManager m_plantLibraryManager = null;
    [SerializeField] private Image m_BluredPlantImage = null;

    public LibraryState m_State
    {
        get => m_state;
        set
        {
            if (m_state != value)
            {
                m_state = value;
                DataManager.SetPlantLibraryState(m_speciesId, m_state);
                //Debug.Log(m_speciesId + ": " + m_state);
                switch (m_state)
                {
                    case LibraryState.UNKNOWN:
                        //m_PlantImage.color = new Color(32.0f / 255.0f, 32.0f / 255.0f, 32.0f / 255.0f, 1.0f);
                        m_NewMark.enabled = false;
                        m_PlantName.text = m_speciesId + 1 + "번";
                        m_BluredPlantImage.gameObject.SetActive(true);
                        break;
                    case LibraryState.DISCOVERED:
                        //m_PlantImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                        m_NewMark.enabled = true;
                        m_PlantName.text = m_plantNameString;
                        m_BluredPlantImage.gameObject.SetActive(false);
                        break;
                    case LibraryState.IDENTIFIED:
                        //m_PlantImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                        m_NewMark.enabled = false;
                        m_PlantName.text = m_plantNameString;
                        m_BluredPlantImage.gameObject.SetActive(false);
                        break;
                }
            }
        }
    }

    public string m_PlantNameString { set => m_plantNameString = value; }
    public int m_SpeciesId { set => m_speciesId = value; }

    public void SetSprites(Sprite newSprite,Sprite bluredSprite)
    {
        m_PlantImage.sprite = newSprite;
        m_PlantImage.SetNativeSize();
        m_BluredPlantImage.sprite = bluredSprite;
        m_BluredPlantImage.SetNativeSize();
    }

    public void OnClicked()
    {
        Debug.Log("OnClick");
        Debug.Log(m_State);
        switch(m_State)
        {
            case LibraryState.DISCOVERED:
                m_State = LibraryState.IDENTIFIED;
                m_plantLibraryManager.ShowPlantInfo(m_speciesId);
                break;
            case LibraryState.IDENTIFIED:
                m_plantLibraryManager.ShowPlantInfo(m_speciesId);
                break;
            case LibraryState.UNKNOWN:
                m_plantLibraryManager.ShowUnkownPlantInfo(m_speciesId);
                break;
        }
    }
}
