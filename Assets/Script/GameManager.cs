using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int m_EquipmentGrade;
    private int m_Money;

    private float[] m_PlantRespawnTimeMin = { 70.0f, 70.0f * 0.8f, 70.0f * 0.6f, 70.0f * 0.4f, 70.0f * 0.2f, };
    private float[] m_PlantRespawnTimeMax = { 70.0f + 20.0f, (70.0f + 20.0f) * 0.8f, (70.0f + 20.0f) * 0.6f, (70.0f + 20.0f) * 0.4f, (70.0f + 20.0f) * 0.2f, };
    private float[] m_PlantRespawnProbability;
    private float m_MaxTime;
    private float m_RemainingTime = 0;

    private string[,] m_PlantData;

    private bool m_IsTimeLeft = false;

    [SerializeField]
    private PlantLibraryManager m_plantLibraryManager = null;

    [SerializeField]
    private Image m_TimeGaugeBarXImage = null;

    [SerializeField]
    private float m_DogPlantRespawnProbability = 50.0f;

    [SerializeField]
    private Image m_TimeGaugeBar = null;

    [SerializeField]
    private Image[] m_PlantImages = null;

    [SerializeField]
    private Plant[] m_Plants = null;

    [SerializeField]
    private Sprite[] m_PlantSprites = null;

    [SerializeField]
    private Text m_MoneyText = null;

    // Start is called before the first frame update
    private void Start()
    {
        Time.timeScale = 1.0f;//시간 배속
        m_Money = DataManager.GetMoney();//돈 저장갑 불러오기
        AddMoney(0);//돈 Text 업데이트
        m_EquipmentGrade = DataManager.GetEquipmentGrade();//장비 등급 불러오기
        m_MaxTime = DataManager.GetMaxTimeOfLastUsedTimeItem();//마지막으로 사용한 시간 아이템의 최대 시간량
        m_PlantRespawnProbability = new float[50/*m_PlantSprites.Length*/];//식물별 등장 확률 배열

        ulong refTime = DataManager.GetReferenceTime();//RemainingTime의 기준 시점
        ulong now = DataManager.GetNow();//현재 시간
        m_RemainingTime = DataManager.GetRemainingTime();//마지막으로 저장된 시간 아이템의 남은 시간
        m_RemainingTime -=( now - refTime);//시간 아이템의 남은 시간을 현재 시간 기준으로 업데이트
        DataManager.SetRemainingTime((int)m_RemainingTime);
        DataManager.RecordReferenceTime();

        if(m_RemainingTime > 0)
        {
            m_IsTimeLeft = true;
            m_TimeGaugeBarXImage.enabled = false;
        }
        else
        {
            m_TimeGaugeBar.fillAmount = 0.0f;
            m_TimeGaugeBarXImage.enabled = true;
        }




        for (int i = 0; i < m_PlantImages.Length; ++i)//접속 종료 전 식물의 데이터를 받아와 그대로 다시 배치함
        {
            int temp = DataManager.GetPlantData(i);
            m_Plants[i].m_PlantObjId = i;

            if (temp >= 0)
            {
                m_PlantImages[i].enabled = true;
                m_Plants[i].m_PlantSpeciesId = DataManager.GetPlantData(i);
                m_PlantImages[i].sprite = m_PlantSprites[DataManager.GetPlantData(i)];
                m_PlantImages[i].SetNativeSize();
            }
            else
            {
                m_PlantImages[i].enabled = false;
            }
        }

        m_PlantData = CsvLoader.LoadCsvBy2DimensionArray("Csv/PlantData");//Csv의 데이터를 읽어옴
        m_PlantRespawnProbability[0] = m_DogPlantRespawnProbability;//개풀(0번 식물)의 확률
        float devidedProbability = (50.0f - m_DogPlantRespawnProbability) / 49.0f;//(50 - 개풀 확률)/(개풀 제외 식물 수)로 하여 나온 값
        //개풀 확률을 50보다 낮게 조정할 경우에 줄어든 확률을 다른 식물에게 똑같이 분배

        for (int i = 1; i < m_PlantSprites.Length; ++i)
        {
            m_PlantRespawnProbability[i] = float.Parse(m_PlantData[i + 3, 13]) + devidedProbability;
        }

        RespawnPlantBetweenTurnOff(now - refTime);//꺼져있던 시간 만큼 식물 리스폰
        StartCoroutine(PlantRespawnIteration());//식물 스폰 코루틴
    }

    // Update is called once per frame
    private void Update()
    {
        if(m_IsTimeLeft)
        {
            m_RemainingTime -= Time.deltaTime;
            m_TimeGaugeBar.fillAmount = m_RemainingTime / m_MaxTime;
            if(m_RemainingTime < 0.0f)
            {
                m_IsTimeLeft = false;
                m_TimeGaugeBarXImage.enabled = true;
            }
        }
    }

    private IEnumerator PlantRespawnIteration()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(m_PlantRespawnTimeMin[m_EquipmentGrade], m_PlantRespawnTimeMax[m_EquipmentGrade]));
            if(m_IsTimeLeft)
            {
                SpawnRandomPlant();
            }
            DataManager.RecordReferenceTime();
            DataManager.SetRemainingTime((int)m_RemainingTime);
        }
    }

    private void RespawnPlantBetweenTurnOff(float PassedTime)
    {
        while ((PassedTime -= Random.Range(m_PlantRespawnTimeMin[m_EquipmentGrade], m_PlantRespawnTimeMax[m_EquipmentGrade])) >= 0)
        {
            SpawnRandomPlant();
        }
    }

    private void SpawnRandomPlant()
    {
        int[] temp = GetEmptyImageNumbers();
        if (temp.Length < 1)
        {
            return;
        }

        int targetImage = Random.Range(0, temp.Length);

        m_PlantImages[temp[targetImage]].enabled = true;

        while (true)
        {
            float randFloat = Random.Range(0.0f, 100.0f);
            float probabilityMin = 0.0f;
            for (int i = 0; i < m_EquipmentGrade * 10; ++i)
            {
                if (probabilityMin <= randFloat && m_PlantRespawnProbability[i] + probabilityMin > randFloat)//확률 범위 안에 randFloat가 들어올 경우
                {
                    m_PlantImages[temp[targetImage]].sprite = m_PlantSprites[i];
                    m_PlantImages[temp[targetImage]].SetNativeSize();
                    m_Plants[temp[targetImage]].m_PlantSpeciesId = i;
                    DataManager.SetPlantData(temp[targetImage], i);
                    
                    goto whileEnd;
                }
                probabilityMin += m_PlantRespawnProbability[i];
            }
        }
    whileEnd:;
    }

    private int[] GetEmptyImageNumbers()
    {
        List<int> temp = new List<int>();

        for (int i = 0; i < m_PlantImages.Length; ++i)
        {
            if (!m_PlantImages[i].enabled)
            {
                temp.Add(i);
            }
        }

        return temp.ToArray();
    }

    public bool UseMoney(int price)
    {
        if(m_Money - price >= 0)
        {
            m_Money -= price;
            m_MoneyText.text = m_Money.ToString();
            DataManager.SetMoney(m_Money);
            return true;
        }
        return false;
    }

    public void AddMoney(int price)
    {
        m_Money += price;
        m_MoneyText.text = m_Money.ToString();
        DataManager.SetMoney(m_Money);
    }

    public void BuyTimeItem(int time,int price)
    {
        if(UseMoney(price))
        {
            m_MaxTime = time;
            m_RemainingTime = time;
            m_IsTimeLeft = true;
            m_TimeGaugeBarXImage.enabled = false;
            m_TimeGaugeBar.fillAmount = 1;
            DataManager.RecordReferenceTime();
            DataManager.SetRemainingTime((int)m_RemainingTime);
            DataManager.SetMaxTimeOfLastUsedTimeItem((int)m_RemainingTime);
        }
    }

    public void Buy(int num)
    {
        int time = 1 ;
        int price = 0;
        switch(num)
        {
            case 0:
                time = 600;
                price = 100;
                break;
            case 1:
                time = 1800;
                price = 35;
                break;
            case 2:
                time = 3200;
                price = 0;
                break;
            case 3:
                time = 16000;
                price = 20;
                break;
            case 4:
                time = 32000;
                price = 65;
                break;
        }
        BuyTimeItem(time, price);

    }

    public void GainPlant(int plantObjId)
    {
        Debug.Log("ObjId:"+plantObjId+"\nSpeciesId: " + m_Plants[plantObjId].m_PlantSpeciesId);

        AddMoney(int.Parse(m_PlantData[m_Plants[plantObjId].m_PlantSpeciesId + 1 , 18]));
        m_PlantImages[plantObjId].sprite = null;
        m_PlantImages[plantObjId].enabled = false;
        m_plantLibraryManager.UpdatePlantLibrary(m_Plants[plantObjId].m_PlantSpeciesId);
        m_Plants[plantObjId].m_PlantSpeciesId = -1;
        DataManager.SetPlantData(plantObjId, m_Plants[plantObjId].m_PlantSpeciesId);
    }
}
