using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    private int m_plantSpeciesId;
    private int m_plantObjId;

    public int m_PlantSpeciesId { get => m_plantSpeciesId; set => m_plantSpeciesId = value; }
    public int m_PlantObjId { get => m_plantObjId; set => m_plantObjId = value; }


    [SerializeField]
    private GameManager m_Gm = null;

    public void OnTouched()
    {
        m_Gm.GainPlant(m_PlantObjId);
    }
}
