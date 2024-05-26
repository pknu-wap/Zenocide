using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DialogueManager : MonoBehaviour, IPointerDownHandler
{
    [Header("Text 오브젝트")]
    public TMP_Text dialogueText;               
    public TMP_Text dialogueName;               
    public TMP_Text choiceUpText;               
    public TMP_Text choiceDownText;             
    public TMP_Text choiceUpRequireText;        
    public TMP_Text choiceDownRequireText;

    [Header("대화창 오브젝트")]      
    public GameObject dialogueBox;              
    public GameObject dialoguePanel;

    [Header("선택지 오브젝트")]            
    public GameObject choiceUpPanel;            
    public GameObject choiceDownPanel;

    [Header("대화창 출력 완료 시 대기 표시")]          
    public GameObject waitCursor;

    [Header("CSV 데이터")]
    public GameObject CSVManager;

    [Header("캐릭터 이미지 데이터")]               
    public Image dialogueImage;                 
    public Sprite[] dialogueImages;             

    [Header("Text 데이터")]
    public string[] storyText;                  
    public string[] storyName;                  
    public string[] choiceUpContent;            
    public string[] choiceDownContent;          

    [Header("대화 텍스트 출력 진행 확인 변수")]            
    private bool isTyping = false;

    [Header("대화 텍스트 출력 중단 요청 확인 변수")]              
    private bool cancelTyping = false;          

    [Header("마우스 입력 감지 변수")]
    private bool isClicked = false;

    [Header("일러스트 데이터")]
    public Dictionary<string, int> illustTable = new Dictionary<string, int>()
    {
        {"???", 0},
        {"좀비", 1},
        {"선택지",2}
    };

    [Header("전체 이벤트 데이터")]
    public EventQueue TotalEvent = EventList.TotalEventQueue;

    void Start()
    {
        // 대화창 비활성화
        dialogueBox.SetActive(false);           
        // 대화 위 선택지 비활성화
        choiceUpPanel.SetActive(false);         
        // 대화 아래 선택지 비활성화
        choiceDownPanel.SetActive(false);                            
        // 대화창 활성화
        ShowDialogue();
        // 대화 시작
        EventProcess();                         
    }

     public void OnPointerDown(PointerEventData eventData)
    {
        isClicked = true;
    }

    private IEnumerator EventProcess()
    {
        while (TotalEvent.Count > 0)
        {
            EventData presentEvent = TotalEvent.Dequeue();
            Debug.Log(presentEvent.startIndex + " " + presentEvent.endIndex);
            for (int i = presentEvent.startIndex; i < presentEvent.endIndex; i++)
            {
                dialogueName.text = CSVManager.GetComponent<CSVManager>().Search(i)["Name"].ToString();
                dialogueText.text = CSVManager.GetComponent<CSVManager>().Search(i)["Text"].ToString();

                DisplayDialogue();
                
                //마우스 입력 대기
                yield return new WaitUntil(() => isClicked);
                isClicked = false;

            }
            
        }
    }

    private void DisplayDialogue()
    {


        if (isTyping && !cancelTyping)
        {
            cancelTyping = true;
            return;
        }

        if (dialogueName.text == "#")
        {
            //DisplayChoices();
        }

        if(dialogueName.text != null && dialogueName.text != "#")
        {
            // 캐릭터 이미지 변경
            dialogueImage.sprite = dialogueImages[illustTable[dialogueName.text]];
            // 대화 출력
            StartCoroutine(TypeSentence(dialogueText.text));
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

    /*private void DisplayChoices()
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
    }*/

    private void ShowDialogue()
    {
        // 대화창 활성화
        dialogueBox.SetActive(true);
        // 초기 이미지를 주인공으로 설정
        dialogueImage.sprite = dialogueImages[illustTable["???"]];
    }

}
