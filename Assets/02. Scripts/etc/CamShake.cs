using UnityEngine;
using System.Collections;

public class CamShake : MonoBehaviour
{
    public static CamShake Instance { get; set; }

    public Camera tutorialCam;    // 튜토리얼씬 카메라
    public Canvas tutorialCanvas; // 튜토리얼씬 캔버스
    public Camera storyCam;       // 스토리씬 카메라
    public Canvas storyCanvas;    // 스토리씬 캔버스
    public Camera battleCam;      // 전투씬 카메라
    public Canvas battleCanvas;   // 전투씬 캔버스
    private Vector3 originalPos;  // 카메라 원래 위치
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    public void test()
    {
        Shake();
    }

    //camNum = 0(스토리씬 카메라를 기본값으로 설정) 스토리씬: 0 / 전투씬: 1 / 튜토리얼: 2
    public void Shake(float duration = 0.2f, float magnitude = 5f, int camNum = 0)
    {
        Camera currentCam;
        Canvas currentCanvas;
        switch(camNum)
        {
            case 0: // 스토리씬 카메라
                currentCam = storyCam;
                currentCanvas = storyCanvas;
                break;
            case 1: // 배틀씬 카메라
                currentCam = battleCam;
                currentCanvas = battleCanvas;
                break;
            case 2: // 튜토리얼씬 카메라
                currentCam = tutorialCam;
                currentCanvas = tutorialCanvas;
                break;
            default: // 기본값 = 스토리씬 카메라
                currentCam = storyCam;
                currentCanvas = battleCanvas;
                break;
        }

        currentCanvas.renderMode = RenderMode.WorldSpace;    // 캔버스 렌더링 모드를 월드로 변경(기본값은 스크린)
        originalPos = currentCam.transform.localPosition;    // 초기 위치를 저장
        StartCoroutine(CoroutineShake(duration, magnitude, currentCam, currentCanvas));
    }

    public IEnumerator CoroutineShake(float duration, float magnitude,Camera cam, Canvas canvas)
    {
        while(duration > 0)
        {
            cam.transform.localPosition = originalPos + Random.insideUnitSphere * magnitude; // 무작위로 흔들기
            duration -= Time.deltaTime;
            yield return null;
        }
        cam.transform.localPosition = originalPos;        // 원래 위치로 복구
        canvas.renderMode = RenderMode.ScreenSpaceCamera; // 캔버스 렌더링 모드를 기본값(스크린)으로 변경
    }

}