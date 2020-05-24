using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SnapScrollViewController : MonoBehaviour, IEndDragHandler, IBeginDragHandler
{
    public RectTransform[] snapPoint;
    private RectTransform contentRef;

    private float midXPos;
    private bool isLerpOn;
    private Vector3 targetPos;

    // Start is called before the first frame update
    void Start()
    {
        midXPos = snapPoint[0].localPosition.x;
        contentRef = GetComponent<ScrollRect>().content;
        Debug.Log("Mid Pos" + midXPos);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isLerpOn = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float targetPosX = 99999;
        Vector3 nowPos = contentRef.anchoredPosition;

        foreach (var v in snapPoint)
        {
            float temp = midXPos - v.localPosition.x;
            if (Mathf.Abs(nowPos.x - temp) < Mathf.Abs(nowPos.x - targetPosX))
            {
                targetPosX = temp;
            }
        }
        Debug.Log(targetPosX);

        targetPos = new Vector3(targetPosX, 0, 0);
        isLerpOn = true;
    }

    void Update()
    {
        if (isLerpOn)
        {
            contentRef.localPosition = Vector3.Lerp(contentRef.localPosition, targetPos, Time.deltaTime * 10);

            if (Vector3.Distance(contentRef.localPosition, targetPos) < 0.001f)
            {
                isLerpOn = false;
                contentRef.localPosition = targetPos;
            }
        }
    }
}
