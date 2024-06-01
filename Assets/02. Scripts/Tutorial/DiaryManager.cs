using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiaryManager : MonoBehaviour
{
    public static DiaryManager Instance { get; set; }

    [Header("스크립트 에셋")]
    // CSV 내용이 담겨 있는 딕셔너리 리스트
    private List<Dictionary<string, object>> diaryDialog;
    [SerializeField] private TextAsset diaryCSV;

    [Header("컴포넌트")]
    [SerializeField] private PageScripter scripter;
    [SerializeField] private Transform choiceParent;
    [SerializeField] private TMP_Text[] choiceButtons;

    [Header("현재 다이얼로그 정보")]
    // 현재 출력 중인 텍스트 번호
    [SerializeField] private int dialogIndex = 0;
    // 현재 출력 중인 페이지 번호
    private int pageIndex = 0;
    // 선택된 단어
    [SerializeField] private string selectedWord;

    private void Awake()
    {
        // 싱글톤
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        choiceButtons = new TMP_Text[choiceParent.childCount];
        for(int i = 0; i < choiceParent.childCount; ++i)
        {
            choiceButtons[i] = choiceParent.GetChild(i).GetChild(0).GetComponent<TMP_Text>();
        }
        choiceParent.gameObject.SetActive(false);

        diaryDialog = CSVReader.Read(diaryCSV);

        Debug.Log(diaryDialog[0]["내용"]);
    }

    // 문장을 넘겨준다.
    public void AddDialog()
    {
        // 출력에 성공한다면
        if (scripter.ShowDialog(diaryDialog[dialogIndex]) == true)
        {
            // 다음 줄로 이동한다.
            ++dialogIndex;
        }
    }

    public bool isSelected = false;

    public IEnumerator ShowChoiceButton()
    {
        // 선택지를 띄운다.
        for(int i = 0; i < choiceButtons.Length; ++i)
        {
            choiceButtons[i].text = diaryDialog[dialogIndex][i.ToString()].ToString();
        }
        choiceParent.gameObject.SetActive(true);

        // 선택할 때까지 기다린다.
        while (isSelected == false)
        {
            yield return null;
        }

        // scripter에 전달해준다.
        scripter.Select(selectedWord);

        // 다시 false로 돌리고 종료
        isSelected = false;
    }

    // 선택한다.
    public void Select(int i)
    {
        selectedWord = diaryDialog[dialogIndex][i.ToString()].ToString();

        choiceParent.gameObject.SetActive(false);
        isSelected = true;
    }
}
