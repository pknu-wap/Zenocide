using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.IO;
using TMPro;
using System.Collections.Generic;


public class DialogueManager : MonoBehaviour, IPointerDownHandler
{

    public TMP_Text dialogueText;        //Story Text
    public TMP_Text dialogueName;        //Story Name

    public TMP_Text ChoiceUpText;        //Up Selection Text
    public TMP_Text ChoiceDownText;      //Down Selection Text

    public TMP_Text ChoiceUpRequireText;       //Up Selection Text
    public TMP_Text ChoiceDownRequireText;     //Down Selection Text

    public GameObject dialogueBox;       //? „ì²? Canvas

    public GameObject Dialogue;          //????™”ì°?

    public GameObject ChoiceUp;          //?œ„ ?„ ?ƒì§? ?‘œ?‹œ
    public GameObject ChoiceDown;         //?•„?˜ ?„ ?ƒì§? ?‘œ?‹œ

    public GameObject WaitCursor;        //?‹¤?Œ Text ???ê¸? ?‘œ?‹œ ì»¤ì„œ
      
    public Image dialogueImage;          //?¼?Ÿ¬?Š¤?Š¸
          
    public Sprite[] dialogueImages;      //?¼?Ÿ¬?Š¤?Š¸ ëª©ë¡
      
    public string[] StoryText;           //Story Text ë°°ì—´
    public string[] StoryName;           //Story Name ë°°ì—´
      
    public int currentLine;              //?˜„?¬ ì¶œë ¥ ì¤‘ì¸ ë¬¸ì?—´ ?œ„ì¹?
      
    private bool isTyping = false;       //????´?•‘ ?š¨ê³? ì§„í–‰ ?—¬ë¶? ?™•?¸ ë³??ˆ˜
    private bool cancelTyping = false;   //

    void Start()
    {
        dialogueBox.SetActive(false);    //?‹œ?‘ ?‹œ Canvas ? „ì²? ë¹„í™œ?„±?™”
        ChoiceUp.SetActive(false);      //?‹œ?‘ ?‹œ ?„ ?ƒì§? ë¹„í™œ?„±?™”
        ChoiceDown.SetActive(false);    
        LoadDialogue();                  //Story Name,Text ë¶ˆëŸ¬?˜¤ê¸?
        ShowDialogue();                  //?´ë¯¸ì????? ? „ì²? Canvas ?‘œ?‹œ
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
            {"Á»ºñ",1}   
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

        if(StoryName[currentLine][0] == '*'){
            Selection();
        }
        else
        {
            // ?´ë¯¸ì?? ë³?ê²?
            dialogueImage.sprite = dialogueImages[IllustTable[dialogueName.text]];
            StartCoroutine(TypeSentence(StoryText[currentLine]));
        }

    }

    //Story Text?— ????´?•‘ ?š¨ê³? ì¶”ê???•˜?Š” ?•¨?ˆ˜
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
        ChoiceUpText.text = StoryName[currentLine].Substring(1);
        ChoiceDownText.text = StoryText[currentLine].Substring(1);
        if(Items.items.Find(x => x == "ÃÑ") != null){
        
        ChoiceDownRequireText.text = "ÇÊ¿äÇÑ ¾ÆÀÌÅÛ : <color=red>ÃÑ</color>";
        
        }
        else{

        ChoiceDownRequireText.text = "ÇÊ¿äÇÑ ¾ÆÀÌÅÛ : ÃÑ";

        }
    }

    //ì´ˆê¸° ?™”ë©? ?‘œ?‹œ
    public void ShowDialogue()
    {
        currentLine = -1;
        dialogueBox.SetActive(true);
        // ?´ë¯¸ì?? ì´ˆê¸°?™”
        dialogueImage.sprite = dialogueImages[0];
    }

    //TXT ?ŒŒ?¼?—?„œ Story Text, Name ë¶ˆëŸ¬?˜¤?Š” ?•¨?ˆ˜
    void LoadDialogue()
    {
        //?ŒŒ?¼ ????¥ ê²½ë¡œ
        TextAsset asset = Resources.Load ("StoryScript")as TextAsset;

        string Story = asset.text;

        StringReader reader = new StringReader(Story);

        string fileContent = reader.ReadToEnd();

        StoryText = fileContent.Split('\n');

        StoryName = new string[StoryText.Length];

        for (int i = 0; i < StoryText.Length; i++)
        {
            string[] Temp = StoryText[i].Split('#');
            StoryText[i] = Temp[1]; // ????™” ë¬¸ì¥ ????¥
            StoryName[i] = Temp[0]; // ????™” ?´ë¦? ????¥
        }

        reader.Close();
    }
}
