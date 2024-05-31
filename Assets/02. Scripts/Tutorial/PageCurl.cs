using UnityEngine;

//[ExecuteInEditMode]
public class PageCurl : MonoBehaviour
{/*
    public Transform front;
    public Transform mask;
    public Transform gradientOuter;
    public Vector2 position = new Vector2(-3000f, 0f);

    private void LateUpdate()
    {
        transform.position = Vector2.zero;
        transform.eulerAngles = Vector3.zero;

        Vector2 pos = front.localPosition;
        float theta = Mathf.Atan2(pos.y, pos.x) * 180f / Mathf.PI;

        if(theta <= 0f || theta >= 90f)
        {
            return;
        }

        float deg = -(90f - theta) * 2f;
        front.eulerAngles = new Vector3(0f, 0f, deg);

        mask.position = (transform.position + front.position) * 0.5f;
        mask.eulerAngles = new Vector3(0f, 0f, deg * 0.5f);

        gradientOuter.position = mask.position;
        gradientOuter.eulerAngles = new Vector3(0f, 0f, deg * 0.5f + 90f);

        transform.localPosition = Vector2.zero;
        transform.eulerAngles = Vector3.zero;
    }*/

    public Transform backPage;
    public Transform mask;
    public Transform frontPage;

    public Vector2 point;
    public Vector3 corner = new Vector3(600f, -450f, 0f);

    public void Awake()
    {
        backPage = transform.GetChild(5).GetChild(0).GetChild(0).GetChild(1);
        frontPage = transform.GetChild(5).GetChild(0).GetChild(0).GetChild(0);
        mask = transform.GetChild(5).GetChild(0);
        
        corner += transform.position;

        Debug.Log(corner);
        Debug.Log(backPage.transform.position);
    }

    public void Update()
    {
        CurlPage();
    }

    public void CurlPage()
    {
        point = backPage.transform.position;

        // x, y 계산
        float x = corner.x - point.x;
        float y = point.y - corner.y;

        // 세타(각도) 계산, 단위는 Degree(도)
        // x == 0인 경우를 처리하기 위해, Atan이 아닌 Atan2를 쓴다.
        float theta = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        // BackPage, FrontPage가 Mask에 영향받아 움직이지 않게, 미리 위치를 캐싱해둔다.
        Vector3 firstFrontPagePosition = frontPage.position;
        Vector3 firstBackPagePosition = backPage.position;

        // Mask의 이동할 거리 계산 및 이동
        float maskX = (Vector2.Distance(point, corner) / 2) / Mathf.Cos(theta * Mathf.Deg2Rad);
        mask.position = corner - new Vector3(maskX, 0f, 0f);
        // Mask 회전
        mask.rotation = Quaternion.Euler(0f, 0f, -theta);

        // BackPage, FrontPage 위치를 원래대로 바꾼다.
        backPage.position = firstBackPagePosition;
        // BackPage의 회전은 계산한 결과대로 변경
        backPage.rotation = Quaternion.Euler(0f, 0f, -2 * theta);

        // FrontPage는 위치, 회전 고정
        frontPage.position = firstFrontPagePosition;
        frontPage.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
}
