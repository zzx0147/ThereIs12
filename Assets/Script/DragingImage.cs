using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragingImage : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    Vector2 BeginDragPos;
    bool m_DragEnabled = true;


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_DragEnabled)
        {
            Debug.Log("BeginDrag: " + eventData.position.y);
            BeginDragPos = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_DragEnabled)
        {
            Debug.Log("OnDrag" + eventData.position.y);
            if (eventData.position.y > BeginDragPos.y + 40)
            {
                Debug.Log("Fit!!!!!!");
                m_DragEnabled = false;
            }
        }
    }

}
