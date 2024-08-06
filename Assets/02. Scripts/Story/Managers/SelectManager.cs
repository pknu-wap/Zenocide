using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    public static SelectManager Instance { get; private set; }
    private void Awake() => Instance = this;

    [Header("선택지 오브젝트")]
    public Choice[] choices = new Choice[4];

    [Header("선택지 갯수 저장 변수")]
    public int choiceCount = 4;

    [Header("선택지 결과 저장 변수")]
    public int result = -1;

    public IEnumerator DisplayChoices(Dictionary<string, object> csvData)
    {
        // 선택 결과 초기화
        ResetChoice();

        int choiceCount = (int)csvData["Choice Count"];
        // 필요한 선택지 개수만큼 반복
        for (int i = 0; i < choiceCount; i++)
        {
            choices[i].EnableChoiceObject();

            // 선택지 대사를 받아온다.
            string choiceText = csvData["Choice" + (i + 1)].ToString();
            // 요구하는 모든 아이템을 받아온다. 
            string[] requireItems = csvData["Require Item" + (i + 1)].ToString().Split('#');
            
            // 선택지 버튼을 갱신한다
            choices[i].UpdateText(choiceText, requireItems);
        }

        // 남는 선택지는 비활성화
        for(int i = choiceCount; i < choices.Length; ++i)
        {
            choices[i].DisableChoiceObject();
        }

        // 선택될 때까지 대기
        while (result == -1)
        {
            // 스킵 요청이 들어오면
            if (TutorialManager.Instance.isSkip == true)
            {
                // 선택지를 숨기고
                for (int i = 0; i < choices.Length; ++i)
                {
                    choices[i].DisableChoiceObject();
                }
                
                // 스킵한다.
                yield break;
            }

            yield return null;
        }

        // 선택이 끝나면 선택지들을 가린다.
        for (int i = 0; i < choices.Length; ++i)
        {
            choices[i].DisableChoiceObject();
        }
    }

    public void ResetChoice()
    {
        result = -1;
    }
    public void SelectChoice(int i)
    {
        result = i;
    }
}