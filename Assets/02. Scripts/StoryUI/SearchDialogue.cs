using UnityEngine;
using UnityEngine.UI;


public class LoadDialogue : MonoBehaviour
{
    public TextData textData;

    [Header("선택지 데이터")]
    public string[] choiceUpContent;
    public string[] choiceDownContent;

    [Header("대화창 데이터")]
    public string[] storyText;
    public string[] storyName;
    
    [Header("적 데이터")]
    public string[] Enemys;

    // Start is called before the first frame update
    void Start()
    {
        LoadText();
    }

    //인덱스에 따라 대화와 이름을 반환
    public (string,string) IndextoText(int index)
    {
        return (textData.storyText[index], textData.storyName[index]); 
    }

    private void LoadText()
    {
        var dataDialog = CSVReader.Read("DialogueScript");

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
