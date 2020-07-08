using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantLibraryManager : MonoBehaviour
{
    public LibraryPlantCell[] m_LibraryPlantCell = null;
    private Sprite[] m_PlantSprites = new Sprite[51];
    private Sprite[] m_BluredPlantSprites = new Sprite[51];
    private string[,] m_PlantCsv = null;

    [SerializeField]
    private GameObject m_InfoPanel = null;
    [SerializeField]
    private Text m_PlantNameText = null;
    [SerializeField]
    private Text m_PlantIdText = null;
    [SerializeField]
    private Text m_PlantPointText = null;
    [SerializeField]
    private Text m_PlantDialogueText = null;
    [SerializeField]
    private Text m_PlantExplainText = null;
    [SerializeField]
    private Image m_PlantInfoImage = null;

    private void Awake()
    {
        m_PlantCsv = CsvLoader.LoadCsvBy2DimensionArray("Csv/PlantMasterTable");

    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_PlantSprites.Length; ++i)
        {
            m_PlantSprites[i] = Resources.Load<Sprite>("Plant/" + m_PlantCsv[i + 1, 2]);
            m_BluredPlantSprites[i] = Resources.Load<Sprite>("Plant/" + m_PlantCsv[i + 1, 2] + "_B");

            m_LibraryPlantCell[i].SetSprites(m_PlantSprites[i], m_BluredPlantSprites[i]);
            m_LibraryPlantCell[i].m_PlantNameString = m_PlantCsv[i + 1, 3];
            m_LibraryPlantCell[i].m_SpeciesId = i;
            m_LibraryPlantCell[i].m_State = DataManager.GetPlantLibraryState(i);
        }
    }

    public void OnGainPlant(int SpeciesId)
    {
        if (m_LibraryPlantCell[SpeciesId].m_State == LibraryState.UNKNOWN)
        {
            m_LibraryPlantCell[SpeciesId].m_State = LibraryState.DISCOVERED;
            switch (SpeciesId)
            {
                case 10:
                    DataManager.SetIsItemBuyable(ItemCategory.LAMP, 1, true);
                    DataManager.SetIsItemBuyable(ItemCategory.NUTRIENTS, 1, true);
                    DataManager.SetIsItemBuyable(ItemCategory.SPRINKLER, 1, true);
                    break;
                case 20:
                    DataManager.SetIsItemBuyable(ItemCategory.LAMP, 2, true);
                    DataManager.SetIsItemBuyable(ItemCategory.NUTRIENTS, 2, true);
                    DataManager.SetIsItemBuyable(ItemCategory.SPRINKLER, 2, true);
                    break;
                case 30:
                    DataManager.SetIsItemBuyable(ItemCategory.LAMP, 3, true);
                    DataManager.SetIsItemBuyable(ItemCategory.NUTRIENTS, 3, true);
                    DataManager.SetIsItemBuyable(ItemCategory.SPRINKLER, 3, true);
                    break;
                case 40:
                    DataManager.SetIsItemBuyable(ItemCategory.LAMP, 4, true);
                    DataManager.SetIsItemBuyable(ItemCategory.NUTRIENTS, 4, true);
                    DataManager.SetIsItemBuyable(ItemCategory.SPRINKLER, 4, true);
                    break;
            }
        }
    }

    public void ShowPlantInfo(int num)
    {
        m_InfoPanel.SetActive(true);
        m_PlantNameText.text = m_PlantCsv[num + 1, 3];
        m_PlantIdText.text = "ID : " + m_PlantCsv[num + 1, 1];
        m_PlantPointText.text = m_PlantCsv[num + 1, 6] + "점";
        m_PlantDialogueText.text = "\"" + m_PlantCsv[num + 1, 7] + "\"";
        m_PlantExplainText.text = m_PlantCsv[num + 1, 8];
        m_PlantInfoImage.sprite = m_PlantSprites[num];
        m_PlantInfoImage.SetNativeSize();
    }
}
