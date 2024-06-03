using System.Collections;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    private const int NotSelected = -1;

    [Header("선택지 오브젝트")]
    public GameObject[] Choices = new GameObject[4];

    [Header("선택지 가림막 오브젝트")]
    public GameObject[] ChoiceBlinder = new GameObject[4];

    [Header("대화창 가림막 오브젝트")]
    public GameObject DialogueBlinder;

    [Header("선택지 갯수 저장 변수")]
    public int choiceCount = 4;

    [Header("선택지 결과 저장 변수")]
    public int result = NotSelected;

    void Update()
    {
        CheckChoice();
    }

    public void CheckChoice()
    {
        for (int i = 0; i < choiceCount; i++)
        {
            if (Choices[i].GetComponent<Choice>().selected)
            {
                DisableChoices();
                result = i;
            }
        }
    }   

    public void ResetChoice()
    {
        for (int i = 0; i < choiceCount; i++)
        {
            Choices[i].GetComponent<Choice>().selected = false;
        }
        result = NotSelected;
    }

    public void DisableChoices()
    {
        for (int i = 0; i < choiceCount; i++)
        {
            Choices[i].SetActive(false);
            ChoiceBlinder[i].SetActive(false);
        }
        DialogueBlinder.SetActive(false);
    }

}