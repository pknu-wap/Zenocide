using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiaryManager : MonoBehaviour
{
    private List<Dictionary<string, object>> diaryDialog;

    private void Awake()
    {
        diaryDialog = CSVReader.Read("Tutorial Diary");

        Debug.Log(diaryDialog[0]["내용"]);
    }
}
