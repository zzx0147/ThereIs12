using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TouchEffectController : MonoBehaviour
{
    [SerializeField] private GameObject m_NormalTouchEffectPrefab = null;
    [SerializeField] private GameObject m_PlantTouchEffectPrefab = null;
    [SerializeField] private AudioSource m_NormalTouchSFX = null;

    void Update()
    {
        RaycastWorldUI();
    }

    void RaycastWorldUI()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);

            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                //WorldUI is my layer name
                if (results[0].gameObject.layer == LayerMask.NameToLayer("UI"))
                {
                    if (results[0].gameObject.GetComponent<Plant>() != null)
                    {
                        Instantiate(m_PlantTouchEffectPrefab, results[0].worldPosition + new Vector3(0, 0, 0.5f), Quaternion.Euler(0, 0, 0)).transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                    }
                    else
                    {
                        m_NormalTouchSFX.Play();
                        Instantiate(m_NormalTouchEffectPrefab, results[0].worldPosition + new Vector3(0, 0, 0.5f), Quaternion.Euler(0, 0, 0)).transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                    }
                    //string dbg = "Root Element: {0} \n GrandChild Element: {1}";
                    //Debug.Log(string.Format(dbg, results[results.Count - 1].gameObject.name, results[0].gameObject.name));
                    ////Debug.Log("Root Element: "+results[results.Count-1].gameObject.name);
                    //Debug.Log("GrandChild Element: "+results[0].gameObject.name);
                    results.Clear();
                }
            }
        }
    }
}
