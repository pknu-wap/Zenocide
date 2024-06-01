using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiaryManager : MonoBehaviour
{
    private List<Dictionary<string, object>> diaryDialog;

    private void Awake()
    {
        diaryDialog = CSVReader.Read("Assets/NoShare2024-1/NoShare2024-1/04. Scenarios/Tutorial Diary");

        Debug.Log(diaryDialog[0]["내용"]);
    }
}
