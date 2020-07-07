using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheat : MonoBehaviour
{
    public void SetSpeed()
    {
        Time.timeScale = 100.0f;
    }

    public void RemoveAllData()
    {
        //Debug.Log("RemoveAllData");
        PlayerPrefs.DeleteAll();
        Quit();
    }

    public static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
