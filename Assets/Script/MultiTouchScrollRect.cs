using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultiTouchScrollRect : ScrollRect
{
#if UNITY_EDITOR

#elif UNITY_ANDROID
    private int minimumTouchCount = 2, maximumTouchCount = 2, pointerId = -100;
    public Vector2 MultiTouchPosition
    {
        get
        {
            Vector2 position = Vector2.zero;
            for (int i = 0; i < Input.touchCount && i < maximumTouchCount; i++)
            {
                position += Input.touches[i].position;
            }
            position /= ((Input.touchCount <= maximumTouchCount) ? Input.touchCount : maximumTouchCount);
            return position;
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (Input.touchCount >= minimumTouchCount)
        {
            pointerId = eventData.pointerId;
            eventData.position = MultiTouchPosition;
            base.OnBeginDrag(eventData);
        }
    }
    public override void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount >= minimumTouchCount)
        {
            eventData.position = MultiTouchPosition;
            if (pointerId == eventData.pointerId)
            {
                base.OnDrag(eventData);
            }
        }
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (Input.touchCount >= minimumTouchCount)
        {
            pointerId = -100;
            eventData.position = MultiTouchPosition;
            base.OnEndDrag(eventData);
        }
    }
#endif
}