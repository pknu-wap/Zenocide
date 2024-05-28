using UnityEngine;

public class SelectManager : MonoBehaviour
{
    public static SelectManager instance;

    [Header("선택지 오브젝트")]
    public GameObject ChoiceUp;
    public GameObject ChoiceDown;

    [Header("대화창 오브젝트")]
    public GameObject Dialogue;

    [Header("위 선택지 표시 오브젝트")]
    public GameObject UpSignLeft;
    public GameObject UpSignRight;

    [Header("아래 선택지 표시 오브젝트")]
    public GameObject DownSignLeft;
    public GameObject DownSignRight;

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
        SetSignVisibility(false, false); 
    }

    void Update()
    {
        if (currentChoice != null)
        {
            if (Choice.Selected != -1)
            {
                ChoiceUp.SetActive(false);
                ChoiceDown.SetActive(false);
                Dialogue.SetActive(true);
                Selected = true;
            }
            // 선택지에 따라 선택 표시 함수 호출
            SetSignVisibility(currentChoice.name == "ChoiceBoxUp", 
                              currentChoice.name == "ChoiceBoxDown");
        }
    }

    private void SetSignVisibility(bool isUpSelected, bool isDownSelected)
    {
        UpSignLeft.SetActive(isUpSelected);
        UpSignRight.SetActive(isUpSelected);
        DownSignLeft.SetActive(isDownSelected);
        DownSignRight.SetActive(isDownSelected);
    }
}
