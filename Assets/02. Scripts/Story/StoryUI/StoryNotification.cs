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
        notificationText.text = CompressWithCount(itemName) + "을(를) 획득했다.";

        MoveMessage();
    }

    public void ShowRemoveItemMessage(string itemName)
    {
        notificationText.text = CompressWithCount(itemName) + "을(를) 잃었다.";

        MoveMessage();
    }

    public void ShowGetCardMessage(string cardName)
    {
        notificationText.text = CompressWithCount(cardName) + " 카드를 획득했다.";

        MoveMessage();
    }

    public void ShowRemoveCardMessage(string cardName)
    {
        notificationText.text = CompressWithCount(cardName) + " 카드를 잃었다.";

        MoveMessage();
    }

    /// <summary>
    /// #으로 연결된 문자열을 받아 개수와 함께 압축한다.
    /// </summary>
    /// <param name="str">#으로 연결된 문자열</param>
    /// <returns></returns>
    private string CompressWithCount(string str)
    {
        string[] strs = str.Split('#');
        string result_str = "";
        int count = 1;

        // 스크립트 작성
        for (int i = 0; i < strs.Length; ++i)
        {
            // 앞전 문자열과 같다면
            if (i > 0 && strs[i] == strs[i - 1])
            {
                // 카운트를 1 증가
                ++count;
                continue;
            }
            // 앞전 문자열과 다를 때
            else
            {
                // count가 1이 아니라면
                if (count > 1)
                {
                    // 개수 적어주기
                    result_str += (" x " + count);
                    count = 1;
                }

                // 첫 문자열이 아니라면
                if (i > 0)
                {
                    // 콤마 찍어주기
                    result_str += ", ";
                }

                // 문자열 적어주기
                result_str += strs[i];
            }
        }

        // 만약, 반복문을 전부 돌았는데 남은 count가 있다면
        if (count > 1)
        {
            // 개수를 마저 작성한다. (덜 적힌 경우이므로)
            result_str += (" x " + count);
        }

        // 만든 결과물을 반환
        return result_str;
    }

    private void MoveMessage()
    {
        gameObject.SetActive(true);
        moveSequence.Restart(); 
    }
}
