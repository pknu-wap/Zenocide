using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiaryManager : MonoBehaviour
{
    // CSV 내용이 담겨 있는 딕셔너리 리스트
    private List<Dictionary<string, object>> diaryDialog;
    public TextAsset diaryCSV;

    // 현재 출력 중인 텍스트 번호
    private int dialogIndex;
    // 현재 출력 중인 페이지 번호
    private int pageIndex;

    private void Awake()
    {
        diaryDialog = CSVReader.Read(diaryCSV);

        Debug.Log(diaryDialog[0]["내용"]);
    }

    // 문장을 넘겨준다.
    public void AddDialog()
    {

    }
}
