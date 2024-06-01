using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PageScripter : MonoBehaviour
{
    [Header("컴포넌트")]
    public TMP_Text diaryText;

    [Header("상태 체크")]
    // 현재 타이핑 중인가?
    private bool isTyping = false;
    private bool cancelTyping = false;
    // 현재까지 적힌 텍스트
    private string currentDialog = "";
    // 현재 진행 중인 행
    private Dictionary<string, object> currentLine;
    // 선택된 단어
    [SerializeField] private string selectedWord;

    [Header("타이핑 시간")]
    private const float typingTime = 0.03f;
    private const float meetMarkTime = 0.5f;

    private void Awake()
    {
        diaryText = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
    }

    public bool ShowDialog(Dictionary<string, object> line)
    {
        if (isTyping)
        {
            // 실패했음을 알린다.
            return false;
        }

        currentLine = line;
        string sentence = line["내용"].ToString();

        StartCoroutine(TypeSentence(sentence));
        // 성공했음을 알린다.
        return true;
    }

    private IEnumerator TypeSentence(string sentence)
    {
        // 대화 진행 시작
        isTyping = true;
        // 다음 대화로 넘어가기 전에 기다리는 커서 비활성화
        // waitCursor.SetActive(false);

        cancelTyping = false;
        diaryText.text = currentDialog;

        for (int i = 0; i < sentence.Length; ++i)
        {
            // 한 글자만 떼온다.
            char letter = sentence[i];

            // 해당 글자가 %라면
            if(letter == '%')
            {
                // 선택지를 띄우고, 응답을 기다린다.
                yield return StartCoroutine(DiaryManager.Instance.ShowChoiceButton());

                letter = '\0';
                diaryText.text += selectedWord;
            }

            // letter를 추가하고
            diaryText.text += letter;

            // delay를 준다.
            float delay = typingTime;
            // 문장 부호로 끝난다면 더 길게 준다.
            if (letter == ',' || letter == '.')
            {
                delay = meetMarkTime;
            }

            yield return new WaitForSeconds(delay);

            // 타이핑 효과 취소 시 대화를 한번에 출력
            if (cancelTyping)
            {
                diaryText.text = currentDialog + sentence + "\n";
                break;
            }
        }

        // 대화 진행 종료
        isTyping = false;
        // waitCursor.SetActive(true);
        // 현재 텍스트 갱신
        currentDialog += sentence + "\n";
    }

    public void Select(string word)
    {
        selectedWord = word;
    }

    // 타이핑 효과를 취소한다.
    public void CancelTyping()
    {
        cancelTyping = false;
    }
}
