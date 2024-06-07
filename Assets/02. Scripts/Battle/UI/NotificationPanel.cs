using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class NotificationPanel : MonoBehaviour
{
    [SerializeField] TMP_Text notificationTMP;

    public void Show(string message)
    {
        notificationTMP.text = message;
        Sequence sequence = DOTween.Sequence()
            .Append(transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutQuad))
            .AppendInterval(0.9f)
            .Append(transform.DOScale(new Vector3(1, 0, 1), 0.3f).SetEase(Ease.InOutQuad));
    }

    // 코루틴으로 사용하는 Show. bool은 쓰레기값이다. (다형성을 위함. 추후 수정 예정)
    public IEnumerator Show(string message, bool isCoroutine)
    {
        notificationTMP.text = message;
        Sequence sequence = DOTween.Sequence()
            .Append(transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutQuad))
            .AppendInterval(0.9f)
            .Append(transform.DOScale(new Vector3(1, 0, 1), 0.3f).SetEase(Ease.InOutQuad));

        yield return sequence.WaitForCompletion();
    }

    void Start() => ScaleZero();

    [ContextMenu("ScaleOne")]
    void SclaeOne() => transform.localScale = Vector3.one;

    [ContextMenu("ScaleZero")]
    public void ScaleZero() => transform.localScale = new Vector3(1, 0, 1);
}
