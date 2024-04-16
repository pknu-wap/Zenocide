using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text dialogueText;        //Story Text
    public TMP_Text dialogueName;        //Story Name
      
    public GameObject dialogueBox;       //전체 Canvas
    public GameObject WaitCursor;        //다음 Text 대기 표시 커서
      
    public Image dialogueImage;          //일러스트
          
    public Sprite[] dialogueImages;      //일러스트 목록
      
    public string[] StoryText;           //Story Text 배열
    public string[] StoryName;           //Story Name 배열
      
    public int currentLine;              //현재 출력 중인 문자열 위치
      
    private bool isTyping = false;       //타이핑 효과 진행 여부 확인 변수
    private bool cancelTyping = false;   //

    void Start()
    {
        dialogueBox.SetActive(false);    //시작 시 Canvas 전체 비활성화
        LoadDialogue();                  //Story Name,Text 불러오기
        ShowDialogue();                  //이미지와 전체 Canvas 표시
    }

    void Update()
    {
        if (dialogueBox.activeInHierarchy)
        {
            if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)){
                if (!isTyping)
                {
                    currentLine++;
                    if (currentLine < StoryText.Length)
                    {
                        StartCoroutine(TypeSentence(StoryText[currentLine]));
                        dialogueName.text = StoryName[currentLine];
                        // 이미지 변경
                        if (currentLine < dialogueImages.Length)
                        {
                            dialogueImage.sprite = dialogueImages[currentLine];
                        }
                    }
                    else
                    {
                        dialogueBox.SetActive(false);
                    }
                }
                else if (isTyping && !cancelTyping)
                {
                    cancelTyping = true;
                }
            }
        }
    }

    //Story Text에 타이핑 효과 추가하는 함수
    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        WaitCursor.SetActive(false);
        cancelTyping = false;
        dialogueText.text = "";
        int posX = 0;
        int posY = 0;
        foreach (char letter in sentence.ToCharArray())
        {
            posX += 1;
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.03f);
            if (cancelTyping)
            {
                dialogueText.text = sentence;
                break;
            }
        }
        isTyping = false;
        if(posX >= 110){
            posY = posX / 110;
            posX %= 110;
            posX += 2;
        }
        float textWidth = 33 * posX;
        Debug.Log(textWidth);
        Vector3 newPosition = dialogueText.transform.position + new Vector3(textWidth,-(5 + 60 * posY),0);
        WaitCursor.transform.position = newPosition;
        WaitCursor.SetActive(true);
        cancelTyping = false;
    }

    //초기 화면 표시
    public void ShowDialogue()
    {
        currentLine = -1;
        dialogueBox.SetActive(true);
        // 이미지 초기화
        dialogueImage.sprite = dialogueImages[0];
    }

    //TXT 파일에서 Story Text, Name 불러오는 함수
    void LoadDialogue()
    {
        //파일 저장 경로
        string FilePath = "Assets/Resources/StoryScript.txt";

        StreamReader reader = new StreamReader(FilePath);

        string fileContent = reader.ReadToEnd();

        StoryText = fileContent.Split('\n');

        StoryName = new string[StoryText.Length];

        for (int i = 0; i < StoryText.Length; i++)
        {
            string[] Temp = StoryText[i].Split('#');
            StoryText[i] = Temp[1]; // 대화 문장 저장
            StoryName[i] = Temp[0]; // 대화 이름 저장
        }

        reader.Close();
    }
}
