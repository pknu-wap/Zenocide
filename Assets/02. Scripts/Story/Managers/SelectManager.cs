using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
        // 선택지 초기화
        ResetChoice();

        int choiceCount = (int)csvData["ChoiceCount"];
        // 필요한 선택지 개수만큼 반복
        for (int i = 0; i < choiceCount; i++)
        {
            choices[i].EnableChoiceObject();
            choices[i].EnableInteractable();

            string choiceText = csvData["Choice" + (i + 1)].ToString();
            string requireItem = csvData["RequireItem" + (i + 1)].ToString();

            choices[i].UpdateText(choiceText, requireItem);
        }

        // 남는 선택지는 비활성화
        for(int i = choiceCount; i < choices.Length; ++i)
        {
            choices[i].DisableChoiceObject();
        }

        // 선택될 때까지 대기
        while (result == -1)
        {
            yield return null;
        }

        // 선택이 끝나면 선택지들을 가린다.
        for (int i = 0; i < choices.Length; ++i)
        {
            choices[i].DisableChoiceObject();
        }
        yield return true;
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