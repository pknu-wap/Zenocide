using UnityEngine.EventSystems;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEditor.Rendering;
using static UnityEditor.Progress;
using Unity.VisualScripting.ReorderableList;

public class Choice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool canSelect = true;
    public bool marked = false;
    public bool selected = false;

    [Header("선택 표시 오브젝트")]
    public GameObject[] sign = new GameObject[2];
    private Button choiceButton;
    private TMP_Text choiceText;
    private TMP_Text requireItemText;
    private GameObject shadow;

    void Start()
    {
        EnrollComponent();
        HideSelectUI();
        DisableChoiceObject();
    }

    private void EnrollComponent()
    {
        sign[0] = transform.GetChild(2).gameObject;
        sign[1] = transform.GetChild(3).gameObject;

        choiceButton = GetComponent<Button>();
        choiceText = transform.GetChild(0).GetComponent<TMP_Text>();
        requireItemText = transform.GetChild(1).GetComponent<TMP_Text>();
        shadow = transform.GetChild(4).gameObject;
}

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (canSelect == false)
        {
            return;
        }

        marked = true;
        ShowSelectUI();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        marked = false;
        HideSelectUI();
    }

    public void UpdateText(string choiceText, string[] requireItemText)
    {
        // 선택지 문구 변경
        this.choiceText.text = choiceText;
        this.requireItemText.text = "";

        // 우선 활성화
        EnableInteractable();

        // 결과 저장
        string result_str = "";

        // 필요한 목록을 받아온다. (태그 : 개수)
        Dictionary<string, int> requireItems = Items.Instance.ItemStringToDictionary(requireItemText);

        // 검색 결과 딕셔너리를 받아온다.
        Dictionary<string, int> resultItems = Items.Instance.FindItemsWithTag(requireItems);

        // 첫번째 문자열인지 검사하는 변수
        bool isFirst = true;

        foreach(var item in resultItems)
        {
            // 첫번째만 제외하고
            if (isFirst == false)
            {
                // 콤마를 찍는다.
                result_str += ", ";
            }

            // 아이템 문자열을 추가한다.
            // 아이템을 찾지 못 했다면
            if(item.Value <= 0)
            {
                // 요구하는 태그와 갯수를 넣고
                string tag = item.Key;
                result_str += GetFormattedItemText(tag, requireItems[tag], false);

                // 선택지는 비활성화
                DisableInteractable();
            }
            // 찾았다면
            else
            {
                // 찾은 아이템의 개수를 넣는다.
                result_str += GetFormattedItemText(item.Key, item.Value, true);
            }

            // 한 번 쓴 후 false로
            isFirst = false;
        }

        // 만들어진 문자열이 있다면 추가한다.
        if(result_str is not "")
        {
            this.requireItemText.text = "필요한 아이템: " + result_str;
        }
    }
    
    // 아이템 종류 및 개수에 따라 문자열을 생성한다.
    private string GetFormattedItemText(string tag, int count, bool isEnough)
    {
        string result_str = "";

        // 먼저, 아이템 이름과 색깔을 지정해주자.
        // 충분한 아이템이라면
        if (isEnough == true)
        {
            // 초록색으로 작성
            result_str += $"<color=green>{tag}";
        }
        // 모자라다면
        else
        {
            // 빨간색으로 작성
            result_str += $"<color=red>{tag}";

        }

        // 이제 타입에 맞춰 개수를 적어준다.
        // 개수가 1개 이상이라면
        if (count > 1)
        {
            // 그리고 아이템이 물품이라면
            if (ItemInfo.Instance.GetItem(tag).Type == ItemType.Item)
            {
                // x를
                result_str += " x ";
            }
            // 능력이라면
            else
            {
                // Lv를 붙인다.
                result_str += " Lv. ";
            }

            // 요구하는 개수를 적어준다.
            result_str += count;
        }

        // 색 태그를 닫는다.
        result_str += "</color>";

        // 반환
        return result_str;
    }

    // 버튼 이벤트 등록용
    public void SelectChoice(int i)
    {
        SelectManager.Instance.result = i;
    }

    public void EnableChoiceObject()
    {
        gameObject.SetActive(true);
    }

    public void DisableChoiceObject()
    {
        gameObject.SetActive(false);
    }

    public void EnableInteractable()
    {
        choiceButton.interactable = true;
        shadow.SetActive(false);
        HideSelectUI();
        canSelect = true;
    }

    public void DisableInteractable()
    {
        choiceButton.interactable = false;
        shadow.SetActive(true);
        canSelect = false;
    }

    void ShowSelectUI()
    {
        sign[0].SetActive(true);
        sign[1].SetActive(true);
    }
    void HideSelectUI()
    {
        sign[0].SetActive(false);
        sign[1].SetActive(false);
    }
}
