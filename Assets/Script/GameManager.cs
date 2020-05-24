using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Variable
    private int m_NumberOfAvailablePlant;
    private int m_LampGrade;
    private int m_EquipmentGrade;
    private int m_TonicGrade;
    private int m_SpringklerGrade;
    private int m_Money;
    private int m_FeverCount;

    [SerializeField]
    private int m_FeverCountMax;

    private float[] m_PlantRespawnTimeMin = { 70.0f, 70.0f * 0.8f, 70.0f * 0.6f, 70.0f * 0.4f, 70.0f * 0.2f, };
    private float[] m_PlantRespawnTimeMax = { 70.0f + 20.0f, (70.0f + 20.0f) * 0.8f, (70.0f + 20.0f) * 0.6f, (70.0f + 20.0f) * 0.4f, (70.0f + 20.0f) * 0.2f, };
    private float[] m_PlantRespawnProbability;
    private float m_MaxTime;
    private float m_RemainingTime = 0;

    private string[,] m_PlantCsv;
    private string[,] m_PlantProbabilityCsv;

    private bool m_IsTimeLeft = false;
    private bool m_LastSpawnSproutIsFailed = false;
    private bool m_isFeverOn = false;
    #endregion

    #region ReferanceVariable
    private Coroutine m_ChangeGamePanelCoroutine = null;
    private Coroutine m_FeverTimeCoroutine = null;

    [SerializeField]
    private RectTransform m_Content = null;//m_GamePanels를 자식으로 하는 Content 패널

    [SerializeField]
    private PlantLibraryManager m_plantLibraryManager = null;//도감 매니저 레퍼런스

    [SerializeField]
    private float m_DogPlantRespawnProbability = 50.0f;//개풀 스폰 확률

    [SerializeField]
    private Image m_FeverTimeGaugeBar = null;//피버 시간 게이지 바
    [SerializeField]
    private Image m_FeverTimeGaugeBarBack = null;//피버 시간 게이지 바 배경

    [SerializeField]
    private Plant[] m_Plants = null;//식물이 터치되었을 때 게임 매니저로 터치되었다는 걸 알려주는 클래스 식물 이미지와 동일한 오브젝트의 컴포넌트

    [SerializeField]
    private RectTransform m_MainPanel = null;

    private Sprite[] m_PlantSprites = new Sprite[51];//각 식물의 스프라이트 배열

    private Sprite m_SproutSprite = null;

    [SerializeField]
    private Text m_MoneyText = null;//돈을 표기하는 텍스트(연구점수)

    [SerializeField]
    private Text m_FeverCountText = null;

    [SerializeField]
    private Text m_TimerText = null;
    #endregion

    private bool IsTimeLeft
    {
        get => m_IsTimeLeft;
        set
        {
            m_IsTimeLeft = value;
            if (m_IsTimeLeft)
            {
                Time.timeScale = 1.0f;
            }
            else
            {
                Time.timeScale = 0.0f;
            }
        }
    }

    private void Awake()
    {
        m_PlantProbabilityCsv = CsvLoader.LoadCsvBy2DimensionArray("Csv/PlantVariableTable");
        m_PlantCsv = CsvLoader.LoadCsvBy2DimensionArray("Csv/PlantMasterTable");//Csv의 데이터를 읽어옴
        m_SproutSprite = Resources.Load<Sprite>("Plant/IL_172");
        for (int i = 0; i < m_PlantSprites.Length; ++i)
        {
            m_PlantSprites[i] = Resources.Load<Sprite>("Plant/" + m_PlantCsv[i + 1, 2]);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_FeverCount = DataManager.GetFeverCount();
        m_FeverCountText.text = m_FeverCount + "/" + m_FeverCountMax;
        Time.timeScale = 1.0f;//시간 배속
        m_Money = DataManager.GetMoney();//돈 저장갑 불러오기
        AddMoney(0);//돈 Text 업데이트
        m_LampGrade = DataManager.GetLampGrade();//램프 등급 불러오기
        m_EquipmentGrade = DataManager.GetEquipmentGrade();
        m_MaxTime = DataManager.GetMaxTimeOfLastUsedTimeItem();//마지막으로 사용한 시간 아이템의 최대 시간량
        m_PlantRespawnProbability = new float[51/*m_PlantSprites.Length*/];//식물별 등장 확률 배열

        ulong refTime = DataManager.GetReferenceTime();//RemainingTime의 기준 시점
        ulong now = DataManager.GetNow();//현재 시간
        float pastRemainingTime = m_RemainingTime = DataManager.GetRemainingTime();//마지막으로 저장된 시간 아이템의 남은 시간
        m_RemainingTime -= (now - refTime);//시간 아이템의 남은 시간을 현재 시간 기준으로 업데이트
        if (m_RemainingTime < 0)
        {
            m_RemainingTime = 0;
        }

        DataManager.SetRemainingTime((int)m_RemainingTime);
        DataManager.RecordReferenceTime();
        m_TimerText.text = m_TimerText.text = (int)m_RemainingTime / 60 + ":" + (int)m_RemainingTime % 60;

        m_NumberOfAvailablePlant = DataManager.GetNumberOfAvailablePlant();


        if (m_RemainingTime > 0)
        {
            IsTimeLeft = true;
        }
        else
        {
            IsTimeLeft = false;
            m_FeverTimeGaugeBar.fillAmount = 0.0f;
        }

        #region Probability Compute

        m_PlantRespawnProbability[0] = m_DogPlantRespawnProbability;//개풀(0번 식물)의 확률
        m_PlantRespawnProbability[1] = 0;//녹초(1번)는 자연 스폰이 되지 않음
        float devidedProbability = (50.0f - m_DogPlantRespawnProbability) / 49.0f;//(50 - 개풀 확률)/(개풀 제외 식물 수)로 하여 나온 값
        //개풀 확률을 50보다 낮게 조정할 경우에 줄어든 확률을 다른 식물에게 똑같이 분배

        for (int i = 2; i < m_PlantSprites.Length - 1; ++i)
        {
            m_PlantRespawnProbability[i] = float.Parse(m_PlantProbabilityCsv[i + 3, 1]) + devidedProbability;
        }

        #endregion



        bool haveSprout = false;
        for (int i = 0; i < m_Plants.Length; ++i)
        {
            PlantDataStruct temp = DataManager.GetPlantData(i);
            m_Plants[i].OnEndGrowEvent.RemoveAllListeners();
            m_Plants[i].OnHarvestEvent.RemoveAllListeners();
            m_Plants[i].OnEndGrowEvent.AddListener(SpawnSprout);
            m_Plants[i].OnHarvestEvent.AddListener(GainPlant);

            if (temp.state != PlantState.SPROUT)
            {
                m_Plants[i].Initialize(i, temp.SpeciesId, m_SproutSprite, (temp.SpeciesId == -1) ? (null) : (m_PlantSprites[temp.SpeciesId]), temp.state);

            }
            else
            {
                if ((ulong)temp.RemainingTime < (now - temp.ReferenceTime))//새싹의 성장까지 남은 시간이 흐른 시간보다 적다(이미 성장했어야 하는 시간이다)
                {
                    ulong lastSproutGrowingTime = 0;
                    m_Plants[i].Initialize(i, temp.SpeciesId, m_SproutSprite, m_PlantSprites[temp.SpeciesId], PlantState.ADULT);
                    lastSproutGrowingTime = temp.ReferenceTime + (ulong)temp.RemainingTime;
                    RespawnPlantBetweenTurnOff((IsTimeLeft) ? now - lastSproutGrowingTime : pastRemainingTime);
                    SpawnSprout();
                    haveSprout = true;
                }
                else//새싹의 성장까지 남은 시간이 흐른 시간보다 많다(아직 성장하지 않았을 시간이다)
                    //이 말은 게임 종료 후 다시 접속했을 때 마지막 새싹이 아직 자라지 않았음을 의미
                    //즉 지난 시간을 계산해 식물을 소환할 필요가 없음을 의미
                {
                    m_Plants[i].Initialize(i, temp.SpeciesId, m_SproutSprite, m_PlantSprites[temp.SpeciesId], PlantState.SPROUT);
                    m_Plants[i].StartGrowing((temp.RemainingTime - (int)(now - temp.ReferenceTime)), temp.SpeciesId, m_PlantSprites[temp.SpeciesId]);
                    haveSprout = true;
                }
            }
        }

        if (!haveSprout)
        {
            SpawnSprout();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsTimeLeft)
        {
            m_RemainingTime -= Time.deltaTime;

            m_TimerText.text = (int)m_RemainingTime / 60 + ":" + (int)m_RemainingTime % 60;

            if (m_RemainingTime < 0.0f)
            {
                m_TimerText.text = "0:0";
                IsTimeLeft = false;
                Time.timeScale = 0.0f;
            }
        }
    }

    private float RespawnPlantBetweenTurnOff(float time)
    {
        while (true)
        {
            float randTime = Random.Range(m_PlantRespawnTimeMin[m_LampGrade], m_PlantRespawnTimeMax[m_LampGrade]);
            if ((time - randTime) >= 0)
            {
                time -= randTime;
                SpawnRandomAdultPlant();
            }
            else
            {
                return randTime - time;
            }

        }
    }

    private void SpawnRandomAdultPlant()
    {
        int[] EmptyPlantObjectsIndexes = GetEmptyImageNumbers();
        if (EmptyPlantObjectsIndexes.Length < 1)
        {
            return;
        }

        int targetPlant = Random.Range(0, EmptyPlantObjectsIndexes.Length);

        while (true)
        {
            float randFloat = Random.Range(0.0f, 100.0f);//
            float probabilityMin = 0.0f;
            for (int i = 0; i < m_EquipmentGrade * 10 + 1; ++i)//현재 장비 등급이 허용하는 범위 내의 식물을 순회하며 확률에 걸렸는지 체크
            {
                if (probabilityMin <= randFloat && m_PlantRespawnProbability[i] + probabilityMin > randFloat)//확률 범위 안에 randFloat가 들어올 경우
                {
                    m_Plants[EmptyPlantObjectsIndexes[targetPlant]].Initialize(EmptyPlantObjectsIndexes[targetPlant], i, m_SproutSprite, m_PlantSprites[i], PlantState.ADULT);
                    DataManager.SetPlantData(EmptyPlantObjectsIndexes[targetPlant], i, PlantState.ADULT, 0, 0);
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

        for (int i = 0; i < m_NumberOfAvailablePlant; ++i)
        {
            if (m_Plants[i].m_State == PlantState.NONE)
            {
                temp.Add(i);
            }
        }

        return temp.ToArray();
    }

    public bool UseMoney(int price)
    {
        if (m_Money - price >= 0)
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

    public void BuyTimeItem(int time, int price)
    {
        if (UseMoney(price))
        {
            m_MaxTime = time;
            m_RemainingTime = time;
            IsTimeLeft = true;
            m_FeverTimeGaugeBar.fillAmount = 1;
            DataManager.RecordReferenceTime();
            DataManager.SetRemainingTime((int)m_RemainingTime);
            DataManager.SetMaxTimeOfLastUsedTimeItem((int)m_RemainingTime);
        }
    }

    public void Buy(int num)
    {
        int time = 1;
        int price = 0;
        switch (num)
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
                time = 3600;
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

    public void GainPlant(int speciesId)
    {
        AddMoney(int.Parse(m_PlantCsv[speciesId + 1, 6]));
        m_plantLibraryManager.DiscoverPlant(speciesId);

        if (!m_isFeverOn)
        {
            m_FeverCountText.text = ++m_FeverCount + "/" + m_FeverCountMax;
            DataManager.SetFeverCount(m_FeverCount);

            if (m_FeverCount >= m_FeverCountMax)
            {
                m_FeverCount = 0;
                Debug.Log("Ready to Fever");
                StartFeverTime();
                return;
            }
        }


        if (m_LastSpawnSproutIsFailed)
        {
            SpawnSprout();
        }
    }

    public void SpawnSprout()
    {

        float randTime = Random.Range(m_PlantRespawnTimeMin[m_LampGrade], m_PlantRespawnTimeMax[m_LampGrade]);
        int selecObj = SelectOnePlantObjRandomly();

        if (selecObj == -1)
        {
            m_LastSpawnSproutIsFailed = true;
            return;
        }
        m_LastSpawnSproutIsFailed = false;

        int species = SelectOnePlantSpeciesRandomly();

        m_Plants[selecObj].StartGrowing(randTime, species, m_PlantSprites[species]);

    }

    private int SelectOnePlantObjRandomly()
    {
        int[] EmptyPlantObjectsIndexes = GetEmptyImageNumbers();
        if (EmptyPlantObjectsIndexes.Length <= 0)
        {
            return -1;
        }

        int targetPlant = Random.Range(0, EmptyPlantObjectsIndexes.Length);

        return EmptyPlantObjectsIndexes[targetPlant];
    }

    private int SelectOnePlantSpeciesRandomly()
    {
        while (true)
        {
            float randFloat = Random.Range(0.0f, 100.0f);//
            float probabilityMin = 0.0f;
            for (int i = 0; i < m_EquipmentGrade * 10 + 1; ++i)//현재 장비 등급이 허용하는 범위 내의 식물을 순회하며 확률에 걸렸는지 체크
            {
                if (probabilityMin <= randFloat && m_PlantRespawnProbability[i] + probabilityMin > randFloat)//확률 범위 안에 randFloat가 들어올 경우
                {
                    return i;
                }
                probabilityMin += m_PlantRespawnProbability[i];
            }
        }
    }

    public void ChangeGamePanel(int num)//게임 패널을 변경
    {
        if (m_ChangeGamePanelCoroutine != null)
        {
            StopCoroutine(m_ChangeGamePanelCoroutine);
        }

        m_ChangeGamePanelCoroutine = StartCoroutine(ChangeGamePanelCoroutine(num));
    }

    private IEnumerator ChangeGamePanelCoroutine(int num)//게임 패널을 변경하는 코루틴//메인화면은 0번 상점은 1번 도감은 2번 설정은 3번
    {
        int screenwidth = 2 * (int)m_MainPanel.localPosition.x;
        int targetPositionX = -(screenwidth * num);
        Debug.Log("Start");
        while (true)
        {
            if (Mathf.Abs(m_Content.transform.localPosition.x - targetPositionX) < Mathf.Epsilon)
            {
                break;
            }
            m_Content.transform.localPosition = new Vector3(Mathf.Lerp(m_Content.transform.localPosition.x, targetPositionX, Time.unscaledDeltaTime * 4), m_Content.transform.localPosition.y, 0);
            yield return null;
        }

        m_ChangeGamePanelCoroutine = null;
    }

    private void StartFeverTime()
    {
        if (!m_isFeverOn)
        {
            m_isFeverOn = true;
            m_FeverCount = 0;
            m_FeverCountText.text = m_FeverCount + "/" + m_FeverCountMax;
            DataManager.SetFeverCount(m_FeverCount);
            Time.timeScale = 0.0f;
            m_FeverTimeGaugeBar.gameObject.SetActive(true);
            m_FeverTimeGaugeBar.fillAmount = 1.0f;
            m_FeverTimeGaugeBarBack.gameObject.SetActive(true);
            for (int i = 0; i < m_Plants.Length; ++i)
            {
                if (m_Plants[i].m_State == PlantState.ADULT)
                {
                    GainPlant(m_Plants[i].m_PlantSpeciesId);
                }
                m_Plants[i].Initialize(i,-1,m_SproutSprite,null,PlantState.NONE);
            }

            m_FeverTimeCoroutine = StartCoroutine(FeverTimeCoroutine());
        }
    }

    private IEnumerator FeverTimeCoroutine()
    {
        float max = 15.0f;
        float time = 15.0f;
        float waiting = 0.0f;
        while (true)
        {
            int select = SelectOnePlantObjRandomly();
            int species = SelectOnePlantSpeciesRandomly();

            m_Plants[select].Initialize(select, species, m_SproutSprite, m_PlantSprites[species], PlantState.ADULT);

            waiting = 0.2f + Random.Range(0.2f, 0.4f);
            while (true)
            {
                time -= Time.unscaledDeltaTime;
                if (time <= 0.0f)
                {
                    m_Plants[select].Initialize(select, -1, m_SproutSprite, null, PlantState.NONE);
                    goto EndLoop;
                }

                m_FeverTimeGaugeBar.fillAmount = time / max;
                if ((waiting -= Time.unscaledDeltaTime) <= 0.0f)
                {
                    break;
                }

                yield return null;
            }

            m_Plants[select].Initialize(select, -1, m_SproutSprite, null, PlantState.NONE);
        }
    EndLoop:

        m_FeverCount = 0;
        m_FeverCountText.text = m_FeverCount + "/" + m_FeverCountMax;
        DataManager.SetFeverCount(0);
        m_FeverTimeCoroutine = null;
        m_isFeverOn = false;
        Time.timeScale = 1.0f;
        m_FeverTimeGaugeBar.gameObject.SetActive(false);
        m_FeverTimeGaugeBarBack.gameObject.SetActive(false);
        SpawnSprout();
    }
}
