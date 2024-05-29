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
    public Transform backMask;

    public Vector2 point;
    public Vector3 corner = new Vector3(600f, -450f, 0f);

    public void Awake()
    {
        backPage = transform.GetChild(0).GetChild(0);
        corner += transform.localPosition;
        Debug.Log(corner);
        Debug.Log(backPage.transform.localPosition);
    }

    public void LateUpdate()
    {
        point = backPage.transform.localPosition;

        // x, y 계산
        float x = corner.x - point.x;
        float y = point.y - corner.y;

        // 세타 계산
        // x == 0인 경우를 처리하기 위해, Atan이 아닌 Atan2를 쓴다.
        float theta = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        // BackPage 변경
        backPage.rotation = Quaternion.Euler(0f, 0f, -2 * theta);

        // BackMask 변경
        backMask.rotation = Quaternion.Euler(0f, 0f, -theta);

        // backMask의 이동할 거리 계산
        float backMaskX = (Vector2.Distance(point, corner) / 2) / Mathf.Abs(Mathf.Cos(theta * Mathf.Deg2Rad));
        backMask.localPosition = corner - new Vector3(backMaskX, 0f, 0f);
    }
}
