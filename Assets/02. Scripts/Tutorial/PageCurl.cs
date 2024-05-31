using DG.Tweening;
using System;
using UnityEngine;

public class PageCurl : MonoBehaviour
{
    // 전체 페이지 
    private Transform[] pages;

    // 구성 요소 (앞면, 뒷면, 마스크)
    private Transform[] frontPage;
    private Transform[] backPage;
    private Transform[] mask;

    // 넘김 효과가 실행 중인가?
    private bool isCurling = false;
    // 넘길 페이지 번호. 한 장 넘긴 후 증가시켜 다음 장을 넘긴다.
    [SerializeField] private int pageNumber = 0;

    // 페이지의 꼭짓점과 책의 꼭짓점
    private Vector2 point;
    private Vector3 corner = new Vector3(300f, -225f, 0f);

    // BackPage의 시작 위치
    private Vector3 firstPosition;

    public void Awake()
    {
        // 배열 초기화
        pages = new Transform[transform.childCount];
        frontPage = new Transform[transform.childCount];
        backPage = new Transform[transform.childCount];
        mask = new Transform[transform.childCount];
        
        // 배열에 자식들을 불러온다.
        for(int i = 0; i < transform.childCount; ++i)
        {
            // 맨 아래의 자식부터 불러온다.
            pages[i] = transform.GetChild((transform.childCount - 1) - i);

            // 나머진 pages를 기준으로 불러오니 i가 들어갔다.
            frontPage[i] = pages[i].GetChild(0).GetChild(0).GetChild(0);
            backPage[i] = pages[i].GetChild(0).GetChild(0).GetChild(1);
            mask[i] = pages[i].GetChild(0);
        }
        
        // 코너를 책 위치 기준으로 해야 하니 책 위치를 더해준다.
        corner += transform.position;
        // 시작 위치를 미리 받아둔다.
        firstPosition = backPage[0].transform.position;

        Debug.Log(corner);
        Debug.Log(backPage[0].transform.position);
    }

    public void Update()
    {
        // Curl 효과가 동작하는 동안
        if (isCurling)
        {
            // 넘김 효과를 실행한다.
            CurlPage(pageNumber);
        }
    }
    
    // 페이지를 넘긴다.
    public void FlipPage()
    {
        if (isCurling || pageNumber >= transform.childCount)
        {
            return;
        }

        isCurling = true;

        MoveBackPage(pageNumber);
    }

    // 페이지를 움직인다.
    private void MoveBackPage(int i)
    {
        // 틀어질 경우를 대비해, 시작 위치로 이동시킨다.
        backPage[i].transform.position = firstPosition;

        // 위치를 포물선으로 이동시킨다. (개선 필요)
        DOTween.Sequence()
            .Append(backPage[i].transform.DOMoveX(firstPosition.x - 300f, 1f))
            .Join(backPage[i].transform.DOMoveY(firstPosition.y + 150f, 1f))
            .Append(backPage[i].transform.DOMoveX(firstPosition.x - 600f, 1f))
            .Join(backPage[i].transform.DOMoveY(firstPosition.y, 1f)).SetEase(Ease.OutCubic)
            // 끝나면 Curling 종료, 페이지 번호 증가, 다음 장을 제일 위로 올리기
            .OnComplete(() => { 
                isCurling = false;
                pageNumber++;
                pages[pageNumber].SetAsLastSibling();
            });
    }

    // 페이지 넘김 효과를 실행한다.
    private void CurlPage(int i)
    {
        // 책 오른쪽 페이지의 우측 하단 꼭지점.
        point = backPage[i].transform.position;

        // x, y 계산
        float x = corner.x - point.x;
        float y = point.y - corner.y;

        // 세타(각도) 계산, 단위는 Degree(도)
        // x == 0인 경우를 처리하기 위해, Atan이 아닌 Atan2를 쓴다.
        float theta = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        // BackPage, FrontPage가 Mask에 영향받아 움직이지 않게, 미리 위치를 캐싱해둔다.
        Vector3 firstFrontPagePosition = frontPage[i].position;
        Vector3 firstBackPagePosition = backPage[i].position;

        // Mask의 이동할 거리 계산 및 이동
        float maskX = (Vector2.Distance(point, corner) / 2) / Mathf.Cos(theta * Mathf.Deg2Rad);
        mask[i].position = corner - new Vector3(maskX, 0f, 0f);
        // Mask 회전
        mask[i].rotation = Quaternion.Euler(0f, 0f, -theta);

        // BackPage, FrontPage 위치를 원래대로 바꾼다.
        backPage[i].position = firstBackPagePosition;
        // BackPage의 회전은 계산한 결과대로 변경
        backPage[i].rotation = Quaternion.Euler(0f, 0f, -2 * theta);

        // FrontPage는 위치, 회전 고정
        frontPage[i].position = firstFrontPagePosition;
        frontPage[i].rotation = Quaternion.Euler(0f, 0f, 0f);
    }
}
