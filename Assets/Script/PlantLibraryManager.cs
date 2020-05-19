using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantLibraryManager : MonoBehaviour
{
    public LibraryPlantCell[] m_LibraryPlantCell = null;
    public Sprite[] m_PlantSprites = null;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_PlantSprites.Length; ++i)
        {
            m_LibraryPlantCell[i].SetSprites(m_PlantSprites[i]);
            m_LibraryPlantCell[i].m_PlantNameString = "Test";
            m_LibraryPlantCell[i].m_SpeciesId = i;
            m_LibraryPlantCell[i].m_State = DataManager.GetPlantLibraryState(i);
        }
    }

    public void UpdatePlantLibrary(int SpeciesId)
    {
        if(m_LibraryPlantCell[SpeciesId].m_State == LibraryState.UNKNOWN)
        {
            m_LibraryPlantCell[SpeciesId].m_State = LibraryState.DISCOVERED;
        }
    }



}
