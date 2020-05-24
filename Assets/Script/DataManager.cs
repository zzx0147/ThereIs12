using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LibraryState
{
    UNKNOWN,//아직 발견하지 않음
    DISCOVERED,//발견됨(최초 발견해서 아직 도감을 보지 않음)
    IDENTIFIED,//확인됨(도감을 확인함)
    UNDEFINED//정의되지 않음
}

public class DataManager//데이터의 세이브와 로드를 담당
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

    public static int GetLampGrade()
    {
        return PlayerPrefs.GetInt("LampGrade", 1);
    }

    public static void SetLampGrade(int num)
    {
        PlayerPrefs.SetInt("LampGrade", num);
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

    public static int GetNumberOfAvailablePlant()
    {
        int num = PlayerPrefs.GetInt("NumberOfAbailablePlant", 15);
        return num;
    }

    public static void SetNumberOfAvailablePlant(int num)
    {
        PlayerPrefs.SetInt("NumberOfAbailablePlant", num);
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
        if (remaining < 0)
        {
            remaining = 0;
        }

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

    public static PlantDataStruct GetPlantData(int objId)
    {
        PlantDataStruct temp;
        temp.SpeciesId = PlayerPrefs.GetInt("Plant_SpeciesId_" + objId, -1);
        temp.state = (PlantState)PlayerPrefs.GetInt("Plant_State_" + objId, (int)PlantState.NONE);
        temp.RemainingTime = PlayerPrefs.GetInt("Plant_RemainingTime_" + objId, 0);
        temp.ReferenceTime = ulong.Parse(PlayerPrefs.GetString("Plant_ReferenceTime_" + objId, (0).ToString()));
        return temp;
    }

    public static void SetPlantData(int objId, int speciesId, PlantState state, int remainingTime, ulong referanceTime)
    {
        if(state == PlantState.SPROUT && speciesId == -1)
        {
            Debug.LogError("sprout wiht no adult type");
        }

        Debug.Log(objId + " : " + speciesId + " : " + state + " : " + remainingTime);
        PlayerPrefs.SetInt("Plant_SpeciesId_" + objId, speciesId);
        PlayerPrefs.SetInt("Plant_State_" + objId, (int)state);
        PlayerPrefs.SetInt("Plant_RemainingTime_" + objId, remainingTime);
        PlayerPrefs.SetString("Plant_ReferenceTime_" + objId, referanceTime.ToString());
        PlayerPrefs.Save();
    }

    public static LibraryState GetPlantLibraryState(int speciesId)
    {
        //Debug.Log((LibraryState)System.Enum.Parse(typeof(LibraryState), PlayerPrefs.GetString("Species_" + speciesId, "UNKNOWN")));
        return (LibraryState)System.Enum.Parse(typeof(LibraryState), PlayerPrefs.GetString("Species_" + speciesId, "UNKNOWN"));
    }

    public static void SetPlantLibraryState(int speciesId, LibraryState state)
    {
        PlayerPrefs.SetString("Species_" + speciesId, state.ToString());
        PlayerPrefs.Save();
    }

    public static int GetFeverCount()
    {
        return PlayerPrefs.GetInt("FeverCount", 0);
    }

    public static void SetFeverCount(int num)
    {
        PlayerPrefs.SetInt("FeverCount",num);
        PlayerPrefs.Save();
    }
}
