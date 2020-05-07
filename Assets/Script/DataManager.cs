using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataManager
{

    public static int GetEquipmentGrade()
    {
        return PlayerPrefs.GetInt("EquipmentGrade",1);
    }

    public static void RecordTime()
    {
        PlayerPrefs.SetString("Now", System.DateTime.Now.ToFileTime().ToString().Substring(0, 11));
        PlayerPrefs.Save();
    }

    public static ulong GetTime()
    {
        string s = PlayerPrefs.GetString("Now", System.DateTime.Now.ToFileTime().ToString().Substring(0, 11));

        return ulong.Parse(s);
    }

    public static int GetPlantData(int id)
    {
        return PlayerPrefs.GetInt("Plant_" + id, -1);
    }

    public static void SetPlantData(int id, int num)
    {
        PlayerPrefs.SetInt("Plant_" + id, num);
        PlayerPrefs.Save();
    }
}
