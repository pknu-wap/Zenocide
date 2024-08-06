using UnityEngine;
using System.Collections;
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

    public void FadeOutEffect()
    {
        StartCoroutine(FadeOut(0.5f));
    }

    public void FadeInEffect()
    {
        StartCoroutine(FadeIn(0.5f));
    }

    public IEnumerator FadeOut(float time)
    {
        if(isFading) yield break;

        float delay = time / (layerCount * 4);

        isFading = true;
        LayerActive(true);

        for (int i = 0; i < layerCount * 4; i++)
        {
            layers[i].transform.localScale = Vector3.zero;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(layers[i].gameObject.transform.DOScale(Vector3.one, delay));
            sequence.Join(layers[i].DOFade(1, delay));
            yield return sequence.WaitForCompletion();
        }
    }
    
    public IEnumerator FadeIn(float time)
    {
        float delay = time / (layerCount * 4);
        
        for (int i = 0; i < layerCount * 4; i++)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(layers[i].gameObject.transform.DOScale(Vector3.zero, delay));
            sequence.Join(layers[i].DOFade(0, delay));
            yield return sequence.WaitForCompletion();
        }
        LayerActive(false);
        isFading = false;
    }

    public void LayerActive(bool isActive)
    {
        for (int i = 0; i < layerCount * 4; i++)
        {
            layers[i].gameObject.SetActive(isActive);
        }
    }
}
