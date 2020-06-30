using System.Collections;
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
    [SerializeField] private Button m_Button = null;
    [SerializeField] private PlantLibraryManager m_plantLibraryManager = null;
    [SerializeField] private Image m_BlurImage = null;
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
                        m_PlantImage.color = new Color(0.3f, 0.3f, 0.3f, 1.0f);
                        m_NewMark.enabled = false;
                        m_Button.interactable = false;
                        m_PlantName.text = "???";
                        m_BlurImage.gameObject.SetActive(true);
                        break;
                    case LibraryState.DISCOVERED:
                        m_PlantImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                        m_NewMark.enabled = true;
                        m_Button.interactable = true;
                        m_PlantName.text = m_plantNameString;
                        m_BlurImage.gameObject.SetActive(false);
                        break;
                    case LibraryState.IDENTIFIED:
                        m_PlantImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                        m_NewMark.enabled = false;
                        m_Button.interactable = true;
                        m_PlantName.text = m_plantNameString;
                        m_BlurImage.gameObject.SetActive(false);
                        break;
                }
            }
        }
    }

    public string m_PlantNameString { set => m_plantNameString = value; }
    public int m_SpeciesId { set => m_speciesId = value; }

    public void SetSprites(Sprite newSprite)
    {
        m_PlantImage.sprite = newSprite;
        m_PlantImage.SetNativeSize();
        m_BlurImage.rectTransform.sizeDelta = m_PlantImage.rectTransform.sizeDelta;
    }

    public void SetStateByInt(int newState)
    {
        m_State = (LibraryState)newState;
    }

    public void OnClicked()
    {
        SetStateByInt(2);
        m_plantLibraryManager.ShowPlantInfo(m_speciesId);
    }
}
