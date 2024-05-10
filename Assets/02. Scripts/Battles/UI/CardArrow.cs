using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        gameObject.SetActive(true);
    }

    public void MoveArrow(Vector2 targetPosition)
    {
        for(int i = 0; i < numberOfPoints + 1; ++i)
        {
            points[i].transform.position = Vector2.Lerp(transform.position, targetPosition, (float)i / numberOfPoints);
        }
    }

    public void HideArrow()
    {
        gameObject.SetActive(false);
    }
    #endregion 화살표 생성
}
