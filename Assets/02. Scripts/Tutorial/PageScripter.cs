using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PageScripter : MonoBehaviour
{
    [Header("컴포넌트")]
    public TMP_Text diaryText;
    public PageCurl book;
    public GameObject waitCursor;

    [Header("상태 체크")]
    // 현재 타이핑 중인가?
    public bool isTyping = false;
    public bool cancelTyping = false;
    // 현재까지 적힌 텍스트
    [SerializeField] private string currentDialog = "";
    // 선택된 단어
    [SerializeField] private string selectedWord;

    [Header("타이핑 시간")]
    private const float typingTime = 0.03f;
    private Dictionary<char, float> markTypingTime = new Dictionary<char, float>
    {
        { ',', 0.1f },
        { '.', 0.5f },
        { '?', 0.5f },
        { '!', 0.5f },
    };

    private void Awake()
    {
        diaryText = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        book = transform.parent.GetComponent<PageCurl>();
        waitCursor = transform.parent.parent.GetChild(3).gameObject;
    }

    // 스크립트를 보여준다.
    public IEnumerator ShowDialog(Dictionary<string, object> line)
    {
        // 대화 진행 시작
        isTyping = true;

        string sentence = line["내용"].ToString();

        // #일 경우 페이지를 넘긴다.
        if(sentence == "#")
        {
            yield return StartCoroutine(book.FlipPage());
            DiaryManager.Instance.currentPageIndex++;
        }

        // $일 경우 튜토리얼을 종료한다.
        if (sentence == "$")
        {
            DiaryManager.Instance.FinishDiary();
            yield break;
        }

        yield return StartCoroutine(TypeSentence(sentence));
    }

    // 문장을 타이핑한다.
    private IEnumerator TypeSentence(string sentence)
    {
        // 다음 대화로 넘어가기 전에 기다리는 커서 비활성화
        waitCursor.SetActive(false);

        cancelTyping = false;
        diaryText.text = currentDialog;

        // 이번 문장에 선택지가 있는가?
        bool hasSelect = sentence.Contains('%');
        // 이번 턴에 선택지가 골라졌는가?
        bool isSelectHappened = false;

        for (int i = 0; i < sentence.Length; ++i)
        {
            // 한 글자만 떼온다.
            string letter = "";
            letter += sentence[i];

            // 해당 글자가 %라면
            if(letter == "%")
            {
                // 선택이 있었음을 체크한다.
                isSelectHappened = true;

                // 선택지를 띄우고, 응답을 기다린다.
                yield return StartCoroutine(DiaryManager.Instance.ShowChoiceButton());

                // letter는 띄어쓰기로
                letter = " ";
                diaryText.text += selectedWord;
            }

            if(letter == "<")
            {
                ++i;
                while (sentence[i] != '>')
                {
                    letter += sentence[i];
                    ++i;
                }
                if (sentence[i] == '>')
                {
                    letter += sentence[i];
                }
            }

            // letter를 추가하고
            diaryText.text += letter;

            // delay를 준다.
            float delay = typingTime;
            // 문장 부호로 끝난다면 더 길게 준다.
            if (markTypingTime.ContainsKey(sentence[i]))
            {
                delay = markTypingTime[sentence[i]];
            }

            yield return new WaitForSeconds(delay);

            // 타이핑 효과 취소 시 대화를 한번에 출력
            if (cancelTyping)
            {
                // 선택이 일어났다면
                if (isSelectHappened)
                {
                    // 선택된 단어로 변경한다.
                    diaryText.text = currentDialog + sentence.Replace("%", selectedWord + ' ');
                }
                // 선택은 하지 않았으나
                else
                {
                    // 선택지가 있는 문장이라면
                    if (hasSelect)
                    {
                        // 캔슬을 무시한다.
                        continue;
                    }

                    // 그 외엔 문장을 즉시 출력한다.
                    diaryText.text = currentDialog + sentence;
                }
                break;
            }
        }

        // waitCursor 켜기
        waitCursor.SetActive(true);

        // 현재 텍스트 갱신
        currentDialog = diaryText.text + "\n";
        // 다음 줄로 이동
        DiaryManager.Instance.currentDialogIndex++;

        // 대화 진행 종료
        isTyping = false;
    }

    // 선택한다.
    public void Select(string word)
    {
        selectedWord = word;
    }

    // 타이핑 효과를 취소한다.
    public void CancelTyping()
    {
        cancelTyping = true;
    }
}
