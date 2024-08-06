using UnityEngine;
using System.Collections;
using DG.Tweening;

public class LoadingEffectManager : MonoBehaviour
{
    public static LoadingEffectManager Instance { get; set; }

    public bool isFading = false; // Fade 중인지 확인하는 변수

    [Header("레이어")]
    public GameObject[] Layers = new GameObject[24]; // 레이어 배열
    public CanvasGroup[] LayerCanvasGroups = new CanvasGroup[24]; // CanvasGroup 배열

    [Header("레이어 층의 오브젝트 개수")]
    private int LayerCount = 6;

    [Header("부모 오브젝트")]
    public GameObject FirstEffectObject;
    public GameObject SecondEffectObject;

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
        FirstEffectObject = GameObject.FindWithTag("BG Effect Layer1");  // 첫번째 레이어
        SecondEffectObject = GameObject.FindWithTag("BG Effect Layer2"); // 두번째 레이어
        GameObject FirstUpLayer = FirstEffectObject.transform.GetChild(0).gameObject;      // 첫번째 레이어의 첫번째 줄
        GameObject FirstDownLayer = FirstEffectObject.transform.GetChild(1).gameObject;   // 첫번째 레이어의 두번째 줄
        GameObject SecondUpLayer = SecondEffectObject.transform.GetChild(0).gameObject;    // 두번째 레이어의 첫번째 줄
        GameObject SecondDownLayer = SecondEffectObject.transform.GetChild(1).gameObject;  // 두번째 레이어의 두번째 줄

        for(int i =  0; i < LayerCount; i++) // 6번째 오브젝트부터 첫번째 배열에 저장
        {
            Layers[i * 4    ] = FirstUpLayer.transform.GetChild(5 - i).gameObject;     // Column을 기준으로 했을 때 Top 오브젝트
            Layers[i * 4 + 1] = FirstDownLayer.transform.GetChild(5 - i).gameObject;    // Column을 기준으로 했을 때 Middle Up 오브젝트
            Layers[i * 4 + 2] = SecondUpLayer.transform.GetChild(5 - i).gameObject;   // Column을 기준으로 했을 때 Middle Down 오브젝트
            Layers[i * 4 + 3] = SecondDownLayer.transform.GetChild(5 - i).gameObject;  // Column을 기준으로 했을 때 Bottom 오브젝트

            for (int j = 0; j < 4; j++)
            {
                CanvasGroup layerCanvas = Layers[i * 4 + j].GetComponent<CanvasGroup>();
                LayerCanvasGroups[i * 4 + j] = layerCanvas;
                layerCanvas.alpha = 0;
                Layers[i * 4 + j].SetActive(false);
            }
        }
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

        float delay = time / (LayerCount * 4);

        isFading = true;
        LayerActive(true);

        for (int i = 0; i < LayerCount * 4; i++)
        {
            Layers[i].transform.localScale = Vector3.zero;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(Layers[i].transform.DOScale(Vector3.one, delay));
            sequence.Join(LayerCanvasGroups[i].DOFade(1f, delay));
            yield return sequence.WaitForCompletion();
        }
    }
    
    public IEnumerator FadeIn(float time)
    {
        float delay = time / (LayerCount * 4);
        
        for (int i = 0; i < LayerCount * 4; i++)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(Layers[i].transform.DOScale(Vector3.zero, delay));
            sequence.Join(LayerCanvasGroups[i].DOFade(0f, delay));
            yield return sequence.WaitForCompletion();
        }
        LayerActive(false);
        isFading = false;
    }

    public void LayerActive(bool isActive)
    {
        for (int i = 0; i < LayerCount * 4; i++)
        {
            Layers[i].SetActive(isActive);
        }
    }
}
