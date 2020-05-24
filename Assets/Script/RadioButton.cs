using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RadioButton : Button
{
    [SerializeField]
    private RadioButton[] m_OtherButton;

    [SerializeField]
    private Vector2 m_ChangeDeltaOnPressed;

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        gameObject.GetComponent<RectTransform>().sizeDelta = m_ChangeDeltaOnPressed;
        interactable = false;

        foreach (var v in m_OtherButton)
        {
            v.OnOtherButtonClick();
        }

        Debug.Log("called");
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
    }

    public void OnOtherButtonClick()
    {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        interactable = true;
    }
}
