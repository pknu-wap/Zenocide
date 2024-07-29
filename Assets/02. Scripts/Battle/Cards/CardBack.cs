using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBack : Poolable
{
    public IEnumerator Move(Transform cardResetPoint, Transform cardSpawnPoint, Transform cardDumpPoint)
    {
        transform.position = cardDumpPoint.position;
        float resetMoveDelay = CardManager.Instance.resetMoveDelay;

        Sequence sequence = DOTween.Sequence()
                .Append(transform.DOMoveX(cardResetPoint.position.x, resetMoveDelay).SetEase(Ease.Linear))
                .Join(transform.DOMoveY(cardResetPoint.position.y, resetMoveDelay).SetEase(Ease.OutCubic))
                .Append(transform.DOMoveX(cardSpawnPoint.position.x, resetMoveDelay).SetEase(Ease.Linear))
                .Join(transform.DOMoveY(cardSpawnPoint.position.y, resetMoveDelay).SetEase(Ease.InCubic));

        yield return new WaitForSeconds(CardManager.Instance.resetDelay);
    }

    public void ResetPosition(Transform cardDumpPoint)
    {
        transform.position = cardDumpPoint.position;
    }
}
