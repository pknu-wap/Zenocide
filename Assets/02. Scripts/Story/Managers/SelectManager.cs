using UnityEngine;

public class SelectManager : MonoBehaviour
{
    public static SelectManager instance;

    [Header("선택지 오브젝트")]
    public GameObject[] Choices = new GameObject[4];

    [Header("대화창 오브젝트")]
    public GameObject Dialogue;

    [Header("왼쪽 선택지 표시 오브젝트")]
    public GameObject[] LeftSign = new GameObject[4];

    [Header("오른쪽 선택지 표시 오브젝트")]
    public GameObject[] RightSign = new GameObject[4];
    
    [Header("선택지 갯수 저장 변수")]
    public int choiceCount = 4;

    private void Awake()
    {
        if (instance == null) instance = this;
        // 기존에 instance가 존재하면 현재 오브젝트를 파괴
        else Destroy(gameObject); 
        DontDestroyOnLoad(gameObject);
    }

    public Choice currentChoice;
    public bool Selected = false;

    void Start()
    {
        // 모든 선택지 표시 비활성화
        SetSignVisibility(null); 
    }

    void Update()
    {
        if (currentChoice != null)
        {
            if (Choice.Selected != -1)
            {
                foreach (GameObject choice in Choices)
                {
                    choice.SetActive(false);
                }
                Dialogue.SetActive(true);
                Selected = true;
            }
            // 선택지에 따라 선택 표시 함수 호출
            SetSignVisibility(currentChoice.name);
        }
    }

    private void SetSignVisibility(string choiceName)
    {
        for (int i = 0; i < choiceCount; i++)
        {
            if (Choices[i].name == choiceName)
            {
                LeftSign[i].SetActive(true);
                RightSign[i].SetActive(true);
                continue;
            }
            LeftSign[i].SetActive(false);
            RightSign[i].SetActive(false);
        }
        
    }
}
