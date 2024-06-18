using DG.Tweening;
using System.Collections;
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
    }

    private void Start()
    {
        gameObject.SetActive(false);
        MakeMoveSequence();
    }

    private void MakeMoveSequence()
    {
        moveSequence = DOTween.Sequence()
            .Append(transform.DOMove(startPosition, 0f))
            .Append(transform.DOMove(startPosition + endOffset, duration))
            .OnComplete(() => {
                gameObject.SetActive(false);
            })
            .SetAutoKill(false);
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
