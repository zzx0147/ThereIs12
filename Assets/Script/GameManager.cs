using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int m_EquipmentGrade;

    private float[] m_PlantRegenTimeMin = { 70.0f, 70.0f * 0.8f, 70.0f * 0.6f, 70.0f * 0.4f, 70.0f * 0.2f, };
    private float[] m_PlantRegenTimeMax = { 70.0f + 20.0f, (70.0f + 20.0f) * 0.8f, (70.0f + 20.0f) * 0.6f, (70.0f + 20.0f) * 0.4f, (70.0f + 20.0f) * 0.2f, };
    private float[] m_PlantRegenProbability;

    [SerializeField]
    private Image[] m_PlantImages;
    [SerializeField]
    private Sprite[] m_PlantSprites;


    // Start is called before the first frame update
    private void Start()
    {
        Time.timeScale = 10.0f;
        m_EquipmentGrade = DataManager.GetEquipmentGrade();
        m_PlantRegenProbability = new float[50/*m_PlantSprites.Length*/];//0번은 개풀

        for (int i = 0; i < m_PlantImages.Length; ++i)
        {
            int temp = DataManager.GetPlantData(i);

            Debug.Log("Image_" + i + ":" +temp);
            if (temp >= 0)
            {
                m_PlantImages[i].enabled = true;
                m_PlantImages[i].sprite = m_PlantSprites[DataManager.GetPlantData(i)];
                m_PlantImages[i].SetNativeSize();
            }
            else
            {
                m_PlantImages[i].enabled = false;
            }
        }

        string[,] PlantData = CsvLoader.LoadCsvBy2DimensionArray("Csv/PlantData");
        m_PlantRegenProbability[0] = 50.0f;

        for (int i = 1; i < m_PlantSprites.Length; ++i)
        {
            m_PlantRegenProbability[i] = float.Parse(PlantData[i + 3, 13]);
            Debug.Log(m_PlantRegenProbability[i]);
        }


        StartCoroutine(PlantGrow());

    }

    // Update is called once per frame
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            SpawnRandomPlant();
        }
    }


    private IEnumerator PlantGrow()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(m_PlantRegenTimeMin[m_EquipmentGrade], m_PlantRegenTimeMax[m_EquipmentGrade]));
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
                if (probabilityMin <= randFloat && m_PlantRegenProbability[i] + probabilityMin > randFloat)//확률 범위 안에 randFloat가 들어올 경우
                {
                    m_PlantImages[temp[targetImage]].sprite = m_PlantSprites[i];
                    m_PlantImages[temp[targetImage]].SetNativeSize();
                    DataManager.SetPlantData(temp[targetImage], i);
                    
                    goto whileEnd;
                }
                probabilityMin += m_PlantRegenProbability[i];
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

}
