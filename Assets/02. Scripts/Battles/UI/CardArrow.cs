using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CardArrow : MonoBehaviour
{
    #region 싱글톤
    public static CardArrow Instance { get; private set; }
    void Awake() => Instance = this;
    #endregion 싱글톤

    #region 화살표 생성
    public GameObject[] points;

    [Header("화살표 정보")]
    // 화살표 속 몸통 개수
    public int numberOfPoints;
    // 몸통 간격
    public float space;
    // 몸통 프리팹
    public GameObject point;
    // 끝부분 프리팹
    public GameObject arrow;
    // 베지에 곡선 중간 좌표
    public Transform middleTr;

    // 마우스 이벤트를 막는 오브젝트
    public GameObject mouseEventBlocker;

    // 부모 화살표 오브젝트, 시작 위치는 transform을 기준으로 한다.

    public void Start()
    {
        points = new GameObject[numberOfPoints + 1];

        // 몸통을 numberOfPoints 개 생성하고
        for(int i = 0; i < numberOfPoints; ++i)
        {
            points[i] = Instantiate(point, transform);
        }

        // 끝부분도 생성한다.
        points[numberOfPoints] = Instantiate(arrow, transform);

        // 시작 설정은 숨김 상태다.
        HideArrow();
    }

    public void ShowArrow()
    {
        ShowBlocker();
        gameObject.SetActive(true);
    }

    public void MoveArrow(Vector2 targetPosition)
    {
        for(int i = 1; i < numberOfPoints + 1; ++i)
        {
            // 베지에 곡선 위, 자신의 위치를 찾아 이동한다.
            points[i].transform.position = GetBezierLerp(transform.position, middleTr.position, targetPosition, (float)i / numberOfPoints);

            // 방향은 자기 자신의 위치 - 이전 포인트의 위치로 결정한다.
            Vector2 delta = points[i].transform.position - points[i - 1].transform.position;
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

            points[i].transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }

    public void HideArrow()
    {
        gameObject.SetActive(false);
        HideBlocker();
    }

    Vector2 GetBezierLerp(Vector2 start, Vector2 middle, Vector2 end, float t)
    {
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * start
            + 2f * oneMinusT * t * middle
            + t * t * end;
    }
    #endregion 화살표 생성

    #region 이벤트 블로커
    public void ShowBlocker()
    {
        mouseEventBlocker.SetActive(true);
    }

    public void HideBlocker()
    {
        mouseEventBlocker.SetActive(false);
    }
    #endregion 이벤트 블로커
}
