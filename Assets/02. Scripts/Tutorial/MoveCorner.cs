using DG.Tweening;
using UnityEngine;

public class MoveCorner : MonoBehaviour
{
    public Vector3 firstPosition;

    void Start()
    {
        firstPosition = transform.position;
        Move();
    }

    public void Move()
    {
        transform.position = firstPosition;

        DOTween.Sequence()
            .Append(transform.DOMoveX(firstPosition.x - 300f, 2f))
            .Join(transform.DOMoveY(firstPosition.y + 150f, 2f))
            .Append(transform.DOMoveX(firstPosition.x - 600f, 2f))
            .Join(transform.DOMoveY(firstPosition.y, 2f)).SetEase(Ease.OutCubic);
    }
}
