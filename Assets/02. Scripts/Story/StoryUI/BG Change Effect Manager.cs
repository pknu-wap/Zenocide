using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BGChangeEffectManager : MonoBehaviour
{
    public static BGChangeEffectManager Instance { get; set; }

    public bool isFading = false; // Fade 중인지 확인하는 변수

    [Header("레이어")]
    private GameObject[] Layers = new GameObject[24]; // 레이어 배열
    private CanvasGroup[] LayerCanvasGroups = new CanvasGroup[24]; // CanvasGroup 배열

    [Header("레이어 층의 오브젝트 개수")]
    private int LayerCount = 6;

    [Header("부모 오브젝트")]
    private GameObject FirstEffectObject;
    private GameObject SecondEffectObject;

    [Header("이전 배경화면 저장 변수")]
    private string PreviousBGName;

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

    public IEnumerator FadeOut(float time)
    {
        if(isFading) yield break;

        float fadeDelay = 0.2f;
        float delay = (time - fadeDelay) / (LayerCount * 4);

        isFading = true;
        LayerActive(true);

        for (int i = 0; i < LayerCount * 4; i++)
        {
            Layers[i].transform.localScale = Vector3.zero;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(Layers[i].transform.DOScale(Vector3.one, fadeDelay));
            sequence.Join(LayerCanvasGroups[i].DOFade(1f, fadeDelay));
            yield return new WaitForSeconds(delay);
            // 전체 시간 = 마름모가 커지는 시간 + 마름모의 개수 * 딜레이
            // time = fadeDelay + 24 * delay;
        }
        yield return new WaitForSeconds(time);
    }
    
    public IEnumerator FadeIn(float time)
    {
        float fadeDelay = 0.2f;
        float delay = (time - fadeDelay) / (LayerCount * 4);
        
        for (int i = 0; i < LayerCount * 4; i++)
        {
           Sequence sequence = DOTween.Sequence();
            sequence.Append(Layers[i].transform.DOScale(Vector3.zero, fadeDelay));
            sequence.Join(LayerCanvasGroups[i].DOFade(0f, fadeDelay));
            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(0.5f);
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
