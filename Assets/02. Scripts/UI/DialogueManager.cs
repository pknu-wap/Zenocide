using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.IO;
using TMPro;
using System.Collections.Generic;
using JetBrains.Annotations;


public class DialogueManager : MonoBehaviour, IPointerDownHandler
{

    public TMP_Text dialogueText;               //Story Text
    public TMP_Text dialogueName;               //Story Name

    public TMP_Text ChoiceUpText;               //Up Selection Text
    public TMP_Text ChoiceDownText;             //Down Selection Text

    public TMP_Text ChoiceUpRequireText;        //Up Selection Text
    public TMP_Text ChoiceDownRequireText;      //Down Selection Text

    public GameObject dialogueBox;              //전체 Canvas

    public GameObject Dialogue;                 //대화창

    public GameObject ChoiceUp;                 //위 선택지 표시
    public GameObject ChoiceDown;               //아래 선택지 표시

    public GameObject WaitCursor;               //다음 대화를 위한 사용자 입력 대기 커서
      
    public Image dialogueImage;                 //일러스트
          
    public Sprite[] dialogueImages;             //일러스트 목록
      
    public string[] StoryText;                  //Story Text 배열
    public string[] StoryName;                  //Story Name 배열
    public string[] ChoiceUpContent;            //Choice Text 배열
    public string[] ChoiceDownContent;          //Choice Text 배열

    public int currentLine;                     //현재 출력 중인 문자열 위치
    public int currentChoice;                   //현재 선택지 위치

    private bool isTyping = false;              //타이핑 효과 진행 여부 확인 변수
    private bool cancelTyping = false;          //사용자의 입력으로 인한 출력 취소 확인 변수

    void Start()
    {
        dialogueBox.SetActive(false);           //시작 시 Canvas 전체 비활성화
        ChoiceUp.SetActive(false);              //시작 시 선택지 비활성화
        ChoiceDown.SetActive(false);    
        LoadDialogue();                         //Story Name,Text 불러오기
        ShowDialogue();                         //이미지와 전체 Canvas 표시
    }

    void Update()
    {
   
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       NextDialogue();
    }

    public void NextDialogue(){
        var IllustTable = new Dictionary<string,int>()
        {
            {"???",0},
            {"좀비",1}   
        };

        if(isTyping && !cancelTyping)
        {
            cancelTyping = true;
            return;
        }

        currentLine++;

        if (currentLine >= StoryText.Length)
        {
            dialogueBox.SetActive(false);
            return;
        }

        dialogueName.text = StoryName[currentLine];

        if(dialogueName.text == "#")
        {
            Selection();
        }
        else
        {
            //이미지 변경
            dialogueImage.sprite = dialogueImages[IllustTable[dialogueName.text]];
            StartCoroutine(TypeSentence(StoryText[currentLine]));
        }

    }

    //Story Text 출력 함수
    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        WaitCursor.SetActive(false);
        cancelTyping = false;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.03f);
            if (cancelTyping)
            {
                dialogueText.text = sentence;
                break;
            }
        }
        isTyping = false;
        WaitCursor.SetActive(true);
        cancelTyping = false;
    }

    void Selection()
    {
        dialogueText.text = "....";
        dialogueName.text = "";
        Dialogue.SetActive(false);
        ChoiceUp.SetActive(true);
        ChoiceDown.SetActive(true);
        ChoiceUpText.text   = ChoiceUpContent[currentChoice];
        ChoiceDownText.text = ChoiceDownContent[currentChoice];
        if(Items.items.Find(x => x == "총") != null){
        
        ChoiceDownRequireText.text = "필요한 아이템: <color=red>총</color>";
        
        }
        else{
        ChoiceDownRequireText.text = "필요한 아이템: <color=green>총</color>/보상: 빵";
        //Items.AddItem("빵");
        }
        currentChoice++;
    }

    //초기 대화창 표시 함수
    public void ShowDialogue()
    {
        currentLine = -1;
        dialogueBox.SetActive(true);
        dialogueImage.sprite = dialogueImages[0];
    }

    //Text 파일 불러오는 함수
    void LoadDialogue()
    {
        List<Dictionary<string, object>> data_Dialog = CSVReader.Read("StoryScript");

        StoryName = new string[data_Dialog.Count];
        StoryText = new string[data_Dialog.Count];
        ChoiceUpContent = new string[data_Dialog.Count];
        ChoiceDownContent = new string[data_Dialog.Count];

        int ChoiceLines = 0;
        for (int i = 0; i < data_Dialog.Count; i++)
        {
            if(data_Dialog[i]["Name"].ToString() == "#")
            {
                ChoiceUpContent[ChoiceLines]   = data_Dialog[i]["Text1"].ToString();
                ChoiceDownContent[ChoiceLines] = data_Dialog[i]["Text2"].ToString();
                ChoiceLines++;
            }
                StoryText[i] = data_Dialog[i]["Text1"].ToString();  //대화 내용 Text
                StoryName[i] = data_Dialog[i]["Name"].ToString();   //대화 중인 캐릭터 이름 Text
        }

    }
}
