/*using UnityEngine;
using System.Collections.Generic;


public class CSVManager : MonoBehaviour
{
    public EventData eventData;

    [Header("CSV 데이터")]
    public List<Dictionary<string, object>> dataCSV;


    void Start()
    {
        //CSV 파일 읽기
        dataCSV = CSVReader.Read("DialogueScript");
    }

    //인덱스에 따라 CSV 행 반환
    public Dictionary<string, object> Search(int index)
    {
        return dataCSV[index];
    }

}*/