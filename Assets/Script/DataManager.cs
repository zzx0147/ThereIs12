using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LibraryState
{
    UNKNOWN,
    DISCOVERED,
    IDENTIFIED,
    UNDEFINED
}

public class DataManager
{
    public static int GetMoney()
    {
        return PlayerPrefs.GetInt("Money", 0);
    }

    public static void SetMoney(int num)
    {
        PlayerPrefs.SetInt("Money", num);
        PlayerPrefs.Save();
    }

    public static int GetEquipmentGrade()
    {
        return PlayerPrefs.GetInt("EquipmentGrade", 1);
    }

    public static void SetEquipmentGrade(int num)
    {
        PlayerPrefs.SetInt("EquipmentGrade", num);
        PlayerPrefs.Save();
    }


    public static ulong GetReferenceTime()
    {
        return ulong.Parse(PlayerPrefs.GetString("ReferenceTime", System.DateTime.Now.ToFileTime().ToString().Substring(0, 11)));
    }

    public static void RecordReferenceTime()
    {
        PlayerPrefs.SetString("ReferenceTime", System.DateTime.Now.ToFileTime().ToString().Substring(0, 11));
        PlayerPrefs.Save();
    }

    public static int GetRemainingTime()
    {
        return PlayerPrefs.GetInt("RemainingTime", 0);
    }

    public static void SetRemainingTime(int remaining)
    {
        PlayerPrefs.SetInt("RemainingTime", remaining);
        PlayerPrefs.Save();
    }

    public static int GetMaxTimeOfLastUsedTimeItem()
    {
        return PlayerPrefs.GetInt("MaxTime", 1);
    }

    public static void SetMaxTimeOfLastUsedTimeItem(int maxTime)
    {
        PlayerPrefs.SetInt("MaxTime", maxTime);
        PlayerPrefs.Save();
    }

    public static ulong GetNow()
    {
        string s = PlayerPrefs.GetString("Now", System.DateTime.Now.ToFileTime().ToString().Substring(0, 11));
        return ulong.Parse(s);
    }

    public static int GetPlantData(int objId)
    {
        return PlayerPrefs.GetInt("Plant_" + objId, -1);
    }

    public static void SetPlantData(int objId, int speciesId)
    {
        PlayerPrefs.SetInt("Plant_" + objId, speciesId);
        PlayerPrefs.Save();
    }

    public static LibraryState GetPlantLibraryState(int speciesId)
    {
        Debug.Log((LibraryState)System.Enum.Parse(typeof(LibraryState), PlayerPrefs.GetString("Species_" + speciesId, "UNKNOWN")));
        return (LibraryState)System.Enum.Parse(typeof(LibraryState), PlayerPrefs.GetString("Species_" + speciesId, "UNKNOWN"));
    }

    public static void SetPlantLibraryState(int speciesId, LibraryState state)
    {
        PlayerPrefs.SetString("Species_" + speciesId, state.ToString());
        PlayerPrefs.Save();
    }
}
