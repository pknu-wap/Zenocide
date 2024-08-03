using UnityEngine.EventSystems;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

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
        this.choiceText.text = choiceText;
        this.requireItemText.text = "";

        // 우선 활성화
        EnableInteractable();

        // 있는 경우
        if (requireItemText.Length > 0 && requireItemText[0] != "")
        {
            this.requireItemText.text += "필요한 아이템: ";

            // 모든 아이템에 대해
            for (int i = 0; i < requireItemText.Length; ++i)
            {
                // 아이템 검색
                Item item = Items.Instance.FindItemWithTag(requireItemText[i]);

                // 아이템이 있다면
                if (item.Name != "Error")
                {
                    // 아이템 이름을 초록색으로 적는다.
                    this.requireItemText.text += "<color=green>" + item.Name + "</color>";
                }

                // 아이템이 없다면
                else
                {
                    // 태그를 빨갛게 적고
                    this.requireItemText.text += "<color=red>" + requireItemText[i] + "</color>";
                    // 비활성화 한다.
                    DisableInteractable();
                }

                // 콤마 넣기
                if (i < requireItemText.Length - 1)
                {
                    this.requireItemText.text += ", ";
                }
            }
        }
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
