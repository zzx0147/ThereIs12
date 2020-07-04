using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public struct PlantDataStruct
{
    public int SpeciesId;//식물의 종류 id 새싹의 경우 자랐을 때 무슨 종이 될 지를 결정
    public PlantState state;//식물의 상태, NONE이면 , SPROUT면, ADULT면 식물이 다 자람
    public int MaxTime;//식물이 성체가 되기까지 필요한(최대치) 시간
    public int RemainingTime;//식물이 성체가 되기까지 필요한(남은) 시간
    public ulong ReferenceTime;//RemainingTime이 기록된 시간, RemainingTime은 이 시간을 기준으로 얼마나 남았는지를 기록하게 됨
}

public enum PlantState
{
    NONE,// 식물 없음
    SPROUT,//새싹, 식물이 자라는 중
    SPROUT2,
    ADULT//성체, 식물이 다 자라서 수확할 수 있는 상태
}

public enum AnimationType
{
    GROWUP,
    DECAY,
    FEVER,
    NONE
}

class OnHarvestEvent : UnityEvent<int>
{
}

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class Plant : MonoBehaviour
{
    private PlantState m_state;

    private int m_plantSpeciesId;
    private int m_plantObjId;

    private float m_RemaingTime2Grow;
    private int m_MaxTime2Grow;

    private Coroutine m_GrowCoroutine = null;

    private Image m_PlantImage = null;
    private Sprite m_AdultPlantSprite = null;
    private Sprite m_SproutSprite = null;
    private Sprite m_SproutSprite2 = null;
    private Button m_Button = null;

    private UnityEvent<int> m_onHarvestEvent = null;
    private UnityEvent m_onEndGrowEvent = null;

    public PlantState m_State
    {
        get => m_state;
        set
        {
            m_state = value;
            switch (m_state)
            {
                case PlantState.NONE:
                    m_PlantImage.enabled = false;
                    m_Button.interactable = false;
                    if(m_GrowCoroutine != null)
                    {
                        StopCoroutine(m_GrowCoroutine);
                        m_GrowCoroutine = null;
                    }

                    DataManager.SetPlantData(m_plantObjId, -1, PlantState.NONE,0, 0, 0);
                    break;

                case PlantState.SPROUT:
                    if(m_plantSpeciesId == -1)
                    {
                        Debug.LogError("Critical Error");
                    }
                    m_PlantImage.enabled = true;
                    m_Button.interactable = true;
                    m_PlantImage.sprite = m_SproutSprite;
                    m_PlantImage.SetNativeSize();
                    DataManager.SetPlantData(m_plantObjId, m_plantSpeciesId, PlantState.SPROUT,m_MaxTime2Grow, (int)m_RemaingTime2Grow, DataManager.GetNow());
                    break;

                case PlantState.SPROUT2:
                    if (m_plantSpeciesId == -1)
                    {
                        Debug.LogError("Critical Error");
                    }
                    m_PlantImage.enabled = true;
                    m_Button.interactable = true;
                    m_PlantImage.sprite = m_SproutSprite2;
                    m_PlantImage.SetNativeSize();
                    DataManager.SetPlantData(m_plantObjId, m_plantSpeciesId, PlantState.SPROUT2,m_MaxTime2Grow, (int)m_RemaingTime2Grow, DataManager.GetNow());

                    break;
                case PlantState.ADULT:
                    m_PlantImage.enabled = true;
                    m_Button.interactable = true;
                    m_PlantImage.sprite = m_AdultPlantSprite;
                    m_PlantImage.SetNativeSize();
                    DataManager.SetPlantData(m_plantObjId, m_plantSpeciesId, PlantState.ADULT, 0,0, 0);
                    break;
            }
        }
    }

    public int m_PlantSpeciesId { get => m_plantSpeciesId; }
    public UnityEvent OnEndGrowEvent { get => m_onEndGrowEvent; set => m_onEndGrowEvent = value; }
    public UnityEvent<int> OnHarvestEvent { get => m_onHarvestEvent; set => m_onHarvestEvent = value; }

    private void Awake()
    {
        m_PlantImage = GetComponent<Image>();
        m_Button = GetComponent<Button>();
        m_Button.onClick.AddListener(OnTouched);
        m_onHarvestEvent = new OnHarvestEvent();
        m_onEndGrowEvent = new UnityEvent();
    }

    public void Initialize(int objId, int speciesId, Sprite sproutSprite,Sprite sproutSprite2, Sprite adultSprite, PlantState state)
    {
        m_plantObjId = objId;
        m_SproutSprite = sproutSprite;
        m_SproutSprite2 = sproutSprite2;
        m_AdultPlantSprite = adultSprite;
        m_plantSpeciesId = speciesId;
        m_State = state;
        float scaling = Random.Range(0.9f, 1.05f);
        GetComponent<RectTransform>().localScale = new Vector3(scaling, scaling, 1); 
    }

    public void SetPlant(int speciesId,Sprite adultSprite,PlantState state,AnimationType animationType)
    {
        Debug.Log("StartGrowupAnimation");
        m_plantSpeciesId = speciesId;
        m_AdultPlantSprite = adultSprite;
        switch(animationType)
        {
            case AnimationType.NONE:
                m_State = state;
                float scaling = Random.Range(0.9f, 1.05f);
                GetComponent<RectTransform>().localScale = new Vector3(scaling, scaling, 1);
                break;
            case AnimationType.DECAY:
                StartCoroutine(DecayAnimationCoroutine());
                break;
            case AnimationType.FEVER:
                m_State = state;
                break;
            case AnimationType.GROWUP:
                m_State = state;
                break;
        }
    }

    public void StartGrowing(int maxtime,int time, int plantSpeciesID, Sprite adultSprite)
    {
        Debug.Log(maxtime);
        m_MaxTime2Grow = maxtime;
        m_RemaingTime2Grow = time;
        m_plantSpeciesId = plantSpeciesID;
        m_State = PlantState.SPROUT;
        m_AdultPlantSprite = adultSprite;


        StartCoroutine(GrowUpAnimationCoroutine());
        //float scaling = Random.Range(0.9f, 1.05f);
        //GetComponent<RectTransform>().localScale = new Vector3(scaling, scaling, 1);
        m_GrowCoroutine = StartCoroutine(GrowCoroutine());
    }

    private IEnumerator GrowCoroutine()
    {
        while(true)
        {
            m_RemaingTime2Grow -= Time.deltaTime;
            if (m_RemaingTime2Grow < (m_MaxTime2Grow / 2))
            {
                m_State = PlantState.SPROUT2;
                StartCoroutine(GrowUpAnimationCoroutine());
                //float scaling = Random.Range(0.9f, 1.05f);
                //GetComponent<RectTransform>().localScale = new Vector3(scaling, scaling, 1);
                break;
            }
            yield return null;
        }

        while (true)
        {
            m_RemaingTime2Grow -= Time.deltaTime;
            if (m_RemaingTime2Grow < 0)
            {
                m_State = PlantState.ADULT;
                StartCoroutine(GrowUpAnimationCoroutine());
                //float scaling = Random.Range(0.9f, 1.05f);
                //GetComponent<RectTransform>().localScale = new Vector3(scaling, scaling, 1);
                m_onEndGrowEvent.Invoke();
                m_GrowCoroutine = null;
                break;
            }
            yield return null;
        }
    }

    private IEnumerator GrowUpAnimationCoroutine()
    {
        m_Button.interactable = false;
        float targetScale = Random.Range(0.9f, 1.05f);
        RectTransform rectTransform  = GetComponent<RectTransform>();
        float scaler = 0.0f;
        while (true)
        {
            scaler += Time.unscaledDeltaTime;

            float temp = Mathf.Lerp(0.0f, targetScale,scaler / 0.15f);
            rectTransform.localScale = new Vector3(temp, temp, 1.0f);

            if(scaler / 0.15f > 1.0f)
            {
                rectTransform.localScale = new Vector3(targetScale, targetScale, 1.0f);
                break;
            }

            yield return null;
        }
        m_Button.interactable = true;
    }

    private IEnumerator DecayAnimationCoroutine()
    {
        m_Button.interactable = false;
        float scaler = 0.075f;

        while(true)
        {
            scaler -= Time.unscaledDeltaTime;
            float temp = Mathf.Lerp(0.0f, 1.0f, scaler / 0.075f);
            Debug.Log(temp);
            m_PlantImage.color = new Color(1.0f, 1.0f, 1.0f, temp);

            if(scaler / 0.075f < 0.0f)
            {
                m_PlantImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                break;
            }

            yield return null;
        }

        scaler = 0.0f;
        m_PlantImage.sprite = m_AdultPlantSprite;
        m_PlantImage.SetNativeSize();

        while (true)
        {
            scaler += Time.unscaledDeltaTime;
            float temp = Mathf.Lerp(0.0f, 1.0f, scaler / 0.075f);
            m_PlantImage.color = new Color(1.0f, 1.0f, 1.0f, temp);
            Debug.Log(m_plantObjId + ":"+temp);

            if (scaler / 0.075f > 1.0f)
            {
                m_PlantImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                break;
            }
            yield return null;
        }
        m_Button.interactable = true;
    }

    public void OnTouched()
    {
        switch (m_State)
        {
            case PlantState.ADULT:
                int temp = m_plantSpeciesId;
                m_State = PlantState.NONE;
                m_onHarvestEvent.Invoke(temp);
                break;

            case PlantState.SPROUT:
            case PlantState.SPROUT2:
                if (Time.timeScale >= 1.0f)
                {
                    m_RemaingTime2Grow -= 1.0f;
                }
                break;
        }
    }
}
