using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterRandomChanger : MonoBehaviour
{
    [SerializeField] private Image m_CharacterImage;
    [SerializeField] private Text m_CharacterDialogue;
    [SerializeField] private Text m_CharacterName;
    [SerializeField] private Sprite[] m_CharacterSprites;

    private string[,] m_CharacterInfoCsv;

    private void Awake()
    {
        m_CharacterInfoCsv = CsvLoader.LoadCsvBy2DimensionArray("Csv/NPC_Table");
        
    }

    public void RandomChangeCharacter()
    {
        int num = Random.Range(1, m_CharacterInfoCsv.GetLength(0));

        m_CharacterName.text = m_CharacterInfoCsv[num, 1];
        m_CharacterDialogue.text = m_CharacterInfoCsv[num,2];

        switch(m_CharacterInfoCsv[num,1])
        {

            case "정채린":
                m_CharacterImage.sprite = m_CharacterSprites[0];
                break;

            case "정단비":
                m_CharacterImage.sprite = m_CharacterSprites[1];
                break;

            case "김혜진":
                m_CharacterImage.sprite = m_CharacterSprites[2];
                break;

            case "김윤영":
                m_CharacterImage.sprite = m_CharacterSprites[3];
                break;

            case "이두현":
                m_CharacterImage.sprite = m_CharacterSprites[4];
                break;

            case "나호겸":
                m_CharacterImage.sprite = m_CharacterSprites[5];
                break;

            case "박세영":
                m_CharacterImage.sprite = m_CharacterSprites[6];
                break;

            case "윤다은":
                m_CharacterImage.sprite = m_CharacterSprites[7];
                break;
        }
    }
}
