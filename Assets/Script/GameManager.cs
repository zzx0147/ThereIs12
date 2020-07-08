using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class GameManager : MonoBehaviour
{
    #region Variable
    [SerializeField] private float m_DogPlantRespawnProbability = 50.0f;//개풀 스폰 확률
    [SerializeField] private int m_FeverCountMax = 0;//피버까지 필요한 식물 수확 갯수

    [SerializeField] private int[] m_PlantRespawnTimeMin = { 70, 60, 50, 40, 30 }; //식물 리스폰 속도 최소치
    [SerializeField] private int[] m_PlantRespawnTimeWeight = { 20, 19, 18, 17, 16 };//식물 리스폰 속도 가중치 (최소치 + 가중치 = 최대치)
    [SerializeField] private int[] m_GreenPlantRespawnTimeMin = { 180, 300, 600, 1000, 1800 };//녹초 리스폰 속도 최소치
    [SerializeField] private int[] m_GreenPlantRespawnTimeWeight = { 120, 150, 200, 400, 600 };//녹초 리스폰 속도 가중치

    private int m_NumberOfAvailablePlant;
    private int m_lampGrade;
    private int m_MaxLampGrade;
    private int m_nutrientsGrade;
    private int m_MaxNutrientsGrade;
    private int m_sprinklerGrade;
    private int m_MaxSprinklerGrade;
    private int m_Money;
    private int m_FeverCount;

    private float[] m_PlantRespawnProbability;
    private float m_MaxTime;
    private float m_RemainingTime = 0;

    private string[,] m_PlantCsv;
    private string[,] m_PlantProbabilityCsv;

    private bool m_isTimeLeft = false;
    private bool m_isFeverOn = false;
    #endregion

    #region ReferanceVariable
    private Coroutine m_ChangeGamePanelCoroutine = null;
    private Coroutine m_FeverTimeCoroutine = null;
    private Coroutine m_SpawnGreenPlantCoroutine = null;
    private Sprite[] m_PlantSprites = new Sprite[51];//각 식물의 스프라이트 배열
    private Sprite m_SproutSprite = null;
    private Sprite m_SproutSprite2 = null;

    [SerializeField] private Sprite[] m_LampSprites = null;
    [SerializeField] private Sprite[] m_SprinklerSprites = null;
    [SerializeField] private Sprite[] m_NutrientsSprites = null;

    [SerializeField] private PlantLibraryManager m_plantLibraryManager = null;//도감 매니저 레퍼런스

    [SerializeField] private RectTransform m_MainPanel = null;
    [SerializeField] private RectTransform m_Content = null;//m_GamePanels를 자식으로 하는 Content 패널

    [SerializeField] private Image m_FeverTimeGaugeBar = null;//피버 시간 게이지 바
    [SerializeField] private Image m_FeverTimeGaugeBarBack = null;//피버 시간 게이지 바 배경
    [SerializeField] private Image m_LampImage = null;//램프 이미지
    [SerializeField] private Image m_SprinklerImage = null;//스프링클러 이미지
    [SerializeField] private Image m_NutrientsImage = null;//영양제 이미지

    [SerializeField] private Plant[] m_Plants = null;//식물이 터치되었을 때 게임 매니저로 터치되었다는 걸 알려주는 클래스 식물 이미지와 동일한 오브젝트의 컴포넌트

    [SerializeField] private Text m_MoneyText = null;//돈을 표기하는 텍스트(연구점수)
    [SerializeField] private Text m_FeverCountText = null;//피버 타임까지 필요한 식물 수확 수 표기 텍스트
    [SerializeField] private Text m_TimerText = null;//식물 성장 시간을 표시하는 타이머
    [SerializeField] private Text[] m_PanelChangeButtonText = null;//(메인, 상점, 도감, 설정) 패널로 이동하는 버튼들의 텍스트

    [SerializeField] private Animator[] m_AnimationsOnFeverStart = null;//피버타임 시작시 플레이할 애니메이션들
    [SerializeField] private Animator m_MirrorBallAnimator = null;
    [SerializeField] private Animator m_SprinklerAnimator = null;
    [SerializeField] private Animator m_NutrientsAnimator = null;
    [SerializeField] private GameObject m_MirrorBallEffect = null;
    [SerializeField] private GameObject m_MirrorBallLightEffect = null;

    #endregion

    #region Property
    private bool m_IsTimeLeft
    {
        get => m_isTimeLeft;
        set
        {
            m_isTimeLeft = value;
            if (m_isTimeLeft)
            {
                Time.timeScale = 1.0f;
            }
            else
            {
                Time.timeScale = 0.0f;
            }
        }
    }

    public int m_LampGrade
    {
        get => m_lampGrade;
        set
        {
            m_lampGrade = value;
            DataManager.SetLampGrade(m_lampGrade);
            if (m_lampGrade > m_MaxLampGrade)
            {
                m_MaxLampGrade = m_lampGrade;
                DataManager.SetMaxLampGrade(m_MaxLampGrade);
            }
            m_LampImage.sprite = m_LampSprites[m_lampGrade];
            m_LampImage.SetNativeSize();
        }
    }
    public int m_NutrientsGrade
    {
        get => m_nutrientsGrade;
        set
        {
            m_nutrientsGrade = value;
            DataManager.SetNutrientsGrade(m_nutrientsGrade);
            if (m_nutrientsGrade > m_MaxNutrientsGrade)
            {
                m_MaxNutrientsGrade = m_nutrientsGrade;
                DataManager.SetMaxNutrientsGrade(m_MaxNutrientsGrade);
            }

            m_NutrientsImage.sprite = m_NutrientsSprites[m_nutrientsGrade];
            m_NutrientsImage.SetNativeSize();
        }
    }
    public int m_SprinklerGrade
    {
        get => m_sprinklerGrade;
        set
        {
            m_sprinklerGrade = value;
            DataManager.SetSprinklerGrade(m_sprinklerGrade);
            if (m_sprinklerGrade > m_MaxSprinklerGrade)
            {
                m_MaxSprinklerGrade = m_sprinklerGrade;
                DataManager.SetMaxSprinklerGrade(m_MaxSprinklerGrade);
            }
            m_SprinklerImage.sprite = m_SprinklerSprites[m_sprinklerGrade];
            m_SprinklerImage.SetNativeSize();
        }
    }

    #endregion
    private void Awake()
    {
        //30프레임 제한 해제 60프레임 설정
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        //

        #region CsvLoading
        m_PlantProbabilityCsv = CsvLoader.LoadCsvBy2DimensionArray("Csv/PlantVariableTable");//식물 확률 Csv를 읽어옴
        m_PlantCsv = CsvLoader.LoadCsvBy2DimensionArray("Csv/PlantMasterTable");//Csv의 데이터를 읽어옴
        m_SproutSprite = Resources.Load<Sprite>("Plant/IL_172");
        m_SproutSprite2 = Resources.Load<Sprite>("Plant/IL_174");
        for (int i = 0; i < m_PlantSprites.Length; ++i)
        {
            m_PlantSprites[i] = Resources.Load<Sprite>("Plant/" + m_PlantCsv[i + 1, 2]);
        }
        #endregion

        #region InitVariable and Screen

        m_FeverCount = DataManager.GetFeverCount();
        m_FeverCountText.text = m_FeverCount + "/" + m_FeverCountMax;
        m_Money = DataManager.GetMoney();//돈 저장갑 불러오기
        AddMoney(0);//돈 Text 업데이트
        m_LampGrade = DataManager.GetLampGrade();//램프 등급 불러오기
        m_MaxLampGrade = DataManager.GetMaxLampGrade();//최대 램프 등급 불러오기
        m_SprinklerGrade = DataManager.GetSprinklerGrade();//스프링클러 등급 불러오기
        m_MaxSprinklerGrade = DataManager.GetMaxSprinklerGrade();//최대 스프링클러 등급 불러오기
        m_NutrientsGrade = DataManager.GetNutrientsGrade();//영양제 등급 불러오기
        m_MaxNutrientsGrade = DataManager.GetMaxNutrientsGrade();//최대 영양제 등급 불러오기
        m_MaxTime = DataManager.GetMaxTimeOfLastUsedTimeItem();//마지막으로 사용한 시간 아이템의 최대 시간량
        m_PlantRespawnProbability = new float[51/*m_PlantSprites.Length*/];//식물별 등장 확률 배열
        #endregion
    }

    // Start is called before the first frame update
    private void Start()
    {
        #region Time Compute
        ulong refTime = DataManager.GetReferenceTimeOfTimeItem();//RemainingTimeOfTimeItem을 저장한 시점, 시간 아이템을 구매한 시점을 말함
        ulong now = DataManager.GetNow();//현재 시간
        int pastRemainingTime = DataManager.GetRemainingTimeOfTimeItem();//마지막으로 저장된 시간 아이템의 남은 시간
        m_RemainingTime = pastRemainingTime;
        m_RemainingTime -= (int)(now - refTime);//시간 아이템의 남은 시간을 계산, (now - refTime) 지난 시간을 저장된 남은 시간에서 빼서 남은 시간을 구함

        if (m_RemainingTime < 0)//남은 시간이 0보다 작을 경우 0으로 설정
        {
            m_RemainingTime = 0;
            m_IsTimeLeft = false;
            //StartSpawnGreenPlant();//시간 아이템을 전부 소모하였으니 녹초 생성을 On
        }
        else
        {
            m_IsTimeLeft = true;
        }
        DataManager.SetRemainingTimeOfTimeItem((int)m_RemainingTime);//시간 아이템의 기준 시간과 남은 시간을 업데이트
        DataManager.RecordReferenceTimeOfTimeItem();
        m_TimerText.text = $"{(int)m_RemainingTime / 60:00}" + ":" + $"{(int)m_RemainingTime % 60:00}";//시간 표시

        m_NumberOfAvailablePlant = DataManager.GetNumberOfAvailablePlant();//사용 가능한 식물 칸의 갯수를 얻어옴(재화를 주고 해방함에 따라 갯수가 달라짐)

        #endregion

        #region Probability Compute

        m_PlantRespawnProbability[0] = m_DogPlantRespawnProbability;//개풀(0번 식물)의 확률
        m_PlantRespawnProbability[1] = 0;//녹초(1번)는 자연 스폰이 되지 않음
        float devidedProbability = (50.0f - m_DogPlantRespawnProbability) / 49.0f;//(50 - 개풀 확률)/(개풀 제외 식물 수)로 하여 나온 값
        //개풀 확률을 50보다 낮게 조정할 경우에 줄어든 확률을 다른 식물에게 똑같이 분배

        for (int i = 2; i < m_PlantSprites.Length - 1; ++i)//최종적으로 각 식물 확률을 저장함
        {
            m_PlantRespawnProbability[i] = float.Parse(m_PlantProbabilityCsv[i + 3, 1]) + devidedProbability;
        }

        #endregion

        #region spawn plant between off

        //bool haveSprout = false;//현재 맵에 새싹이 있는지 없는지를 판단하는 플래그
        ulong lastSproutGrowingTime = 0;//게임을 끄기 전 마지막 새싹이 다 자라는 순간의 시간// 재접속시 마지막 새싹이 다 자라지 않았다면 사용하지 않음
        for (int i = 0; i < m_Plants.Length; ++i)
        {
            PlantDataStruct temp = DataManager.GetPlantData(i);
            m_Plants[i].OnEndGrowEvent.RemoveAllListeners();
            m_Plants[i].OnHarvestEvent.RemoveAllListeners();
            m_Plants[i].OnEndGrowEvent.AddListener(SpawnSprout);
            m_Plants[i].OnHarvestEvent.AddListener(GainPlant);

            if (temp.state != PlantState.SPROUT && temp.state != PlantState.SPROUT2)//성체 식물 혹은 아예 없는 경우
            {
                if (temp.state == PlantState.NONE)
                {
                    m_Plants[i].Initialize(i, temp.SpeciesId, m_SproutSprite, m_SproutSprite2,
                        (temp.SpeciesId == -1) ? (null) : (m_PlantSprites[temp.SpeciesId]), temp.state, InteractionMode.NONE);
                }
                else if (temp.SpeciesId == 1)
                {
                    m_Plants[i].Initialize(i, temp.SpeciesId, m_SproutSprite, m_SproutSprite2,
                        (temp.SpeciesId == -1) ? (null) : (m_PlantSprites[temp.SpeciesId]), temp.state, InteractionMode.BUTTON);
                }
                else
                {
                    m_Plants[i].Initialize(i, temp.SpeciesId, m_SproutSprite, m_SproutSprite2,
                        (temp.SpeciesId == -1) ? (null) : (m_PlantSprites[temp.SpeciesId]), temp.state, InteractionMode.DRAG);
                }

            }
            else//새싹SPROUT,SPROUT2인 경우
            {
                if ((ulong)temp.RemainingTime < (now - temp.ReferenceTime))//새싹의 성장까지 남은 시간이 흐른 시간보다 적다(이미 성장했어야 하는 시간이다)
                {
                    m_Plants[i].Initialize(i, temp.SpeciesId, m_SproutSprite, m_SproutSprite2, m_PlantSprites[temp.SpeciesId], PlantState.ADULT, InteractionMode.DRAG);//새싹으로 저장되어 있지만 이미 성장할 시간이 지났으므로 성체 식물로 초기화한다
                    lastSproutGrowingTime = temp.ReferenceTime + (ulong)temp.RemainingTime;
                }
                else//새싹의 성장까지 남은 시간이 흐른 시간보다 많다(아직 성장하지 않았을 시간이다)
                    //이 말은 게임 종료 후 다시 접속했을 때 마지막 새싹이 아직 자라지 않았음을 의미
                    //즉 지난 시간을 계산해 식물을 소환할 필요가 없음을 의미
                {
                    m_Plants[i].Initialize(i, temp.SpeciesId, m_SproutSprite, m_SproutSprite2, m_PlantSprites[temp.SpeciesId], PlantState.SPROUT, InteractionMode.BUTTON);
                    m_Plants[i].StartGrowing(temp.MaxTime, (temp.RemainingTime - (int)(now - temp.ReferenceTime)), temp.SpeciesId, m_PlantSprites[temp.SpeciesId]);
                    //haveSprout = true;
                }
            }
        }


        if (lastSproutGrowingTime > 0)//끄기 전 마지막 새싹이 다 자랐을 경우
        {
            RespawnPlantBetweenTurnOff((m_IsTimeLeft) ? (int)(now - lastSproutGrowingTime) : pastRemainingTime);
        }

        if (!m_IsTimeLeft)
        {
            RespawnGreenPlantBetweenTurnOff((int)(now - ((ulong)pastRemainingTime + refTime)));//기준 시간 + 시간 아이템 남은 시간 = 시간 아이템이 끝나는 시간
                                                                                               //시간 아이템이 끝나는 시간과 현재 시간 사이의 차 만큼 녹초를 생성
        }
        //else if (!haveSprout)
        //{
        //    SpawnSprout();
        //}

        #endregion
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_IsTimeLeft)
        {
            m_RemainingTime -= Time.deltaTime;

            m_TimerText.text = $"{(int)m_RemainingTime / 60:00}" + ":" + $"{(int)m_RemainingTime % 60:00}";

            if (m_RemainingTime < 0.0f)
            {
                m_TimerText.text = "00:00";
                m_IsTimeLeft = false;
                StartSpawnGreenPlant();
            }
        }
    }

    private void RespawnPlantBetweenTurnOff(int time)
    {
        while (true)
        {
            int randTime = Random.Range(m_PlantRespawnTimeMin[m_MaxLampGrade], m_PlantRespawnTimeMin[m_MaxLampGrade] + m_PlantRespawnTimeWeight[m_MaxLampGrade]);
            if ((time - randTime) >= 0)//주어진 시간(게임이 꺼진 동안 지난 시간이면서 남아있던 시간아이템 시간)이 여유가 있으면
            {
                SpawnRandomAdultPlant();//성체 식물을 하나 스폰
                time -= randTime;
            }
            else//아니라면
            {
                int sel = SelectOnePlantObjRandomly();
                if (sel < 0)//식물 칸이 꽉 참
                {
                    return;
                }
                int plantspecies = SelectOnePlantSpeciesRandomly();
                m_Plants[sel].StartGrowing(randTime, randTime - time, plantspecies, m_PlantSprites[plantspecies]);
                return;
            }
        }
    }

    private bool SpawnRandomAdultPlant()
    {
        int plantObjNum = SelectOnePlantObjRandomly();
        if (plantObjNum < 0)
        {
            return false;
        }

        int plantSpeciesNum = SelectOnePlantSpeciesRandomly();

        m_Plants[plantObjNum].SetPlant(plantSpeciesNum, m_PlantSprites[plantSpeciesNum], PlantState.ADULT, AnimationType.NONE, InteractionMode.DRAG);
        DataManager.SetPlantData(plantObjNum, plantSpeciesNum, PlantState.ADULT, 0, 0, 0);

        return true;
    }

    private int[] GetEmptyPlantImageNumbers()
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

    private int[] GetNotEmptyPlantImageNumbers()
    {
        List<int> temp = new List<int>();

        for (int i = 0; i < m_NumberOfAvailablePlant; ++i)
        {
            if (m_Plants[i].m_State == PlantState.ADULT && m_Plants[i].m_PlantSpeciesId != 1)//개풀이 아니면서 성체인 경우
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

    public bool AddMoney(int price)
    {
        if (m_Money + price < 0)
        {
            return false;
        }

        m_Money += price;
        m_MoneyText.text = m_Money.ToString();
        DataManager.SetMoney(m_Money);
        return true;
    }

    public void MoneyCheat()
    {
        AddMoney(999999);
    }

    public void BuyTimeItem(int time, int price)
    {
        if (UseMoney(price))
        {
            m_MaxTime = time;
            m_RemainingTime = time;
            m_IsTimeLeft = true;
            StopSpawnGreenPlantCoroutine();
            DataManager.RecordReferenceTimeOfTimeItem();
            DataManager.SetRemainingTimeOfTimeItem((int)m_RemainingTime);
            DataManager.SetMaxTimeOfLastUsedTimeItem((int)m_RemainingTime);

            foreach (var v in m_Plants)
            {
                if (v.m_State == PlantState.SPROUT || v.m_State == PlantState.SPROUT2)
                {
                    return;
                }
            }
            SpawnSprout();
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
                time = 18000;
                price = 20;
                break;
            case 4:
                time = 36000;
                price = 65;
                break;
        }
        BuyTimeItem(time, price);

    }

    public void GainPlant(int speciesId)
    {
        AddMoney(int.Parse(m_PlantCsv[speciesId + 1, 6]));
        m_plantLibraryManager.OnGainPlant(speciesId);

        if ((!m_isFeverOn) && speciesId != 1)
        {
            m_FeverCountText.text = ++m_FeverCount + "/" + m_FeverCountMax;
            DataManager.SetFeverCount(m_FeverCount);

            if (m_FeverCount >= m_FeverCountMax)
            {
                m_FeverCount = 0;
                //Debug.Log("Ready to Fever");
                StartFeverTime();
                return;
            }
        }

        if (!CheckHaveSprout() && !m_isFeverOn && m_IsTimeLeft)
        {
            SpawnSprout();
        }
    }

    private bool CheckHaveSprout()
    {
        foreach (var v in m_Plants)
        {
            if (v.m_State == PlantState.SPROUT || v.m_State == PlantState.SPROUT2)
            {
                return true;
            }
        }
        return false;
    }

    public void SpawnSprout()
    {

        int randTime = Random.Range(m_PlantRespawnTimeMin[m_MaxLampGrade], m_PlantRespawnTimeMin[m_MaxLampGrade] + m_PlantRespawnTimeWeight[m_MaxLampGrade]);
        //Debug.Log("randTime: " + randTime);
        int selecObj = SelectOnePlantObjRandomly();

        if (selecObj == -1)
        {
            return;
        }

        int species = SelectOnePlantSpeciesRandomly();

        m_Plants[selecObj].StartGrowing(randTime, randTime, species, m_PlantSprites[species]);
    }

    private int SelectOnePlantObjRandomly()
    {
        int[] EmptyPlantObjectsIndexes = GetEmptyPlantImageNumbers();
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
            for (int i = 0; i < (m_MaxNutrientsGrade + 1) * 10 + 1; ++i)//현재 장비 등급이 허용하는 범위 내의 식물을 순회하며 확률에 걸렸는지 체크
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

        foreach (var v in m_PanelChangeButtonText)
        {
            v.color = new Color(103.0f / 255.0f, 65 / 255.0f, 41 / 255.0f, 1.0f);
        }
        m_PanelChangeButtonText[num].color = new Color(233.0f / 255.0f, 230.0f / 255.0f, 223.0f / 255.0f, 1.0f);

        m_ChangeGamePanelCoroutine = StartCoroutine(ChangeGamePanelCoroutine(num));
    }

    private IEnumerator ChangeGamePanelCoroutine(int num)//게임 패널을 변경하는 코루틴//메인화면은 0번 상점은 1번 도감은 2번 설정은 3번
    {
        int screenwidth = 2 * (int)m_MainPanel.localPosition.x;
        int targetPositionX = -(screenwidth * num);
        //Debug.Log("Start");
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
                m_Plants[i].SetPlant(-1, null, PlantState.NONE, AnimationType.NONE, InteractionMode.NONE);
            }


            foreach (var v in m_AnimationsOnFeverStart)
            {
                v.SetFloat("Speed", 1.0f);
                v.Play("FeverStart", -1, 0f);
            }
            m_MirrorBallAnimator.SetFloat("Speed", 1.0f);
            m_MirrorBallAnimator.Play("FeverStart", -1, 0.0f);
            m_FeverTimeCoroutine = StartCoroutine(FeverTimeCoroutine());
        }
    }

    private IEnumerator FeverTimeCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        m_MirrorBallAnimator.Play("Fever", -1, 0.0f);
        m_MirrorBallEffect.SetActive(true);
        m_MirrorBallLightEffect.SetActive(true);
        m_SprinklerAnimator.Play("Fever", -1, 0.0f);
        m_NutrientsAnimator.Play("Fever", -1, 0.0f);
        //RuntimeManager.StudioSystem.setParameterByName("Main_BGM_Tempo", 1.0f);


        float max = 15.0f;
        float time = 15.0f;
        float waiting = 0.0f;
        while (true)
        {
            int select = SelectOnePlantObjRandomly();
            int species = SelectOnePlantSpeciesRandomly();

            //m_Plants[select].Initialize(select, species, m_SproutSprite, m_SproutSprite2, m_PlantSprites[species], PlantState.ADULT);
            m_Plants[select].SetPlant(species, m_PlantSprites[species], PlantState.ADULT, AnimationType.FEVER, InteractionMode.BUTTON);

            waiting = 0.2f + Random.Range(0.2f, 0.4f);
            while (true)
            {
                time -= Time.unscaledDeltaTime;
                if (time <= 0.0f)
                {
                    //m_Plants[select].Initialize(select, -1, m_SproutSprite, m_SproutSprite2, null, PlantState.NONE);
                    m_Plants[select].SetPlant(-1, null, PlantState.NONE, AnimationType.NONE, InteractionMode.NONE);//종료하기 전 마지막 식물 제거
                    goto EndLoop;
                }

                m_FeverTimeGaugeBar.fillAmount = time / max;
                if ((waiting -= Time.unscaledDeltaTime) <= 0.0f)
                {
                    break;
                }

                yield return null;
            }

            //m_Plants[select].Initialize(select, -1, m_SproutSprite, m_SproutSprite2, null, PlantState.NONE);
            if(!m_Plants[select].m_IsOnHarvesting)
            {
                m_Plants[select].SetPlant(-1, null, PlantState.NONE, AnimationType.NONE, InteractionMode.NONE);
            }
        }
    EndLoop:

        m_FeverCount = 0;
        m_FeverCountText.text = m_FeverCount + "/" + m_FeverCountMax;
        DataManager.SetFeverCount(0);
        m_FeverTimeCoroutine = null;
        m_isFeverOn = false;
        m_FeverTimeGaugeBar.gameObject.SetActive(false);
        m_FeverTimeGaugeBarBack.gameObject.SetActive(false);

        foreach (var v in m_AnimationsOnFeverStart)
        {
            v.SetFloat("Speed", -1.0f);
            v.Play("FeverStart", -1, 1.0f);
        }
        m_MirrorBallAnimator.SetFloat("Speed", -1.0f);
        m_MirrorBallAnimator.Play("FeverStart", -1, 1.0f);
        m_MirrorBallEffect.SetActive(false);
        m_MirrorBallLightEffect.SetActive(false);
        m_SprinklerAnimator.Play("Idle", -1, 0.0f);
        m_NutrientsAnimator.Play("Idle", -1, 0.0f);
        RuntimeManager.StudioSystem.setParameterByName("Main_BGM_Tempo", 0.0f);

        yield return new WaitForSecondsRealtime(1.0f);
        if (m_isTimeLeft)
        {
            Time.timeScale = 1.0f;
            SpawnSprout();
        }
    }

    private void StartSpawnGreenPlant(int time = 0)//time은 최초 1회에 한해 녹초의 생성 시간을 단축시키는 변수, 꺼져있는 동안 녹초를 생성시키고 남은 시간만큼 다음 녹초의 생성 시간을 앞당기는 용도
    {
        if (m_SpawnGreenPlantCoroutine == null)
        {
            m_SpawnGreenPlantCoroutine = StartCoroutine(SpawnGreenPlantCoroutine(time));
        }
        else
        {
            //Debug.LogError("Already SpawnGreenPlant is in Operating");
        }
    }

    private IEnumerator SpawnGreenPlantCoroutine(int time)
    {
        int nextGreenPlantSpawnTime = Random.Range(m_GreenPlantRespawnTimeMin[m_sprinklerGrade], m_GreenPlantRespawnTimeMin[m_sprinklerGrade] + m_GreenPlantRespawnTimeWeight[m_sprinklerGrade]);

        int temp = nextGreenPlantSpawnTime - time;
        if (temp < 0)
        {
            temp = 0;
        }

        yield return new WaitForSecondsRealtime(temp);

        while(true)
        {
            if(m_FeverTimeCoroutine == null)
            {
                break;
            }
            yield return null;
        }

        SpawnRandomGreenPlant(true);

        while (true)
        {
            yield return new WaitForSecondsRealtime(Random.Range(m_GreenPlantRespawnTimeMin[m_sprinklerGrade], m_GreenPlantRespawnTimeMin[m_sprinklerGrade] + m_GreenPlantRespawnTimeWeight[m_sprinklerGrade]));
            if(m_FeverTimeCoroutine != null)
            {
                continue;
            }
            SpawnRandomGreenPlant(true);
        }
    }

    private void StopSpawnGreenPlantCoroutine()
    {
        if (m_SpawnGreenPlantCoroutine != null)
        {
            StopCoroutine(m_SpawnGreenPlantCoroutine);
            m_SpawnGreenPlantCoroutine = null;
        }
    }

    private void SpawnRandomGreenPlant(bool isAnimated)
    {
        int[] arr = GetNotEmptyPlantImageNumbers();
        if (arr.Length <= 0)
        {
            return;
        }
        int sel = Random.Range(0, arr.Length);

        m_Plants[arr[sel]].SetPlant(1, m_PlantSprites[1], PlantState.ADULT, (isAnimated) ? (AnimationType.DECAY) : (AnimationType.NONE), InteractionMode.BUTTON);
    }

    private void RespawnGreenPlantBetweenTurnOff(int time)
    {
        while (true)
        {
            int randTime = Random.Range(m_GreenPlantRespawnTimeMin[m_sprinklerGrade], m_GreenPlantRespawnTimeMin[m_sprinklerGrade] + m_GreenPlantRespawnTimeWeight[m_sprinklerGrade]);
            if (time - randTime >= 0)
            {
                time -= randTime;
                SpawnRandomGreenPlant(false);
            }
            else
            {
                StartSpawnGreenPlant((randTime - time));
                return;
            }
        }
    }
}
