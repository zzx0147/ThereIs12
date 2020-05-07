using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class CsvLoader
{

    public static string[] LoadCsvByArray(string path)
    {
        string[] Seperator = new string[1] { "\r\n" };
        string[] LoadedCsv = null;

        LoadedCsv = (Resources.Load(path) as TextAsset).text.Split(Seperator, StringSplitOptions.None);

        return LoadedCsv;
    }

    public static string[,] LoadCsvBy2DimensionArray(string path)
    {
        string[] Seperator = new string[1] { "," };
        string[] LoadedCsv = LoadCsvByArray(path);
        MatchCollection matches = Regex.Matches(LoadedCsv[1], ",");
        int cnt = matches.Count + 1;

        string[,] LoadedCsvBy2DementionArray = new string[LoadedCsv.Length, cnt];//CSV파일 가로세로 길이 측정 후 2차원 배열 생성

        for (int i = 0; i < LoadedCsv.Length; ++i)
        {
            string[] TempStringArray = LoadedCsv[i].Split(Seperator, StringSplitOptions.None);

            for (int j = 0; j < TempStringArray.Length; ++j)
            {
                LoadedCsvBy2DementionArray[i, j] = TempStringArray[j];
            }

        }
        return LoadedCsvBy2DementionArray;
    }
}
