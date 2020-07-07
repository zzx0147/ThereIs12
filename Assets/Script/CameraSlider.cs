using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSlider : MonoBehaviour
{
    [SerializeField]
    private Transform[] m_CameraPosition = null;
    Coroutine m_NowPlayedCoroutine = null;

    public void MoveCameraPoint(int pointNum)
    {
        if (m_NowPlayedCoroutine != null)
        {
            StopCoroutine(m_NowPlayedCoroutine);
        }
        m_NowPlayedCoroutine = StartCoroutine(MoveCameraPointCoroutine(pointNum));
    }

    IEnumerator MoveCameraPointCoroutine(int pointNum)
    {
        while ((Vector3.Distance(transform.position, m_CameraPosition[pointNum].position) > Mathf.Epsilon))
        {
            transform.position = Vector3.Lerp(transform.position, m_CameraPosition[pointNum].position, Time.unscaledDeltaTime * 4);
            yield return null;
        }
        m_NowPlayedCoroutine = null;
    }
}
