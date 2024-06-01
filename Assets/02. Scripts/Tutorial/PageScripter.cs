using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class PageScripter : MonoBehaviour
{
    [Header("컴포넌트")]
    public TMP_Text diaryText;

    [Header("상태 체크")]
    // 현재 타이핑 중인가?
    private bool isTyping = false;
    private bool cancelTyping = false;
    private float typingTime = 0.03f;
    private string currentDialog = "";

    public void ShowDialog(string sentence)
    {
        StartCoroutine(TypeSentence(sentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        // 대화 진행 시작
        isTyping = true;
        // 다음 대화로 넘어가기 전에 기다리는 커서 비활성화
        // waitCursor.SetActive(false);

        cancelTyping = false;
        diaryText.text = currentDialog;

        foreach (char letter in sentence)
        {
            diaryText.text += letter;

            yield return new WaitForSeconds(typingTime);

            // 타이핑 효과 취소 시 대화를 한번에 출력
            if (cancelTyping)
            {
                diaryText.text = currentDialog + sentence;
                break;
            }
        }
        // 대화 진행 종료
        isTyping = false;
        // waitCursor.SetActive(true);
        // 현재 텍스트 갱신
        currentDialog += sentence;
    }
}
