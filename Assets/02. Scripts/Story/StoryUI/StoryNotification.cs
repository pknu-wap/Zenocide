using DG.Tweening;
using TMPro;
using UnityEngine;

public class StoryNotification : MonoBehaviour
{
    private TMP_Text notificationText;
    private Sequence moveSequence;
    public Vector2 startPosition;
    public Vector2 endOffset = new Vector2(0, 30f);
    public float duration = 1f;

    private void Awake()
    {

        notificationText = transform.GetChild(0).GetComponent<TMP_Text>();
        startPosition = transform.position;

        gameObject.SetActive(false);

        MakeMoveSequence();
        ShowGetCardMessage("글러트니");
    }

    private void MakeMoveSequence()
    {
        moveSequence = DOTween.Sequence()
            .Append(transform.DOMove(startPosition, 0f))
            .Append(transform.DOMove(startPosition + endOffset, duration))
            .SetAutoKill(false)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }

    public void ShowGetItemMessage(string itemName)
    {
        notificationText.text = itemName + "을 획득했다.";
        MoveMessage();
    }

    public void ShowRemoveItemMessage(string itemName)
    {
        notificationText.text = itemName + "을 잃었다.";
        MoveMessage();
    }

    public void ShowGetCardMessage(string cardName)
    {
        notificationText.text = "'" + cardName + "' 카드를 획득했다.";
        MoveMessage();
    }

    public void ShowRemoveCardMessage(string cardName)
    {
        notificationText.text = "'" + cardName + "' 카드를 잃었다.";
        MoveMessage();
    }

    private void MoveMessage()
    {
        gameObject.SetActive(true);
        moveSequence.Restart();
    }
}
