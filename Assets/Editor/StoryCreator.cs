using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class StoryCreator : Editor
{
    [MenuItem("Assets/Create Story/Create Story")]
    public static void CreateStory()
    {
        // 먼저 스토리 이벤트를 전부 생성한다.
        CreateMainStory();
        CreateSubStory();
        CreateRelationStory();

        // 이후 이벤트를 할당한다.
        AssignMainStory();
        AssignSubStory();
        AssignRelationStory();
    }

    #region 메인 스토리
    // 메인 스토리를 생성한다.
    public static void CreateMainStory()
    {
        // 메인 스토리 CSV를 불러온다.
        TextAsset subStory = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/NoShare2024-1/NoShare2024-1/04. Scenarios/MainStorys.csv");
        List<Dictionary<string, object>> csvFile = CSVReader.Read(subStory);

        // 파일을 생성할 폴더 경로
        string destinationFolder = "Assets/02. Scripts/Story/EventData SO/Events/Main/";
        string destinationPath; // 복사된 파일 경로
        EventData currentAsset;

        // 스킵할 아이디
        string skipId = "";
        // 현재 라인의 아이디
        string currentId;

        // 기존 이벤트의 끝에서, 마지막까지 탐색
        for (int i = 0; i < csvFile.Count; ++i)
        {
            // 아이디를 가져온다.
            currentId = csvFile[i]["Id"].ToString();

            // 빈칸이거나 스킵 아이디라면
            if (csvFile[i]["Id"].ToString() == "" || currentId == skipId)
            {
                // 넘어간다.
                continue;
            }

            // 빈칸이 아니라면 새 경로를 만든다.
            destinationPath = Path.Combine(destinationFolder, $"{currentId}.asset");

            // 파일이 없다면 생성한다.
            if (File.Exists(destinationPath) == false)
            {
                // 파일이 없다면 새로 만든다.
                EventData asset = CreateInstance<EventData>();
                AssetDatabase.CreateAsset(asset, destinationPath);
            }

            // 현재 가리키는 에셋 교체
            currentAsset = AssetDatabase.LoadAssetAtPath<EventData>(destinationPath);
            // 스킵 아이디로 설정한다.
            skipId = currentId;

            // 정보를 변경한다.
            currentAsset.eventID = EventType.Main;
            // 시작 인덱스를 적어준다.
            currentAsset.startIndex = i;

            // 빈칸이 나올 때까지 스킵
            while (true)
            {
                if(i + 1 >= csvFile.Count)
                {
                    break;
                }

                currentId = csvFile[i + 1]["Id"].ToString();

                if(currentId == "")
                {
                    break;
                }

                ++i;
            }

            // 마지막 인덱스를 적어준다.
            currentAsset.endIndex = i;
        }

        // 에셋 데이터베이스를 리프레시해서 새로 생성된 파일을 유니티가 인식하도록 함
        AssetDatabase.Refresh();
        Debug.Log("메인 스토리가 생성되었습니다!");
    }

    // 메인 스토리 이벤트들 내에 연관 이벤트를 할당한다.
    public static void AssignMainStory()
    {
        // 메인 스토리 CSV를 불러온다.
        TextAsset subStory = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/NoShare2024-1/NoShare2024-1/04. Scenarios/MainStorys.csv");
        List<Dictionary<string, object>> csvFile = CSVReader.Read(subStory);

        // 작업할 폴더 경로
        string folderPath = "Assets/02. Scripts/Story/EventData SO/Events/Main/";

        // 폴더 내의 모든 에셋 파일 경로 가져오기
        string[] assetGuids = AssetDatabase.FindAssets("", new[] { folderPath });
        // 각 에셋을 로드하고 ScriptableObject로 캐스팅
        foreach (string guid in assetGuids)
        {
            // 에셋 경로 가져오기
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // 에셋 로드
            EventData asset = AssetDatabase.LoadAssetAtPath<EventData>(assetPath);

            if (asset != null)
            {
                // ScriptableObject의 내부 데이터 접근 예시
                if (asset is EventData story)
                {
                    int i = story.endIndex;

                    // 선택지 이벤트가 있다면
                    if(csvFile[i]["Choice Event Count"].ToString() != "")
                    {
                        // 개수만큼 만들고
                        int count = int.Parse(csvFile[i]["Choice Event Count"].ToString());
                        story.relationEvent = new EventData[count];

                        // 할당한다.
                        for (int j = 1; j <= count; ++j)
                        {
                            // 빈칸이 아니면 할당한다.
                            if (csvFile[i]["Choice Event" + j].ToString() != "")
                            {
                                story.relationEvent[j - 1] = FindEventData(csvFile[i]["Choice Event" + j].ToString());
                            }
                        }
                    }

                    // Next Event가 있다면 연결한다.
                    if(csvFile[i]["Next Event"].ToString() != "")
                    {
                        story.nextEvent = FindEventData(csvFile[i]["Next Event"].ToString());
                    }

                    // Add Event가 있다면 연결한다.
                    if (csvFile[i]["Add Event"].ToString() != "")
                    {
                        // 일단 하나만 가능하게 해보자.
                        story.addEvent = new EventData[] { FindEventData(csvFile[i]["Add Event"].ToString()) };
                    }
                }
            }
        }

        Debug.Log("메인 이벤트에 연계 이벤트를 모두 할당했습니다.");
    }
    #endregion 메인 스토리

    #region 서브 스토리
    // 서브 스토리를 생성한다.
    public static void CreateSubStory()
    {
        // 서브 스토리 CSV를 불러온다.
        TextAsset subStory = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/NoShare2024-1/NoShare2024-1/04. Scenarios/SubStorys.csv");
        List<Dictionary<string, object>> csvFile = CSVReader.Read(subStory);

        // 파일을 생성할 폴더 경로
        string destinationFolder = "Assets/02. Scripts/Story/EventData SO/Events/Sub/";
        string destinationPath; // 복사된 파일 경로
        EventData currentAsset;

        // 현재 라인의 아이디
        string currentId;

        // 기존 이벤트의 끝에서, 마지막까지 탐색
        for (int i = 0; i < csvFile.Count; ++i)
        {
            // 아이디를 가져온다.
            currentId = csvFile[i]["Id"].ToString();

            // 빈칸이거나 스킵 아이디라면
            if (csvFile[i]["Id"].ToString() == "")
            {
                // 넘어간다.
                continue;
            }

            // 빈칸이 아니라면 새 경로를 만든다.
            destinationPath = Path.Combine(destinationFolder, $"{currentId}.asset");

            // 파일이 없다면 생성한다.
            if (File.Exists(destinationPath) == false)
            {
                // 파일이 없다면 새로 만든다.
                EventData asset = CreateInstance<EventData>();
                AssetDatabase.CreateAsset(asset, destinationPath);
            }

            // 현재 가리키는 에셋 교체
            currentAsset = AssetDatabase.LoadAssetAtPath<EventData>(destinationPath);
            // 정보를 변경한다.
            currentAsset.eventID = EventType.Sub;
            // 시작 인덱스를 적어준다.
            currentAsset.startIndex = i;

            // 빈칸이 나올 때까지 스킵
            while (true)
            {
                if (i + 1 >= csvFile.Count)
                {
                    break;
                }

                currentId = csvFile[i + 1]["Id"].ToString();

                if (currentId == "")
                {
                    break;
                }

                ++i;
            }

            // 마지막 인덱스를 적어준다.
            currentAsset.endIndex = i;
        }

        // 에셋 데이터베이스를 리프레시해서 새로 생성된 파일을 유니티가 인식하도록 함
        AssetDatabase.Refresh();
        Debug.Log("서브 스토리가 생성되었습니다!");
    }

    // 서브 스토리 이벤트들 내에 연관 이벤트를 할당한다.
    public static void AssignSubStory()
    {
        // 서브 스토리 CSV를 불러온다.
        TextAsset subStory = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/NoShare2024-1/NoShare2024-1/04. Scenarios/SubStorys.csv");
        List<Dictionary<string, object>> csvFile = CSVReader.Read(subStory);

        // 작업할 폴더 경로
        string folderPath = "Assets/02. Scripts/Story/EventData SO/Events/Sub/";

        // 폴더 내의 모든 에셋 파일 경로 가져오기
        string[] assetGuids = AssetDatabase.FindAssets("", new[] { folderPath });
        // 각 에셋을 로드하고 ScriptableObject로 캐스팅
        foreach (string guid in assetGuids)
        {
            // 에셋 경로 가져오기
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // 에셋 로드
            EventData asset = AssetDatabase.LoadAssetAtPath<EventData>(assetPath);

            if (asset != null)
            {
                // ScriptableObject의 내부 데이터 접근 예시
                if (asset is EventData story)
                {
                    int i = story.endIndex;

                    // 선택지 이벤트가 있다면
                    if (csvFile[i]["Choice Event Count"].ToString() != "")
                    {
                        // 개수만큼 만들고
                        int count = int.Parse(csvFile[i]["Choice Event Count"].ToString());
                        story.relationEvent = new EventData[count];

                        // 할당한다.
                        for (int j = 1; j <= count; ++j)
                        {
                            // 빈칸이 아니면 할당한다.
                            if (csvFile[i]["Choice Event" + j].ToString() != "")
                            {
                                story.relationEvent[j - 1] = FindEventData(csvFile[i]["Choice Event" + j].ToString());
                            }
                        }
                    }

                    // Next Event가 있다면 연결한다.
                    if (csvFile[i]["Next Event"].ToString() != "")
                    {
                        story.nextEvent = FindEventData(csvFile[i]["Next Event"].ToString());
                    }

                    // Add Event가 있다면 연결한다.
                    if (csvFile[i]["Add Event"].ToString() != "")
                    {
                        // 일단 하나만 가능하게 해보자.
                        story.addEvent = new EventData[] { FindEventData(csvFile[i]["Add Event"].ToString()) };
                    }
                }
            }
        }

        Debug.Log("서브 이벤트에 연계 이벤트를 모두 할당했습니다.");
    }
    #endregion 서브 스토리

    #region 선택지 스토리
    // 선택지 스토리를 생성한다.
    public static void CreateRelationStory()
    {
        // 선택지 스토리 CSV를 불러온다.
        TextAsset subStory = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/NoShare2024-1/NoShare2024-1/04. Scenarios/RelationStorys.csv");
        List<Dictionary<string, object>> csvFile = CSVReader.Read(subStory);

        // 파일을 생성할 폴더 경로
        string destinationFolder = "Assets/02. Scripts/Story/EventData SO/Events/Relation/";
        string destinationPath; // 복사된 파일 경로
        EventData currentAsset;

        // 스킵할 아이디
        string skipId = "";
        // 현재 라인의 아이디
        string currentId;

        // 기존 이벤트의 끝에서, 마지막까지 탐색
        for (int i = 0; i < csvFile.Count; ++i)
        {
            // 아이디를 가져온다.
            currentId = csvFile[i]["Id"].ToString();

            // 빈칸이거나 스킵 아이디라면
            if (csvFile[i]["Id"].ToString() == "" || currentId == skipId)
            {
                // 넘어간다.
                continue;
            }

            // 빈칸이 아니라면 새 경로를 만든다.
            destinationPath = Path.Combine(destinationFolder, $"{currentId}.asset");

            // 파일이 없다면 생성한다.
            if (File.Exists(destinationPath) == false)
            {
                // 파일이 없다면 새로 만든다.
                EventData asset = CreateInstance<EventData>();
                AssetDatabase.CreateAsset(asset, destinationPath);
            }

            // 현재 가리키는 에셋 교체
            currentAsset = AssetDatabase.LoadAssetAtPath<EventData>(destinationPath);
            // 스킵 아이디로 설정한다.
            skipId = currentId;

            // 정보를 변경한다.
            currentAsset.eventID = EventType.Relation;
            // 시작 인덱스를 적어준다.
            currentAsset.startIndex = i;

            // 빈칸이 나올 때까지 스킵
            while (true)
            {
                if (i + 1 >= csvFile.Count)
                {
                    break;
                }

                currentId = csvFile[i + 1]["Id"].ToString();

                if (currentId == "")
                {
                    break;
                }

                ++i;
            }

            // 마지막 인덱스를 적어준다.
            currentAsset.endIndex = i;
        }

        // 에셋 데이터베이스를 리프레시해서 새로 생성된 파일을 유니티가 인식하도록 함
        AssetDatabase.Refresh();
        Debug.Log("선택지 스토리가 생성되었습니다!");
    }

    // 선택지 스토리 이벤트들 내에 연관 이벤트를 할당한다.
    public static void AssignRelationStory()
    {
        // 선택지 스토리 CSV를 불러온다.
        TextAsset subStory = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/NoShare2024-1/NoShare2024-1/04. Scenarios/RelationStorys.csv");
        List<Dictionary<string, object>> csvFile = CSVReader.Read(subStory);

        // 작업할 폴더 경로
        string folderPath = "Assets/02. Scripts/Story/EventData SO/Events/Relation/";

        // 폴더 내의 모든 에셋 파일 경로 가져오기
        string[] assetGuids = AssetDatabase.FindAssets("", new[] { folderPath });
        // 각 에셋을 로드하고 ScriptableObject로 캐스팅
        foreach (string guid in assetGuids)
        {
            // 에셋 경로 가져오기
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // 에셋 로드
            EventData asset = AssetDatabase.LoadAssetAtPath<EventData>(assetPath);

            if (asset != null)
            {
                // ScriptableObject의 내부 데이터 접근 예시
                if (asset is EventData story)
                {
                    int i = story.endIndex;

                    // 선택지 이벤트가 있다면
                    if (csvFile[i]["Choice Event Count"].ToString() != "")
                    {
                        // 개수만큼 만들고
                        int count = int.Parse(csvFile[i]["Choice Event Count"].ToString());
                        story.relationEvent = new EventData[count];

                        // 할당한다.
                        for (int j = 1; j <= count; ++j)
                        {
                            // 빈칸이 아니면 할당한다.
                            if (csvFile[i]["Choice Event" + j].ToString() != "")
                            {
                                story.relationEvent[j - 1] = FindEventData(csvFile[i]["Choice Event" + j].ToString());
                            }
                        }
                    }

                    // Next Event가 있다면 연결한다.
                    if (csvFile[i]["Next Event"].ToString() != "")
                    {
                        story.nextEvent = FindEventData(csvFile[i]["Next Event"].ToString());
                    }

                    // Add Event가 있다면 연결한다.
                    if (csvFile[i]["Add Event"].ToString() != "")
                    {
                        // 일단 하나만 가능하게 해보자.
                        story.addEvent = new EventData[] { FindEventData(csvFile[i]["Add Event"].ToString()) };
                    }
                }
            }
        }

        Debug.Log("선택지 이벤트에 연계 이벤트를 모두 할당했습니다.");
    }
    #endregion 선택지 스토리

    static EventData FindEventData(string eventName)
    {
        // M이면 Main에서 찾는다.
        if (eventName[0] == 'M')
        {
            string path = Path.Combine("Assets/02. Scripts/Story/EventData SO/Events/Main/", $"{eventName}.asset"); // 복사된 파일 경로
            return AssetDatabase.LoadAssetAtPath<EventData>(path);
        }

        // S이면 Sub에서 찾는다.
        else if (eventName[0] == 'S')
        {
            string path = Path.Combine("Assets/02. Scripts/Story/EventData SO/Events/Sub/", $"{eventName}.asset"); // 복사된 파일 경로
            return AssetDatabase.LoadAssetAtPath<EventData>(path);
        }

        // R이면 Relation에서 찾는다.
        else if (eventName[0] == 'R')
        {
            string path = Path.Combine("Assets/02. Scripts/Story/EventData SO/Events/Relation/", $"{eventName}.asset"); // 복사된 파일 경로
            return AssetDatabase.LoadAssetAtPath<EventData>(path);
        }

        else
        {
            Debug.LogError("이벤트 이름이 잘못되었습니다.");
            return null;
        }
    }
}
