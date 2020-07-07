using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ItemCategory
{
    LAMP,
    NUTRIENTS,
    SPRINKLER,
    ETC
}

class OnClickedEvent : UnityEvent<ItemCategory, int>
{

}

public class StoreItemCell : MonoBehaviour
{
    [SerializeField] private Text m_IntroductionText = null;
    [SerializeField] private Image m_ItemImage = null;
    [SerializeField] private Button m_Button = null;

    private int m_Price;
    private string m_Name;
    private bool m_AlreadyHave;
    private ItemCategory m_Category;
    private int m_Id;

    private UnityEvent<ItemCategory,int> m_onClickedEvent = null;

    public UnityEvent<ItemCategory,int> m_OnClickedEvent { get => m_onClickedEvent; }

    public void Initialize(Sprite sprite,ItemCategory category,int id, string name, int price, bool alreadyHave)
    {
        m_onClickedEvent = new OnClickedEvent();
        m_Button.onClick.AddListener(OnClick);
        m_ItemImage.sprite = sprite;
        m_Category = category;
        m_Id = id;
        m_Name = name;
        m_Price = price;
        m_AlreadyHave = alreadyHave;
        m_IntroductionText.text = m_Price + "pt " + m_Name;
        if (m_AlreadyHave)
        {
            m_ItemImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else
        {
            m_ItemImage.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        }

    }

    public void SetAlreadyHave(bool AlreadyHave)
    {
        m_AlreadyHave = AlreadyHave;
        if (m_AlreadyHave)
        {
            m_ItemImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else
        {
            m_ItemImage.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    public void OnClick()
    {
        m_OnClickedEvent.Invoke(m_Category,m_Id);
    }
}
