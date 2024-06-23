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
    public Vector3 startPosition;
    // 베지에 곡선 중간 좌표
    public Transform middlePosition;

    // 부모 화살표 오브젝트, 시작 위치는 transform을 기준으로 한다.

    public void Start()
    {
        startPosition = transform.position;

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

    public void MoveStartPosition(Vector2 position)
    {
        startPosition = position;
    }

    public void MoveArrow(Vector2 targetPosition)
    {
        // for문 안에 예외 처리를 하지 않으려 밖으로 뺐다. (회전을 안 주기 위함
        points[0].transform.position = startPosition;

        for(int i = 1; i < numberOfPoints + 1; ++i)
        {
            // 베지에 곡선 위, 자신의 위치를 찾아 이동한다.
            points[i].transform.position = GetBezierLerp(startPosition, middlePosition.position, targetPosition, (float)i / numberOfPoints);

            // 방향은 자기 자신의 위치 - 이전 포인트의 위치로 결정한다.
            Vector2 delta = points[i].transform.position - points[i - 1].transform.position;
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

            points[i].transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }

    Vector2 GetBezierLerp(Vector2 start, Vector2 middle, Vector2 end, float t)
    {
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * start
            + 2f * oneMinusT * t * middle
            + t * t * end;
    }

    public void ShowArrow()
    {
        gameObject.SetActive(true);
    }

    public void HideArrow()
    {
        gameObject.SetActive(false);
    }
    #endregion 화살표 생성
}
