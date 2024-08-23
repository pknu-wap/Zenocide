using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }

        DontDestroyOnLoad(Instance);
    }

    // 데이터 파일 이름
    string dataFileName = "Data.json";

    // 데이터 파일 경로
    string filePath;

    // SaveData 변수
    public Data data = new Data();

    // load 여부
    public bool isLoaded = false;

    private void Start()
    {
        filePath = Application.persistentDataPath + "/" + dataFileName;
    }

    // 로드
    public void LoadData()
    {
        // 세이브 데이터가 있으면
        if (IsFileExist())
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
        // 옵션으로 생기는 세이브 방지
        if(data.currentEvent == null)
        {
            return;
        }

        string filePath = Application.persistentDataPath + "/" + dataFileName;

        // Data를 Json으로 변환 (true = 가독성 향상)
        string json = JsonUtility.ToJson(data, true);

        // json 파일을 8bit unsigned int로 변환
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);

        // 바이트 배열을 base-64 인코딩 문자열로 변환
        string encodedJson = System.Convert.ToBase64String(bytes);

        // 파일을 새로 생성하거나 덮어쓰기
        File.WriteAllText(filePath, encodedJson);
    }

    public void DeleteData()
    {
        if (IsFileExist())
        {
            File.Delete(filePath);
        }
    }
    
    public void StartLoadedGame()
    {
        // 각자 자리에 삽입한다.
        Player.Instance.LoadHp();
        CardManager.Instance.LoadDeck();
        DialogueManager.Instance.LoadDialogueData();
        Items.Instance.LoadItems();
        SoundManager.Instance.LoadVolumeSettings();
        ResolutionManager.Instance.LoadResolutionSettings();

        StartCoroutine(DialogueManager.Instance.ProcessLoadedEvent(data.CurrentEvent));
    }

    // Dictionary 형을 DicionaryData의 리스트로 형변환
    public List<DictionaryData<EventData, int>> DictionaryToDictionaryData(Dictionary<EventData, int> dic)
    {
        List<DictionaryData<EventData, int>> dictionaryDatas = new List<DictionaryData<EventData, int>>();

        foreach(EventData Event in dic.Keys)
        {
            DictionaryData<EventData, int> dictionaryData = new DictionaryData<EventData, int>();
            dictionaryData.key = Event;
            dictionaryData.value = dic[Event];
            dictionaryDatas.Add(dictionaryData);
        }

        return dictionaryDatas;
    }

    // DicionaryData의 리스트를 Dicionary로 형변환
    public Dictionary<EventData, int> DictionaryDataToDictinary(List<DictionaryData<EventData, int>> dicDatas)
    {
        Dictionary<EventData, int> dic = new Dictionary<EventData, int>();
        foreach(DictionaryData<EventData, int> dicData in dicDatas)
        {
            dic[dicData.key] = dicData.value;
        }

        return dic;
    }

    public bool IsFileExist()
    {
        return File.Exists(Application.persistentDataPath + "/" + dataFileName);
    }
}
