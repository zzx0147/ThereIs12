using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSlider : MonoBehaviour
{
    [SerializeField]
    private Transform[] m_CameraPosition = null;
    Coroutine m_NowPlayedCoroutine = null;


    // Update is called once per frame
    void Update()
    {

    }

    public void MoveCameraPoint(int pointNum)
    {
        if(m_NowPlayedCoroutine != null)
        {
            StopCoroutine(m_NowPlayedCoroutine);
        }
        m_NowPlayedCoroutine = StartCoroutine(MoveCameraPointCoroutine(pointNum));
    }

    IEnumerator MoveCameraPointCoroutine(int pointNum)
    {
        Vector3 start = transform.position;
        float temp = 0.0f;
        while((temp - 1.0f) < Mathf.Epsilon)
        {
            transform.position = Vector3.Lerp(start, m_CameraPosition[pointNum].position, temp);
            temp += Time.deltaTime;
            yield return null;
        }
        m_NowPlayedCoroutine = null;
    }
}
