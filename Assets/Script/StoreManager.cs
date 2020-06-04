using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    [SerializeField] private GameManager m_GameManager = null;

    [SerializeField] private GameObject m_NotifyPanel = null;
    [SerializeField] private Text m_NotifyText;

    [SerializeField] private GameObject m_QuestionPanel = null;
    [SerializeField] private Text m_QuestionText = null;
    [SerializeField] private Button m_QuestionYesButton = null;

    [SerializeField] private StoreItemCell[] m_LampItemCells = null;
    [SerializeField] private StoreItemCell[] m_SprinklerItemCells = null;
    [SerializeField] private StoreItemCell[] m_NutrientsItemCells = null;
    [SerializeField] private StoreItemCell[] m_ETCItemCells = null;

    [SerializeField] private Sprite[] m_LampSprites = null;
    [SerializeField] private Sprite[] m_SprinklerSprites = null;
    [SerializeField] private Sprite[] m_NutrientsSprites = null;
    [SerializeField] private Sprite[] m_ETCSprites = null;

    private ItemCategory m_RecentSelectedItemCategory;
    private int m_RecentSelectedItemId;


    private string[,] m_ItemCsv = null;

    private void Awake()
    {
        m_ItemCsv = CsvLoader.LoadCsvBy2DimensionArray("Csv/Item_Table");

        int i = 0;
        for (int j = 0; i < m_LampItemCells.Length; ++i, ++j)
        {
            m_LampItemCells[j].Initialize(m_LampSprites[j], ItemCategory.LAMP, j, m_ItemCsv[i + 1, 2], int.Parse(m_ItemCsv[i + 1, 8]), DataManager.GetHaveItem(ItemCategory.LAMP, j));
            m_LampItemCells[j].m_OnClickedEvent.AddListener(OnStoreItemCellClicked);
        }

        for (int j = 0; i < m_LampItemCells.Length + m_NutrientsItemCells.Length; ++i, ++j)
        {
            m_NutrientsItemCells[j].Initialize(m_NutrientsSprites[j], ItemCategory.NUTRIENTS, j, m_ItemCsv[i + 1, 2], int.Parse(m_ItemCsv[i + 1, 8]), DataManager.GetHaveItem(ItemCategory.NUTRIENTS, j));
            m_NutrientsItemCells[j].m_OnClickedEvent.AddListener(OnStoreItemCellClicked);
        }

        for (int j = 0; i < m_LampItemCells.Length + m_NutrientsItemCells.Length + m_SprinklerItemCells.Length; ++i, ++j)
        {
            m_SprinklerItemCells[j].Initialize(m_SprinklerSprites[j], ItemCategory.SPRINKLER, j, m_ItemCsv[i + 1, 2], int.Parse(m_ItemCsv[i + 1, 8]), DataManager.GetHaveItem(ItemCategory.SPRINKLER, j));
            m_SprinklerItemCells[j].m_OnClickedEvent.AddListener(OnStoreItemCellClicked);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnStoreItemCellClicked(ItemCategory category, int id)
    {
        m_RecentSelectedItemCategory = category;
        m_RecentSelectedItemId = id;

        if (DataManager.GetHaveItem(category, id))//가지고 있는 아이템인 경우
        {
            Debug.Log("already have!!");
            switch (category)//이미 사용중인 장비를 클릭했을 경우
            {
                case ItemCategory.LAMP:
                    if (id == m_GameManager.m_LampGrade)
                    {
                        m_NotifyText.text = "이미 사용중인 장비입니다";
                        m_NotifyPanel.SetActive(true);
                        return;
                    }
                    break;
                case ItemCategory.NUTRIENTS:
                    if (id == m_GameManager.m_NutrientsGrade)
                    {
                        m_NotifyText.text = "이미 사용중인 장비입니다";
                        m_NotifyPanel.SetActive(true);
                        return;
                    }
                    break;
                case ItemCategory.SPRINKLER:
                    if (id == m_GameManager.m_SprinklerGrade)
                    {
                        m_NotifyText.text = "이미 사용중인 장비입니다";
                        m_NotifyPanel.SetActive(true);
                        return;
                    }
                    break;
                default:
                    break;
            }
            //사용중인 장비가 아닌 경우
            m_QuestionPanel.SetActive(true);
            m_QuestionText.text = m_ItemCsv[(int)category * 5 + id + 1, 13];
            m_QuestionYesButton.onClick.RemoveAllListeners();
            m_QuestionYesButton.onClick.AddListener(ChangeEquipment);
            m_QuestionYesButton.onClick.AddListener(delegate { m_QuestionPanel.SetActive(false);});

        }
        else//가지고 있지 않은 아이템일 경우
        {
            if(DataManager.GetIsItemBuyable(category, id))//기본 조건이 충족된 경우
            {
                m_QuestionPanel.SetActive(true);
                m_QuestionText.text = m_ItemCsv[(int)category * 5 + id + 1, 6];
                m_QuestionYesButton.onClick.RemoveAllListeners();
                m_QuestionYesButton.onClick.AddListener(TryBuyItem);
            }
            else//기본 조건이 충족되지 않은 경우
            {
                m_NotifyText.text = m_ItemCsv[(int)category * 5 + id + 1, 5];
                m_NotifyPanel.SetActive(true);
                return;
            }
        }
    }

    public void TryBuyItem()
    {
        m_QuestionPanel.SetActive(false);

        if (m_GameManager.AddMoney(int.Parse(m_ItemCsv[(int)m_RecentSelectedItemCategory * 5 + m_RecentSelectedItemId + 1, 8])))
        {
            m_NotifyPanel.SetActive(true);
            m_NotifyText.text = m_ItemCsv[(int)m_RecentSelectedItemCategory * 5 + m_RecentSelectedItemId + 1, 10];
            DataManager.SetHaveItem(m_RecentSelectedItemCategory, m_RecentSelectedItemId, true);
            ChangeEquipment();

            switch (m_RecentSelectedItemCategory)
            {
                case ItemCategory.LAMP:
                    m_LampItemCells[m_RecentSelectedItemId].SetAlreadyHave(true);
                    break;
                case ItemCategory.NUTRIENTS:
                    m_NutrientsItemCells[m_RecentSelectedItemId].SetAlreadyHave(true);
                    break;
                case ItemCategory.SPRINKLER:
                    m_SprinklerItemCells[m_RecentSelectedItemId].SetAlreadyHave(true);
                    break;
            }

        }
        else
        {
            m_NotifyPanel.SetActive(true);
            m_NotifyText.text = m_ItemCsv[(int)m_RecentSelectedItemCategory * 5 + m_RecentSelectedItemId + 1, 9];
        }
    }

    public void ChangeEquipment()
    {
        //m_QuestionPanel.SetActive(false);
        switch (m_RecentSelectedItemCategory)//이미 사용중인 장비를 클릭했을 경우
        {
            case ItemCategory.LAMP:
                m_GameManager.m_LampGrade = m_RecentSelectedItemId;
                break;
            case ItemCategory.NUTRIENTS:
                m_GameManager.m_NutrientsGrade = m_RecentSelectedItemId;
                break;
            case ItemCategory.SPRINKLER:
                m_GameManager.m_SprinklerGrade = m_RecentSelectedItemId;
                break;
        }
    }
}
