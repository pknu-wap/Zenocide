using UnityEngine;
using TMPro;
using System.IO;

public class TextBox : MonoBehaviour
{
    public TMP_Text outputText; // UI Text 오브젝트를 가리키는 변수

    private string StoryfilePath = "Assets/Resources/StoryScript.txt"; // 텍스트 파일의 경로

    void Start()
    {
        ReadStoryText();
    }

    void ReadStoryText()
    {
        // 파일을 한 줄씩 읽어오기 위해 StreamReader를 사용합니다.
        StreamReader reader = new StreamReader(StoryfilePath);

        // 파일의 끝까지 한 줄씩 읽어오면서 UI에 출력합니다.
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            outputText.text += line + "\n"; // 읽어온 문장을 UI Text에 추가합니다.
        }

        reader.Close(); // 파일을 닫습니다.
    }
}
