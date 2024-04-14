using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text dialogueText;
    public TMP_Text dialogueName;
    public GameObject dialogueBox;
    public GameObject WaitCursor;
    public Image dialogueImage;
    public Sprite[] dialogueImages;
    public string[] StoryText;
    public string[] StoryName;
    public int currentLine;

    private bool isTyping = false;
    private bool cancelTyping = false;

    void Start()
    {
        dialogueBox.SetActive(false);
        LoadDialogue();
        ShowDialogue();
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

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        WaitCursor.SetActive(false);
        cancelTyping = false;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
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

    public void ShowDialogue()
    {
        currentLine = -1;
        dialogueBox.SetActive(true);
        // 이미지 초기화
        dialogueImage.sprite = dialogueImages[0];
    }

    void LoadDialogue()
    {
        string FilePath = "Assets/Resources/StoryScript.txt";

        StreamReader reader = new StreamReader(FilePath);

        string fileContent = reader.ReadToEnd();

        StoryText = fileContent.Split('\n');

        StoryName = new string[StoryText.Length];

        for (int i = 0; i < StoryText.Length; i++)
        {
            string[] Temp = StoryText[i].Split(',');
            StoryText[i] = Temp[1]; // 대화 문장 저장
            StoryName[i] = Temp[0]; // 대화 이름 저장
        }

        reader.Close();
    }
}
