using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class StoryInformation : MonoBehaviour
{
    public static StoryInformation Instance { get; set; }
    
    // 스토리 정보를 출력하는 메인 오브젝트
    public GameObject StoryInformationObject;
    // 스토리 이름을 출력하는 텍스트
    public TMP_Text StoryText;
    // 스토리 정보 출력 중인지 확인하는 변수
    private bool isShowing = false;
    // 오브젝트의 위치를 저장하는 변수
    private Vector3 targetPostion;
    // 오브젝트의 가로 길이를 저장하는 변수
    private float objectWidth;

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
        //메인 오브젝트 Position 값 저장
        targetPostion = StoryInformationObject.transform.position;
        //메인 오브젝트의 가로 길이 저장
        objectWidth = StoryInformationObject.transform.GetComponent<RectTransform>().rect.width;
        //메인 오브젝트를 오브젝트의 가로 길이 만큼 이동하여 캔버스 바깥으로 이동
        StoryInformationObject.transform.position = new Vector3(targetPostion.x - objectWidth/2, targetPostion.y, targetPostion.z);
        //오브젝트 비활성화
        StoryInformationObject.SetActive(false);   
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
        //목표 X 좌표 값으로 이동하여 캔버스에서 나타나는 효과 부여
        yield return StoryInformationObject.transform.DOMoveX(targetPostion.x, duration).WaitForCompletion();
        // 1 초 동안 스토리 이름 표시
        yield return new WaitForSeconds(1);
        //Target 좌표에서 오브젝트의 가로 길이를 뺀 만큼 이동하여 캔버스에서 사라지는 효과 부여
        yield return StoryInformationObject.transform.DOMoveX(targetPostion.x - objectWidth/2, duration).WaitForCompletion();
        //자원 절약을 위해 오브젝트 비활성화
        StoryInformationObject.SetActive(false);
        //함수가 종료 되었으므로 변수 값 변경
        isShowing = false;
    }

}
