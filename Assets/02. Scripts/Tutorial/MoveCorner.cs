using DG.Tweening;
using UnityEngine;

public class MoveCorner : MonoBehaviour
{
    public Vector3 firstPosition;

    void Start()
    {
        firstPosition = transform.localPosition;
        Move();
    }

    public void Move()
    {
        transform.localPosition = firstPosition;

        DOTween.Sequence()
            .Append(transform.DOLocalMoveX(0f, 2f))
            .Join(transform.DOLocalMoveY(-350f, 2f))
            .Append(transform.DOLocalMoveX(-600f, 2f))
            .Join(transform.DOLocalMoveY(-450f, 2f)).SetEase(Ease.OutCubic);
    }
}
