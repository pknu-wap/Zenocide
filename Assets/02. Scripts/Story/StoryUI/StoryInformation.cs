using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

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
        StartCoroutine(ShowInformation(2f,"스토리 정보"));
    }

    public IEnumerator ShowInformation(float duration,string text)
    {
        //함수가 중복 실행되거나 이벤트 이름이 null일 경우 함수 종료
        if(isShowing || text is null)
        {
            yield break;
        }
        //함수 실행 중임을 표시하는 변수
        isShowing = true;
        //스토리 정보가 출력되는 오브젝트 활성화
        StoryInformationObject.SetActive(true);
        //스토리 이름을 받아와서 텍스트에 출력
        StoryText.text = text;
        //오브젝트의 X 크기를 duration/2 초 동안 1로 변경되는 효과 부여
        yield return StoryInformationObject.transform.DOScaleX(1, duration/4).WaitForCompletion();
        //스토리 이름을 null로 초기화하여 자연스러운 효과 연출
        yield return new WaitForSeconds(duration/2);
        StoryText.text = null;
        //오브젝트의 X 크기를 duration/2 초 동안 0으로 변경되는 효과 부여
        yield return StoryInformationObject.transform.DOScaleX(0, duration/4).WaitForCompletion();
        //자원 절약을 위해 오브젝트 비활성화
        StoryInformationObject.SetActive(false);
        //함수가 종료 되었으므로 변수 값 변경
        isShowing = false;
    }

}
