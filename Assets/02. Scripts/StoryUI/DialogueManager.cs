using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DialogueManager : MonoBehaviour, IPointerDownHandler
{
    [Header("Text 오브젝트 관련 요소")]
    public TMP_Text dialogueText;               
    public TMP_Text dialogueName;               
    public TMP_Text choiceUpText;               
    public TMP_Text choiceDownText;             
    public TMP_Text choiceUpRequireText;        
    public TMP_Text choiceDownRequireText;
    [Header("대화창 오브젝트 관련 요소")]      
    public GameObject dialogueBox;              
    public GameObject dialoguePanel;            
    public GameObject choiceUpPanel;            
    public GameObject choiceDownPanel;          
    public GameObject waitCursor;
    [Header("캐릭터 이미지 데이터 요소")]               
    public Image dialogueImage;                 
    public Sprite[] dialogueImages;             

    [Header("Text 데이터 요소")]
    public string[] storyText;                  
    public string[] storyName;                  
    public string[] choiceUpContent;            
    public string[] choiceDownContent;          

    private int currentLine = -1;               
    private int currentChoice = 0;              
    private bool isTyping = false;              
    private bool cancelTyping = false;          

    // 대화창 등장인물 이미지 테이블
    private readonly Dictionary<string, int> illustTable = new Dictionary<string, int>()
    {
        {"???", 0},
        {"좀비", 1}
    };

    void Start()
    {
        // 대화창 비활성화
        dialogueBox.SetActive(false);           
        // 대화 위 선택지 비활성화
        choiceUpPanel.SetActive(false);         
        // 대화 아래 선택지 비활성화
        choiceDownPanel.SetActive(false);       
        // 대화 불러오기
        LoadDialogue();                         
        // 대화창 활성화
        ShowDialogue();                         
    }

    void Update() {}

    // 마우스 입력에 반응하여 대화 진행
    public void OnPointerDown(PointerEventData eventData)
    {
        NextDialogue();
    }

    private void NextDialogue()
    {
        if (isTyping && !cancelTyping)
        {
            cancelTyping = true;
            return;
        }

        currentLine++;

        if (currentLine >= storyText.Length)
        {
            dialogueBox.SetActive(false);
            return;
        }

        dialogueName.text = storyName[currentLine];

        if (dialogueName.text == "#")
        {
            DisplayChoices();
        }

        if(dialogueName.text != null && dialogueName.text != "#")
        {
            // 캐릭터 이미지 변경
            dialogueImage.sprite = dialogueImages[illustTable[dialogueName.text]];
            // 대화 출력
            StartCoroutine(TypeSentence(storyText[currentLine]));
        }
        
    }

    private IEnumerator TypeSentence(string sentence)
    {   
        // 대화 진행 시작
        isTyping = true;
        // 다음 대화로 넘어가기 전에 기다리는 커서 비활성화
        waitCursor.SetActive(false);
        // 
        cancelTyping = false;
        dialogueText.text = "";

        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.03f);
            // 타이핑 효과 취소 시 대화를 한번에 출력
            if (cancelTyping)
            {
                dialogueText.text = sentence;
                break;
            }
        }
        // 대화 진행 종료
        isTyping = false;
        waitCursor.SetActive(true);
    }

    private void DisplayChoices()
    {
        dialogueText.text = "....";
        dialogueName.text = "";
        dialoguePanel.SetActive(false);
        choiceUpPanel.SetActive(true);
        choiceDownPanel.SetActive(true);

        choiceUpText.text = choiceUpContent[currentChoice];
        choiceDownText.text = choiceDownContent[currentChoice];

        if (Items.items.Contains("총"))
        {
            choiceDownRequireText.text = "필요한 아이템: <color=red>총</color>";
        }
        else
        {
            choiceDownRequireText.text = "필요한 아이템: <color=green>총</color>/보상: 빵";
        }

        currentChoice++;
    }

    private void ShowDialogue()
    {
        currentLine = -1;
        // 대화창 활성화
        dialogueBox.SetActive(true);
        // 초기 이미지를 주인공으로 설정
        dialogueImage.sprite = dialogueImages[illustTable["???"]];
    }

    private void LoadDialogue()
    {
        var dataDialog = CSVReader.Read("StoryScript");

        // 대화 데이터의 개수
        int dataCount = dataDialog.Count;

        storyName = new string[dataCount];
        storyText = new string[dataCount];
        choiceUpContent = new string[dataCount];
        choiceDownContent = new string[dataCount];

        int choiceLines = 0;
        for (int i = 0; i < dataCount; i++)
        {
            if (dataDialog[i]["Name"].ToString() == "#")
            {
                choiceUpContent[choiceLines] = dataDialog[i]["Text1"].ToString();
                choiceDownContent[choiceLines] = dataDialog[i]["Text2"].ToString();
                choiceLines++;
            }
                storyText[i] = dataDialog[i]["Text1"].ToString();
                storyName[i] = dataDialog[i]["Name"].ToString();

        }
    }
}
