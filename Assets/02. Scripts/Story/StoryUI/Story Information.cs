using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class StoryInformation : MonoBehaviour
{
    public static StoryInformation Instance { get; set; }
    
    public GameObject StoryInformationObject;
    public TMP_Text StoryText;
    private bool isShowing = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(this);
        }
        //스토리 이름 오브젝트 할당
        StoryText = StoryInformationObject.transform.GetChild(0).GetComponent<TMP_Text>();
        //오브젝트의 가로 크기를 0으로 설정
        StoryInformationObject.transform.DOScaleX(0, 0);  
    }

    //오브젝트를 버튼을 사용하여 테스트 하기 위한 테스트 함수
    public void ShowStoryInformation()
    {
        StartCoroutine(ShowInformation(2f,0.5f,"스토리 정보"));
    }

    public IEnumerator ShowInformation(float duration,float delay,string text)
    {
        //함수가 중복 실행되거나 이벤트 이름이 null일 경우 함수 종료
        if(isShowing || text is null)
        {
            yield break;
        }
        //함수 실행 중임을 표시하는 변수
        isShowing = true;
        //FadeIn 효과가 끝나기 전까지 대기
        yield return new WaitForSeconds(delay);
        //스토리 정보가 출력되는 오브젝트 활성화
        StoryInformationObject.SetActive(true);
        //오브젝트의 X 크기를 duration/4 초 동안 1로 변경되는 효과 부여
        StoryInformationObject.transform.DOScaleX(1, duration/8);
        //스토리 이름을 duration/8 초 동안 타이핑 효과를 추가하여 출력
        StartCoroutine(DoText(StoryText, text, duration/4));
        //duration/2 초 대기
        yield return new WaitForSeconds(duration/2);
        //스토리 이름을 null로 초기화하여 자연스러운 효과 연출
        StoryText.text = null;
        //오브젝트의 X 크기를 duration/2 초 동안 0으로 변경되는 효과 부여
        StoryInformationObject.transform.DOScaleX(0, duration/2);
        //duration/2 초 대기
        yield return new WaitForSeconds(duration/2);
        //자원 절약을 위해 오브젝트 비활성화
        StoryInformationObject.SetActive(false);
        //함수가 종료 되었으므로 변수 값 변경
        isShowing = false;
    }

    //TMP_Pro의 경우 Dotween의 DOText를 사용할 수 없어서 직접 구현(Dotween Pro 버전은 가능하다고 함)
    IEnumerator DoText(TMP_Text text, string endValue, float duration)
    {
        string tempString = null;
        WaitForSeconds charPerTime = new WaitForSeconds(duration / endValue.Length);

        for (int i = 0; i < endValue.Length; i++)
        {
            tempString += endValue[i];
            text.text = tempString;

            yield return charPerTime;
        }
    }
}
