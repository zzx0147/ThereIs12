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
        return PlayerPrefs.GetInt("LampGrade", 0);
    }

    public static void SetLampGrade(int num)
    {
        PlayerPrefs.SetInt("LampGrade", num);
        PlayerPrefs.Save();
    }

    public static int GetMaxLampGrade()
    {
        return PlayerPrefs.GetInt("MaxLampGrade", 0);
    }

    public static void SetMaxLampGrade(int num)
    {
        PlayerPrefs.SetInt("MaxLampGrade", num);
        PlayerPrefs.Save();
    }

    public static int GetSprinklerGrade()
    {
        return PlayerPrefs.GetInt("SprinklerGrade", 0);
    }

    public static void SetSprinklerGrade(int num)
    {
        PlayerPrefs.SetInt("SprinklerGrade", num);
        PlayerPrefs.Save();
    }

    public static int GetMaxSprinklerGrade()
    {
        return PlayerPrefs.GetInt("MaxSprinklerGrade", 0);
    }

    public static void SetMaxSprinklerGrade(int num)
    {
        PlayerPrefs.SetInt("MaxSprinklerGrade", num);
        PlayerPrefs.Save();
    }

    public static int GetNutrientsGrade()
    {
        return PlayerPrefs.GetInt("NutrientsGrade", 0);
    }

    public static void SetNutrientsGrade(int num)
    {
        PlayerPrefs.SetInt("NutrientsGrade", num);
        PlayerPrefs.Save();
    }

    public static int GetMaxNutrientsGrade()
    {
        return PlayerPrefs.GetInt("MaxNutrientsGrade", 0);
    }

    public static void SetMaxNutrientsGrade(int num)
    {
        PlayerPrefs.SetInt("MaxNutrientsGrade", num);
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

    public static ulong GetReferenceTimeOfTimeItem()
    {
        return ulong.Parse(PlayerPrefs.GetString("ReferenceTimeOfTimeItem", System.DateTime.Now.ToFileTime().ToString().Substring(0, 11)));
    }

    public static void RecordReferenceTimeOfTimeItem()
    {
        PlayerPrefs.SetString("ReferenceTimeOfTimeItem", System.DateTime.Now.ToFileTime().ToString().Substring(0, 11));
        PlayerPrefs.Save();
    }

    public static int GetRemainingTimeOfTimeItem()
    {
        return PlayerPrefs.GetInt("RemainingTimeOfTimeItem", 0);
    }

    public static void SetRemainingTimeOfTimeItem(int remaining)
    {
        if (remaining < 0)
        {
            remaining = 0;
        }

        PlayerPrefs.SetInt("RemainingTimeOfTimeItem", remaining);
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
        PlantDataStruct temp = new PlantDataStruct();
        temp.SpeciesId = PlayerPrefs.GetInt("Plant_SpeciesId_" + objId, -1);
        temp.state = (PlantState)PlayerPrefs.GetInt("Plant_State_" + objId, (int)PlantState.NONE);
        temp.RemainingTime = PlayerPrefs.GetInt("Plant_RemainingTime_" + objId, 0);
        temp.ReferenceTime = ulong.Parse(PlayerPrefs.GetString("Plant_ReferenceTime_" + objId, (0).ToString()));
        temp.MaxTime = PlayerPrefs.GetInt("Plant_MaxTime_" + objId, 0);
        return temp;
    }

    public static void SetPlantData(int objId, int speciesId, PlantState state, int MaxTime, int remainingTime, ulong referanceTime)
    {
        if (state == PlantState.SPROUT && speciesId == -1)
        {
            //Debug.LogError("sprout wiht no adult type");
        }

        //Debug.Log(objId + " : " + speciesId + " : " + state + " : " + remainingTime);
        PlayerPrefs.SetInt("Plant_SpeciesId_" + objId, speciesId);
        PlayerPrefs.SetInt("Plant_State_" + objId, (int)state);
        PlayerPrefs.SetInt("Plant_RemainingTime_" + objId, remainingTime);
        PlayerPrefs.SetInt("Plant_MaxTime_" + objId, MaxTime);
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
        PlayerPrefs.SetInt("FeverCount", num);
        PlayerPrefs.Save();
    }

    public static bool GetHaveItem(ItemCategory category, int num)
    {
        if (category == ItemCategory.ETC)
        {
            return false;
        }

        else if (num == 0)
        {
            return true;
        }

        int temp = PlayerPrefs.GetInt("Item_" + category.ToString() + "_" + num, 0);

        return (temp == 0) ? (false) : (true);
    }

    public static void SetHaveItem(ItemCategory category, int num, bool haveAlready)
    {
        PlayerPrefs.SetInt("Item_" + category.ToString() + "_" + num, (haveAlready) ? (1) : (0));
        PlayerPrefs.Save();
    }

    public static bool GetIsItemBuyable(ItemCategory category, int num)
    {
        if (category == ItemCategory.ETC)
        {
            return true;
        }
        else if (num == 0)
        {
            return true;
        }

        int temp = PlayerPrefs.GetInt("Item_" + category.ToString() + "_" + num + "_Buyable", 0);


        return (temp == 0) ? (false) : (true);
    }

    public static void SetIsItemBuyable(ItemCategory category, int num, bool isBuyable)
    {
        PlayerPrefs.SetInt("Item_" + category.ToString() + "_" + num + "_Buyable", (isBuyable) ? (1) : (0));
        PlayerPrefs.Save();
    }

    public static int GetLastUsedTimeItem()
    {
        return PlayerPrefs.GetInt("LastUsedTimeItem", -1);
    }

    public static void SetLastUsedTimeItem(int num)
    {
        PlayerPrefs.SetInt("LastUsedTimeItem", num);
        PlayerPrefs.Save();
    }

    public static int GetLastUsedETCItem()
    {
        return PlayerPrefs.GetInt("LastUsedETCItem", -1);
    }
    public static void SetLastUsedETCItem(int num)
    {
        PlayerPrefs.SetInt("LastUsedETCItem", num);
    }

    public static ulong GetReferenceTimeOfETCItem()
    {
        return ulong.Parse(PlayerPrefs.GetString("ReferenceTimeOfETCItem", System.DateTime.Now.ToFileTime().ToString().Substring(0, 11)));
    }

    public static void RecordReferenceTimeOfETCItem()
    {
        PlayerPrefs.SetString("ReferenceTimeOfETCItem", System.DateTime.Now.ToFileTime().ToString().Substring(0, 11));
        PlayerPrefs.Save();
    }

    public static int GetRemainingTimeOfETCItem()
    {
        return PlayerPrefs.GetInt("RemainingTimeOfETCItem", 0);
    }

    public static void SetRemainingTimeOfETCItem(int remaining)
    {
        if (remaining < 0)
        {
            remaining = 0;
        }

        PlayerPrefs.SetInt("RemainingTimeOfETCItem", remaining);
        PlayerPrefs.Save();
    }

    public static int GetLastUsedETCItemWeight()
    {
        return PlayerPrefs.GetInt("LastUsedETCItemWeight", -1);
    }

    public static void SetLastUsedETCItemWeight(int weight)
    {
        PlayerPrefs.SetInt("LastUsedETCItemWeight", weight);
        PlayerPrefs.Save();
    }

    public static bool GetMission(int num)
    {
        int complete = PlayerPrefs.GetInt("Mission_" + num, 0);
        return (complete == 1) ? (true) : (false);
    }

    public static void SetMission(int num, bool isComplete)
    {
        PlayerPrefs.SetInt("Mission_" + num, (isComplete) ? (1) : (0));
        PlayerPrefs.Save();
    }

    public static int GetDogPlantNum()
    {
        return PlayerPrefs.GetInt("DogPlant", 0);
    }

    public static void SetDogPlantNum(int num)
    {
        PlayerPrefs.SetInt("DogPlant", num);
        PlayerPrefs.Save();
    }

    public static int GetDiscoveredPlantNum()
    {
        return PlayerPrefs.GetInt("DiscoveredPlant",0);
    }

    public static void SetDiscoveredPlantNum(int num)
    {
        PlayerPrefs.SetInt("DiscoveredPlant", num);
        PlayerPrefs.Save();
    }

    public static void SetIsFirst(bool isFirst)
    {
        PlayerPrefs.SetInt("First",(isFirst)?(1):(0));
        PlayerPrefs.Save();
    }

    public static bool GetIsFirst()
    {
        return (PlayerPrefs.GetInt("First",1) == 1)?(true):(false);
    }

    public static float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat("SFX",0.5f);
    }

    public static void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFX", value);
        PlayerPrefs.Save();
    }

    public static float GetBGMVolume()
    {
        return PlayerPrefs.GetFloat("BGM", 0.5f);
    }

    public static void SetBGMVolume(float value)
    {
        PlayerPrefs.SetFloat("BGM", value);
        PlayerPrefs.Save();
    }

}
