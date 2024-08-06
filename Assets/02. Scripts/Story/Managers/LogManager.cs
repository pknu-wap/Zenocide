using System.Collections.Generic;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    public static LogManager Instance { get; set; }
    
    [Header("로그 오브젝트")]
    public GameObject logPannel;

    [Header("로그 박스 오브젝트")]
    public GameObject[] logBoxes;
    
    [Header("현재 로그 인덱스")]
    public int currentIndex = 0;

    [Header("최대 저장 가능 로그 수")]
    private int maxNum = 30; //0 ~ 29 = 30개

    [Header("획득 확인 변수")]
    private const string empty = "";

    [Header("선택지 확인 변수")]
    private const int notSelected = -1;
    
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(this);
        }
        logPannel = GameObject.FindWithTag("Log Pannel");
        GameObject Logs = logPannel.transform.GetChild(2).GetChild(0).gameObject;
        //Cotent 오브젝트의 자식 개수(= 로그 수 / 현재 30 개)만큼 GameObject 배열 생성
        logBoxes = new GameObject[Logs.transform.childCount];
        //30개의 로그 박스들을 저장 후 비활성화
        for (int i = Logs.transform.childCount - 1; i >= 0  ; i--)
        {
            logBoxes[i] = Logs.transform.GetChild(i).gameObject;
            logBoxes[i].SetActive(false);
        }
        logPannel.SetActive(false);
    }

    //로그 초기화 함수
    public void ResetLogs()
    {
        for (int i = 0; i < logBoxes.Length; i++)
        {
            logBoxes[i].SetActive(false);
        }
    }

    public void AddLog(Dictionary<string, object> Dialogue,int choiceResult)
    {
        string text = Dialogue["Text"].ToString();

        if(text is empty && choiceResult is notSelected ) return; //대사가 비어 있고 선택지도 아닐 경우 함수 종료

        //로그 박스 활성화
        logBoxes[currentIndex].SetActive(true);
        //해당 로그 박스 업데이트
        logBoxes[currentIndex].GetComponent<TextLog>().UpdateTextLog(Dialogue,choiceResult);
        //해당 로그 박스 순위를 마지막으로 변경
        logBoxes[currentIndex].transform.SetAsLastSibling();

        currentIndex = (currentIndex + 1) % maxNum;

    }
}