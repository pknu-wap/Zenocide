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
    
    private RectTransform storyCanvasRect;      // 스토리씬 캔버스 사이즈 정보
    private RectTransform battleCanvasRect;     // 배틀씬 캔버스 사이즈 정보
    private RectTransform tutorialCanvasRect;   // 튜토리얼씬 캔버스 사이즈 정보

    public enum Scene
    {
        Story,
        Battle,
        Tutorial
    };

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
        storyCanvasRect = storyCanvas.GetComponent<RectTransform>();       // 스토리씬 캔버스 정보 불러옴
        battleCanvasRect = battleCanvas.GetComponent<RectTransform>();     // 배틀씬 캔버스 정보 불러옴
        tutorialCanvasRect = tutorialCanvas.GetComponent<RectTransform>(); // 튜토리얼씬 캔버스 정보 불러옴
    }

    public void test()
    {
        Shake(0.2f,5f,Scene.Story);
    }

    //currentScene = scene.story(스토리씬 카메라를 기본값으로 설정) 스토리씬: scene.story / 전투씬: scene.battle / 튜토리얼: scene.tutorial
    public void Shake(float duration = 0.2f, float magnitude = 5f, Scene currentScene = Scene.Story)
    {
        Camera currentCam;
        Canvas currentCanvas;
        RectTransform currentCanvasRect;
        RectTransform originalCanvasRect;
        switch(currentScene)
        {
            case Scene.Story: // 스토리씬 카메라
                currentCam = storyCam;
                currentCanvas = storyCanvas;
                currentCanvasRect = storyCanvasRect;
                break;
            case Scene.Battle: // 배틀씬 카메라
                currentCam = battleCam;
                currentCanvas = battleCanvas;
                currentCanvasRect = battleCanvasRect;
                break;
            case Scene.Tutorial: // 튜토리얼씬 카메라
                currentCam = tutorialCam;
                currentCanvas = tutorialCanvas;
                currentCanvasRect = tutorialCanvasRect;
                break;
            default: // 기본값 = 스토리씬 카메라
                currentCam = storyCam;
                currentCanvas = storyCanvas;
                currentCanvasRect = storyCanvasRect;
                break;
        }

        currentCanvas.renderMode = RenderMode.WorldSpace;           // 캔버스 렌더링 모드를 월드로 변경(기본값은 스크린)
        originalPos = currentCam.transform.localPosition;           // 기본 위치 저장
        originalCanvasRect = currentCanvasRect;                     // 기본 캔버스 정보 저장
        float currentCanvasHeight = currentCanvasRect.rect.height;  // 캔버스의 높이 정보 불러옴
        float currentCanvasWidth = currentCanvasRect.rect.width;    // 캔버스의 너비 정보 불러옴
        currentCanvasRect.sizeDelta = new Vector2(currentCanvasWidth + 200, currentCanvasHeight + 200); // 높이와 너비에 각각 200씩 추가하여 여백이 보이지 않도록 확장
        StartCoroutine(CoroutineShake(duration, magnitude, currentCam, currentCanvas)); // 화면 흔들림 효과를 주는 코루틴 함수 실행
        currentCanvasRect = originalCanvasRect; // 흔들기 함수 종료 후 캔버스 사이즈 복구
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