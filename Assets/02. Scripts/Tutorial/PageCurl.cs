using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PageCurl : MonoBehaviour
{
    // 전체 페이지 
    private Transform[] pages;

    // 구성 요소 (앞면, 뒷면, 마스크)
    private Transform[] mask;
    private Transform[] frontPage;
    private Transform[] backPage;
    private Transform[] gradient;

    // 넘김 효과가 실행 중인가?
    public bool isCurling = false;
    // 넘길 페이지 번호. 한 장 넘긴 후 증가시켜 다음 장을 넘긴다.
    private int pageNumber = 0;
    private GameObject waitCursor;

    // 책의 너비와 높이
    private float bookWidth;
    private float bookHeight;

    // 페이지의 꼭짓점과 책의 꼭짓점
    private Vector2 point;
    [SerializeField] private Vector3 corner;

    // BackPage의 시작 위치, corner와 다를 시 정상적으로 작동하지 않는다.
    [SerializeField] private Vector3 firstBackPagePosition;

    // DOTween 애니메이션 관련
    public Transform curlPoint;
    public float flipTime = 0.5f;

    public void Awake()
    {
        // 배열 초기화
        pages = new Transform[transform.childCount];
        mask = new Transform[transform.childCount];
        frontPage = new Transform[transform.childCount];
        backPage = new Transform[transform.childCount];
        gradient = new Transform[transform.childCount];
        
        // 배열에 자식들을 불러온다.
        for(int i = 0; i < transform.childCount; ++i)
        {
            // 맨 아래의 자식부터 불러온다.
            pages[i] = transform.GetChild((transform.childCount - 1) - i);

            // 나머진 pages를 기준으로 불러오니 i가 들어갔다.
            mask[i] = pages[i].GetChild(0);
            frontPage[i] = mask[i].GetChild(0);
            backPage[i] = mask[i].GetChild(1);
            gradient[i] = backPage[i].GetChild(0);
        }

        // 대기 커서를 불러온다.
        waitCursor = transform.parent.GetChild(3).gameObject;

        // 책의 너비와 높이를 구한다.
        RectTransform bookRect = GetComponent<RectTransform>();
        bookWidth = bookRect.rect.width;
        bookHeight = bookRect.rect.height;

        // 코너를 책 위치 기준으로 해야 하니 책 위치에 더해준다.
        corner = transform.position + new Vector3(bookWidth / 4, -bookHeight / 4, 0f);

        // 시작 위치를 미리 받아둔다. (corner와 같아야 함)
        firstBackPagePosition = backPage[0].transform.position;
    }

    public void LateUpdate()
    {
        // Curl 효과가 동작하는 동안
        if (isCurling)
        {
            // 넘김 효과를 실행한다.
            CurlPage(pageNumber);
        }
    }
    
    // 페이지를 넘긴다.
    public IEnumerator FlipPage()
    {
        if (isCurling || pageNumber >= transform.childCount)
        {
            yield break;
        }

        isCurling = true;
        waitCursor.SetActive(false);
        yield return StartCoroutine(MoveBackPage(pageNumber));
        waitCursor.SetActive(true);
    }

    // 페이지를 움직인다.
    private IEnumerator MoveBackPage(int i)
    {
        // 틀어질 경우를 대비해, 시작 위치로 이동시킨다.
        backPage[i].transform.position = firstBackPagePosition;

        // 위치를 포물선으로 이동시킨다.
        Sequence move = DOTween.Sequence()
            .Append(backPage[i].transform.DOMoveX(curlPoint.position.x, flipTime / 2).SetEase(Ease.Linear))
            .Join(backPage[i].transform.DOMoveY(curlPoint.position.y, flipTime / 2).SetEase(Ease.OutCubic))
            .Append(backPage[i].transform.DOMoveX(firstBackPagePosition.x - bookWidth / 2, flipTime / 2).SetEase(Ease.Linear))
            .Join(backPage[i].transform.DOMoveY(firstBackPagePosition.y, flipTime / 2).SetEase(Ease.InCubic));

        // 끝날 때까지 기다린다.
        yield return move.WaitForCompletion();

        // 위치를 딱 맞춘다. (Snapping)
        mask[i].position = corner;
        mask[i].rotation = Quaternion.Euler(Vector3.zero);

        frontPage[i].position = corner - new Vector3(bookWidth / 2, 0f, 0f);
        frontPage[i].rotation = Quaternion.Euler(Vector3.zero);

        backPage[i].position = corner - new Vector3(bookWidth / 2, 0f, 0f);
        backPage[i].rotation = Quaternion.Euler(Vector3.zero);

        gradient[i].position = corner - new Vector3(bookWidth / 4, 0f, 0f);
        gradient[i].rotation = Quaternion.Euler(Vector3.zero);

        // Curling 종료, 넘길 페이지 번호 증가, 다음 장을 맨 위로 올린다.
        isCurling = false;
        pageNumber++;
        pages[pageNumber].SetAsLastSibling();
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
        Vector3 originFrontPagePosition = frontPage[i].position;
        Vector3 originBackPagePosition = backPage[i].position;

        // 오브젝트 이동. 부모 오브젝트부터 자식 오브젝트 순으로 위치를 변경해야 한다.
        // Mask의 이동할 거리 계산
        float maskX = (Vector2.Distance(point, corner) / 2) / Mathf.Cos(theta * Mathf.Deg2Rad);

        // Mask 이동 및 회전
        mask[i].position = corner - new Vector3(maskX, 0f, 0f);
        mask[i].rotation = Quaternion.Euler(0f, 0f, -theta);

        // FrontPage는 위치, 회전 고정
        frontPage[i].position = originFrontPagePosition;
        frontPage[i].rotation = Quaternion.Euler(Vector3.zero);

        // BackPage의 회전은 계산한 결과대로 변경, 위치는 원래대로
        backPage[i].position = originBackPagePosition;
        backPage[i].rotation = Quaternion.Euler(0f, 0f, -2 * theta);

        // gradient도  Mask와 같게 이동 및 회전
        gradient[i].position = corner - new Vector3(maskX, 0f, 0f);
        gradient[i].rotation = Quaternion.Euler(0f, 0f, -theta);

        // 음영을 활성화한다.
        gradient[i].gameObject.SetActive(true);
    }
}
