using UnityEngine;
using System.Collections;

public class CamShake : MonoBehaviour
{
    public static CamShake Instance { get; set; }

    public Canvas canvas;         // 카메라가 비추는 캔버스
    private Vector3 originalPos;  // 카메라 원래 위치

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        originalPos = transform.localPosition; // 초기 위치를 저장
    }

    public void Shake(float duration = 0.2f, float magnitude = 5f)
    {
        canvas.renderMode = RenderMode.WorldSpace;        // 캔버스 렌더링 모드를 월드로 변경(기본값은 스크린)
        StartCoroutine(CoroutineShake(duration, magnitude));
    }

    public IEnumerator CoroutineShake(float duration, float magnitude)
    {
        while(duration > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * magnitude; // 랜덤하게 흔들기
            duration -= Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;            // 원래 위치로 복구
        canvas.renderMode = RenderMode.ScreenSpaceCamera; // 캔버스 렌더링 모드를 기본값(스크린)으로 변경
    }

}