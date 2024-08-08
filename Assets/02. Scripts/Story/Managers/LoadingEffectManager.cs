using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using System.Linq;

public class LoadingEffectManager : MonoBehaviour
{
    public static LoadingEffectManager Instance { get; set; }

    public bool isFading = false; // Fade 중인지 확인하는 변수

    [Header("레이어")]
    private CanvasGroup[] layers; // 레이어 배열
    
    [Header("레이어 층의 오브젝트 개수")]
    private int layerCount;

    [Header("총 전환 시간")]
    public float fadeTime = 2f;

    [Header("Fade 시간")]
    public float fadeDuration = 0.2f;

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
        // 로딩 씬 불러옴
        GameObject loadingScene = GameObject.FindWithTag("Loading Scene"); 
        // 레이어 층의 오브젝트 개수 초기화
        layerCount = loadingScene.transform.GetChild(0).GetChild(0).childCount; 
        // 레이어 배열 초기화
        CanvasGroup[] tempLayers = loadingScene.GetComponentsInChildren<CanvasGroup>(); 

        // 가장 오른쪽에 있는 오브젝트들(6번째 오브젝트)부터 시작하도록 정렬
        // 이때 오브젝트는 가장 위층 레이어부터 아래로 내려오며 저장됨
        // 정렬된 레이어를 배열에 할당
        layers = tempLayers
                 .GroupBy(canvas => canvas.name)
                 .OrderByDescending(c => c.Key)
                 .SelectMany(c => c)
                 .ToArray();

        //모든 레이어 비활성화
        LayerActive(false);
    }
    
    public async Task FadeEffect(float time)
    {
        StartCoroutine(FadeOut(time));
        await Task.Delay((int)(time * 1000));
        StartCoroutine(FadeIn(time));
        await Task.Delay((int)(time * 1000));
    }

    // 테스트 함수 - 1
    public void FadeOutEffect()
    {
        StartCoroutine(FadeOut(fadeTime));
    }

    // 테스트 함수 - 2
    public void FadeInEffect()
    {
        StartCoroutine(FadeIn(fadeTime));
    }

    public IEnumerator FadeOut(float time)
    {
        // FadeIn 또는 FadeOut 중인 경우 함수 종료
        if(isFading) yield break;
        // time / 총 레이어 수 만큼 딜레이를 설정
        // 각 레이어가 순차적으로 FadeOut 되며 time(소수점이라 근사치) 동안 진행
        float delay = time / (layerCount * 4);

        isFading = true;
        LayerActive(true);

        for (int i = 0; i < layerCount * 4; i++)
        {
            layers[i].transform.localScale = Vector3.zero;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(layers[i].gameObject.transform.DOScale(Vector3.one, fadeDuration).SetEase(Ease.Linear));
            sequence.Join(layers[i].DOFade(1, fadeDuration).SetEase(Ease.Linear));
            yield return new WaitForSeconds(delay);
        }
        // 마지막 레이어가 FadeOut 되는 시간만큼 대기
        yield return new WaitForSeconds(fadeDuration);
        isFading = false;
    }
    
    public IEnumerator FadeIn(float time)
    {
        // FadeIn 또는 FadeOut 중인 경우 함수 종료
        if(isFading) yield break;
        // time / 총 레이어 수 만큼 딜레이를 설정
        // 각 레이어가 순차적으로 FadeOut 되며 time(소수점이라 근사치) 동안 진행
        float delay = time / (layerCount * 4);

        isFading = true;

        for (int i = 0; i < layerCount * 4; i++)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(layers[i].gameObject.transform.DOScale(Vector3.zero, fadeDuration).SetEase(Ease.Linear));
            sequence.Join(layers[i].DOFade(0, fadeDuration).SetEase(Ease.Linear));
            yield return new WaitForSeconds(delay);
        }
        // 마지막 레이어가 FadeIn 되는 시간만큼 대기
        yield return new WaitForSeconds(fadeDuration);
        //모든 레이어 비활성화
        LayerActive(false);
        isFading = false;
    }

    // 로딩 씬 레이어 활성화 함수
    public void LayerActive(bool isActive)
    {
        for (int i = 0; i < layerCount * 4; i++)
        {
            layers[i].gameObject.SetActive(isActive);
        }
    }
}
