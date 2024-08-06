using DG.Tweening;
using System.Collections.Generic;
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
        notificationText.text = CompressWithCount(itemName, true) + "을(를) 획득했다.";

        MoveMessage();
    }

    public void ShowRemoveItemMessage(string itemName)
    {
        notificationText.text = CompressWithCount(itemName, true) + "을(를) 잃었다.";

        MoveMessage();
    }

    public void ShowGetCardMessage(string cardName)
    {
        notificationText.text = CompressWithCount(cardName, false) + " 카드를 획득했다.";

        MoveMessage();
    }

    public void ShowRemoveCardMessage(string cardName)
    {
        notificationText.text = CompressWithCount(cardName, false) + " 카드를 잃었다.";

        MoveMessage();
    }

    /// <summary>
    /// #으로 연결된 문자열을 받아 개수와 함께 압축한다.
    /// </summary>
    /// <param name="str">#으로 연결된 문자열</param>
    /// <returns>이름, 개수가 정리된 문자열</returns>
    private string CompressWithCount(string str, bool isItem = true)
    {
        // 먼저 문자열을 분리해 Dictionary로 정리한다.
        Dictionary<string, int> items = Items.Instance.ItemStringToDictionary(str.Split('#'));
        // 결과를 저장할 문자열
        string result_str = "";

        // 첫번째 문자열인지 검사하는 변수
        bool isFirst = true;

        foreach (var item in items)
        {
            // 첫번째만 제외하고
            if (isFirst == false)
            {
                // 콤마를 찍는다.
                result_str += ", ";
            }

            // 이름을 적어주고
            result_str += item.Key;

            // 아이템이 1개 이하라면
            if(item.Value <= 1)
            {
                // 수량을 적지 않고 끝낸다.
                continue;
            }

            // 아이템이자 능력이라면
            if (isItem == true && ItemInfo.Instance.GetItem(tag).type == ItemType.Status)
            {
                // Lv를 붙인다.
                result_str += " Lv. ";
            }
            // 카드거나 물품이라면
            else
            {
                // x를 붙인다.
                result_str += " x ";
            }

            // 요구하는 개수를 적어준다.
            result_str += item.Value;

            // 한 번 쓴 후 false로
            isFirst = false;
        }

        // 만들어진 문자열을 반환한다.
        return result_str;
    }

    private void MoveMessage()
    {
        gameObject.SetActive(true);
        moveSequence.Restart(); 
    }
}
