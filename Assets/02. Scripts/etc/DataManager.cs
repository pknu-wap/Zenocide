using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using UnityEditor.MPE;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    void Awake() => Instance = this;

    // 데이터 파일 이름
    string DataFileName = "Data.json";

    // SaveData 변수
    public Data data = new Data();

    // 로드
    public void LoadData()
    {
        string filePath = Application.persistentDataPath + "/" + DataFileName;

        // 세이브 데이터가 있으면
        if (File.Exists(filePath))
        {
            // 저장된 파일을 읽고
            string json = File.ReadAllText(filePath);

            byte[] bytes = System.Convert.FromBase64String(json);

            string decodedJson = System.Text.Encoding.UTF8.GetString(bytes);

            // Json을 Data 형식으로 전환
            data = JsonUtility.FromJson<Data>(decodedJson);
        }
    }

    // 세이브
    // 세이브 타이밍: 전투 종료, 이벤트 종료, 사망, 아이템 획득, 카드 획득, 수동 저장
    public void SaveData()
    {
        string filePath = Application.persistentDataPath + "/" + DataFileName;

        // Data를 Json으로 변환 (true = 가독성 향상)
        string json = JsonUtility.ToJson(data, true);

        // json 파일을 8bit unsigned int로 변환
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);

        // 바이트 배열을 base-64 인코딩 문자열로 변환
        string encodedJson = System.Convert.ToBase64String(bytes);

        // 파일을 새로 생성하거나 덮어쓰기
        File.WriteAllText(filePath, encodedJson);
    }

    public void StartSavedGame()
    {
        // 데이터를 로드하고
        LoadData();

        // 각자 자리에 삽입한다.
        Player.Instance.LoadHp();
        CardManager.Instance.LoadDeck();
        DialogueManager.Instance.LoadData();
        Items.Instance.LoadItems();
        SoundManager.Instance.LoadVolumeSettings();
        ResolutionManager.Instance.LoadResolutionSettings();

        // 이벤트씬으로 전환

        // 저장된 currentEvent를 실행
        StartCoroutine(DialogueManager.Instance.ProcessEvent(data.CurrentEvent));

        // 해당 이벤트가 실행된 이후에는 계속 다른 이벤트를 진행한다.
        StartCoroutine(DialogueManager.Instance.ProcessRandomEvent());
    }
}
