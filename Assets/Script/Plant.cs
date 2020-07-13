using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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

public enum InteractionMode
{
    BUTTON,
    DRAG,
    NONE,
}

class OnHarvestEvent : UnityEvent<int>
{
}

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class Plant : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] AudioClip m_SproutTouchSFX = null;
    [SerializeField] AudioClip m_PlantHarvestSFX = null;
    [SerializeField] AudioClip m_GreenPlantHarvestSFX = null;
    [SerializeField] AudioSource m_AudioSource = null;

    private Vector2 BeginDragPos;
    private bool m_DragEnabled = true;
    private bool m_isOnHarvesting = false;

    private int m_plantSpeciesId;
    private int m_plantObjId;
    private int m_MaxTime2Grow;
    private float m_RemaingTime2Grow;

    private PlantState m_state;
    private InteractionMode m_interactionMode;

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
                    if (m_GrowCoroutine != null)
                    {
                        StopCoroutine(m_GrowCoroutine);
                        m_GrowCoroutine = null;
                    }

                    DataManager.SetPlantData(m_plantObjId, -1, PlantState.NONE, 0, 0, 0);
                    break;

                case PlantState.SPROUT:
                    if (m_plantSpeciesId == -1)
                    {
                        //Debug.LogError("Critical Error");
                    }
                    m_PlantImage.enabled = true;
                    m_PlantImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    m_PlantImage.sprite = m_SproutSprite;
                    m_PlantImage.SetNativeSize();
                    DataManager.SetPlantData(m_plantObjId, m_plantSpeciesId, PlantState.SPROUT, m_MaxTime2Grow, (int)m_RemaingTime2Grow, DataManager.GetNow());
                    break;

                case PlantState.SPROUT2:
                    if (m_plantSpeciesId == -1)
                    {
                        //Debug.LogError("Critical Error");
                    }
                    m_PlantImage.enabled = true;
                    m_PlantImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    m_PlantImage.sprite = m_SproutSprite2;
                    m_PlantImage.SetNativeSize();
                    DataManager.SetPlantData(m_plantObjId, m_plantSpeciesId, PlantState.SPROUT2, m_MaxTime2Grow, (int)m_RemaingTime2Grow, DataManager.GetNow());

                    break;
                case PlantState.ADULT:
                    m_PlantImage.enabled = true;
                    m_PlantImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    m_PlantImage.sprite = m_AdultPlantSprite;
                    m_PlantImage.SetNativeSize();
                    DataManager.SetPlantData(m_plantObjId, m_plantSpeciesId, PlantState.ADULT, 0, 0, 0);
                    break;
            }
        }
    }

    public InteractionMode m_InteractionMode
    {
        get => m_interactionMode;

        set
        {
            m_interactionMode = value;
            switch (m_interactionMode)
            {
                case InteractionMode.BUTTON:
                    m_Button.interactable = true;
                    m_DragEnabled = false;
                    break;

                case InteractionMode.DRAG:
                    m_Button.interactable = false;
                    m_DragEnabled = true;
                    break;

                case InteractionMode.NONE:
                    m_Button.interactable = false;
                    m_DragEnabled = false;
                    break;
            }
        }
    }

    public int m_PlantSpeciesId { get => m_plantSpeciesId; }
    public UnityEvent OnEndGrowEvent { get => m_onEndGrowEvent; set => m_onEndGrowEvent = value; }
    public UnityEvent<int> OnHarvestEvent { get => m_onHarvestEvent; set => m_onHarvestEvent = value; }
    public bool m_IsOnHarvesting { get => m_isOnHarvesting; }

    private void Awake()
    {
        m_PlantImage = GetComponent<Image>();
        m_Button = GetComponent<Button>();
        m_Button.onClick.RemoveAllListeners();
        //m_Button.onClick.AddListener(OnTouched);//alreay added on Inspector
        m_onHarvestEvent = new OnHarvestEvent();
        m_onEndGrowEvent = new UnityEvent();
    }

    public void Initialize(int objId, int speciesId, Sprite sproutSprite, Sprite sproutSprite2, Sprite adultSprite, PlantState state, InteractionMode mode)
    {
        m_plantObjId = objId;
        m_SproutSprite = sproutSprite;
        m_SproutSprite2 = sproutSprite2;
        m_AdultPlantSprite = adultSprite;
        m_plantSpeciesId = speciesId;
        m_State = state;
        m_InteractionMode = mode;
        float scaling = Random.Range(0.9f, 1.05f);
        GetComponent<RectTransform>().localScale = new Vector3(scaling, scaling, 1);
    }

    public void SetPlant(int speciesId, Sprite adultSprite, PlantState state, AnimationType animationType)
    {
        //Debug.Log("StartGrowupAnimation");
        m_plantSpeciesId = speciesId;
        m_AdultPlantSprite = adultSprite;
        switch (animationType)
        {
            case AnimationType.NONE:
                if(state == PlantState.NONE)
                {
                    m_InteractionMode = InteractionMode.NONE;
                }
                else if(state == PlantState.ADULT && m_plantSpeciesId != 1)
                {
                    m_InteractionMode = InteractionMode.DRAG;
                }
                else if(state == PlantState.ADULT && m_plantSpeciesId == 1)
                {
                    m_InteractionMode = InteractionMode.BUTTON;
                }
                m_State = state;
                float scaling = Random.Range(0.9f, 1.05f);
                GetComponent<RectTransform>().localScale = new Vector3(scaling, scaling, 1);
                break;

            case AnimationType.DECAY:
                StartCoroutine(DecayAnimationCoroutine());
                break;

            case AnimationType.FEVER:
                if(state == PlantState.NONE)
                {
                    StartCoroutine(FeverTimePlantRemoveAnimationCoroutine());
                }
                else if(state == PlantState.ADULT)
                {
                    StartCoroutine(FeverTimePlantGrowAnimationCoroutine());
                }
                break;
            case AnimationType.GROWUP:
                m_State = state;
                break;
        }
    }

    public void StartGrowing(int maxtime, int time, int plantSpeciesID, Sprite adultSprite)
    {
        //Debug.Log(maxtime);
        m_MaxTime2Grow = maxtime;
        m_RemaingTime2Grow = time;
        m_plantSpeciesId = plantSpeciesID;
        m_State = PlantState.SPROUT;
        //m_InteractionMode = InteractionMode.BUTTON;
        m_AdultPlantSprite = adultSprite;

        StartCoroutine(GrowUpAnimationCoroutine(InteractionMode.BUTTON));
        //float scaling = Random.Range(0.9f, 1.05f);
        //GetComponent<RectTransform>().localScale = new Vector3(scaling, scaling, 1);
        m_GrowCoroutine = StartCoroutine(GrowCoroutine());
    }

    private IEnumerator GrowCoroutine()
    {
        while (true)
        {
            m_RemaingTime2Grow -= Time.deltaTime;
            if (m_RemaingTime2Grow < (m_MaxTime2Grow / 2))
            {
                m_State = PlantState.SPROUT2;
                StartCoroutine(GrowUpAnimationCoroutine(InteractionMode.BUTTON));
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
                StartCoroutine(GrowUpAnimationCoroutine(InteractionMode.DRAG));
                //float scaling = Random.Range(0.9f, 1.05f);
                //GetComponent<RectTransform>().localScale = new Vector3(scaling, scaling, 1);
                m_onEndGrowEvent.Invoke();
                m_GrowCoroutine = null;
                break;
            }
            yield return null;
        }
    }

    private IEnumerator GrowUpAnimationCoroutine(InteractionMode mode)
    {
        m_InteractionMode = InteractionMode.NONE;
        float targetScale = Random.Range(0.9f, 1.05f);
        RectTransform rectTransform = GetComponent<RectTransform>();
        float scaler = 0.0f;
        while (true)
        {
            scaler += Time.unscaledDeltaTime;

            float temp = Mathf.Lerp(0.0f, targetScale, scaler / 0.15f);
            rectTransform.localScale = new Vector3(temp, temp, 1.0f);

            if (scaler / 0.15f > 1.0f)
            {
                rectTransform.localScale = new Vector3(targetScale, targetScale, 1.0f);
                break;
            }

            yield return null;
        }
        m_InteractionMode = mode;
    }

    private IEnumerator DecayAnimationCoroutine()
    {
        m_InteractionMode = InteractionMode.NONE;
        float scaler = 0.075f;

        while (true)
        {
            scaler -= Time.unscaledDeltaTime;
            float temp = Mathf.Lerp(0.0f, 1.0f, scaler / 0.075f);
            //Debug.Log(temp);
            m_PlantImage.color = new Color(1.0f, 1.0f, 1.0f, temp);

            if (scaler / 0.075f < 0.0f)
            {
                m_PlantImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                break;
            }

            yield return null;
        }

        scaler = 0.0f;
        m_State = PlantState.ADULT;

        while (true)
        {
            scaler += Time.unscaledDeltaTime;
            float temp = Mathf.Lerp(0.0f, 1.0f, scaler / 0.075f);
            m_PlantImage.color = new Color(1.0f, 1.0f, 1.0f, temp);
            //Debug.Log(m_plantObjId + ":" + temp);

            if (scaler / 0.075f > 1.0f)
            {
                m_PlantImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                break;
            }
            yield return null;
        }
        m_InteractionMode = InteractionMode.BUTTON;
    }

    public void OnTouched()
    {
        switch (m_State)
        {
            case PlantState.ADULT:
                if (m_plantSpeciesId == 1)
                {
                    //Debug.Log("GreenPlantHarvested");
                    m_InteractionMode = InteractionMode.NONE;
                    StartCoroutine(GreenPlantHarvestAnimationCoroutine());
                }
                else
                {
                    m_InteractionMode = InteractionMode.NONE;
                    //Debug.Log("PlantHarvested");
                    StartCoroutine(PlantHarvestAnimationCoroutine());
                }
                break;

            case PlantState.SPROUT:
            case PlantState.SPROUT2:
                if (Time.timeScale >= 1.0f)
                {
                    m_AudioSource.clip = m_SproutTouchSFX;
                    m_AudioSource.Play();
                    m_RemaingTime2Grow -= 1.0f;
                }
                break;
        }
    }

    private IEnumerator FeverTimePlantGrowAnimationCoroutine()
    {
        Debug.Log("StartFeverGrow");
        m_State = PlantState.ADULT;
        float imageYSize = m_PlantImage.rectTransform.sizeDelta.y;
        float time = 0.0f;
        float TargetTime = 0.2f;
        float StartYPos = m_PlantImage.rectTransform.anchoredPosition.y - imageYSize;
        float TargetYPos = m_PlantImage.rectTransform.anchoredPosition.y;
        float nowYPos;

        m_PlantImage.rectTransform.anchoredPosition = new Vector2(m_PlantImage.rectTransform.anchoredPosition.x,StartYPos);

        while (true)
        {
            yield return null;
            time += Time.unscaledDeltaTime;
            if (time >= TargetTime)
            {
                m_PlantImage.rectTransform.anchoredPosition = new Vector2(m_PlantImage.rectTransform.anchoredPosition.x, TargetYPos);
                m_InteractionMode = InteractionMode.BUTTON;
                break;
            }
            nowYPos = Mathf.Lerp(StartYPos, TargetYPos, time / TargetTime);
            m_PlantImage.rectTransform.anchoredPosition = new Vector2(m_PlantImage.rectTransform.anchoredPosition.x,nowYPos);
        }
    }

    private IEnumerator FeverTimePlantRemoveAnimationCoroutine()
    {
        m_InteractionMode = InteractionMode.NONE;
        Debug.Log("StartFeverGrow");
        float imageYSize = m_PlantImage.rectTransform.sizeDelta.y;
        float time = 0.0f;
        float TargetTime = 0.2f;
        float StartYPos = m_PlantImage.rectTransform.anchoredPosition.y;
        float TargetYPos = m_PlantImage.rectTransform.anchoredPosition.y - imageYSize;
        float nowYPos;

        m_PlantImage.rectTransform.anchoredPosition = new Vector2(m_PlantImage.rectTransform.anchoredPosition.x, StartYPos);

        while (true)
        {
            yield return null;
            time += Time.unscaledDeltaTime;
            if (time >= TargetTime)
            {
                m_PlantImage.rectTransform.anchoredPosition = new Vector2(m_PlantImage.rectTransform.anchoredPosition.x, StartYPos);
                m_State = PlantState.NONE;
                break;
            }
            nowYPos = Mathf.Lerp(StartYPos, TargetYPos, time / TargetTime);
            m_PlantImage.rectTransform.anchoredPosition = new Vector2(m_PlantImage.rectTransform.anchoredPosition.x, nowYPos);
        }
        yield return null;
    }

    private IEnumerator PlantHarvestAnimationCoroutine()
    {
        m_AudioSource.clip = m_PlantHarvestSFX;
        m_AudioSource.Play();
        //Debug.Log("StartPlantHarvestAnimation");
        m_isOnHarvesting = true;
        int temp = m_plantSpeciesId;
        Vector2 Origin = m_PlantImage.rectTransform.anchoredPosition;

        float time = 0.0f;
        float xScale = m_PlantImage.rectTransform.localScale.x;
        float yScale = m_PlantImage.rectTransform.localScale.y;

        float xTargetScale1 = xScale * 1.4f;
        float yTargetScale1 = yScale * 0.8f;

        float xTargetScale2 = xScale * 0.7f;
        float yTargetScale2 = yScale * 1.2f;

        float xTargetScale3 = xScale;
        float yTargetScale3 = yScale;


        while (time < 0.15f)
        {
            time += Time.unscaledDeltaTime;
            float xtemp = Mathf.Lerp(xScale, xTargetScale1, time / 0.15f);
            float ytemp = Mathf.Lerp(yScale, yTargetScale1, time / 0.15f);
            m_PlantImage.rectTransform.localScale = new Vector3(xtemp, ytemp, 1.0f);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.05f);

        time = 0.0f;
        xScale = m_PlantImage.rectTransform.localScale.x;
        yScale = m_PlantImage.rectTransform.localScale.y;

        while (time < 0.3f)
        {
            time += Time.unscaledDeltaTime;
            float xtemp = Mathf.Lerp(xScale, xTargetScale2, time / 0.3f);
            float ytemp = Mathf.Lerp(yScale, yTargetScale2, time / 0.3f);
            m_PlantImage.rectTransform.localScale = new Vector3(xtemp, ytemp, 1.0f);
            yield return null;
        }

        time = 0.0f;
        xScale = m_PlantImage.rectTransform.localScale.x;
        yScale = m_PlantImage.rectTransform.localScale.y;
        m_PlantImage.rectTransform.pivot = new Vector2(0.5f, 1.0f);//피봇을 바꾸면 이미지의 위치도 변함
        m_PlantImage.rectTransform.anchoredPosition = new Vector2(m_PlantImage.rectTransform.anchoredPosition.x,
            m_PlantImage.rectTransform.anchoredPosition.y + m_PlantImage.rectTransform.sizeDelta.y * m_PlantImage.rectTransform.localScale.y);


        while (time < 0.2f)
        {
            time += Time.unscaledDeltaTime;
            float xtemp = Mathf.Lerp(xScale, xTargetScale3, time / 0.2f);
            float ytemp = Mathf.Lerp(yScale, yTargetScale3, time / 0.2f);
            m_PlantImage.rectTransform.localScale = new Vector3(xtemp, ytemp, 1.0f);
            yield return null;
        }

        //yield return new WaitForSecondsRealtime(0.05f);

        time = 0.0f;
        float ypos = m_PlantImage.rectTransform.anchoredPosition.y;
        float targetYPos = ypos + 40;

        while (time < 0.3f)
        {
            time += Time.unscaledDeltaTime;
            m_PlantImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - time / 0.3f);
            float yPosTemp = Mathf.Lerp(ypos, targetYPos, time / 0.3f);
            m_PlantImage.rectTransform.anchoredPosition = new Vector2(m_PlantImage.rectTransform.anchoredPosition.x, yPosTemp);
            yield return null;
        }


        m_State = PlantState.NONE;
        m_PlantImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        m_PlantImage.rectTransform.pivot = new Vector2(0.5f, 0.0f);
        m_PlantImage.rectTransform.anchoredPosition = Origin;
        m_isOnHarvesting = false;
        m_onHarvestEvent.Invoke(temp);
    }

    private IEnumerator GreenPlantHarvestAnimationCoroutine()
    {
        m_AudioSource.clip = m_GreenPlantHarvestSFX;
        m_AudioSource.Play();
        float scaler = 1.0f;
        while ((scaler -= Time.unscaledDeltaTime) > 0.0f)
        {
            m_PlantImage.color = new Color(1.0f, 1.0f, 1.0f, scaler);
            yield return null;
        }
        m_PlantImage.enabled = false;
        m_PlantImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        int temp = m_plantSpeciesId;
        m_State = PlantState.NONE;
        m_onHarvestEvent.Invoke(temp);

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_DragEnabled)
        {
            //Debug.Log("BeginDrag: " + eventData.position.y);
            BeginDragPos = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_InteractionMode == InteractionMode.DRAG)
        {
            //Debug.Log("OnDrag" + eventData.position.y);
            if (eventData.position.y > BeginDragPos.y + 40)
            {
                //Debug.Log("Fit!!!!!!");
                m_InteractionMode = InteractionMode.NONE;
                OnTouched();
            }
        }
    }
}
