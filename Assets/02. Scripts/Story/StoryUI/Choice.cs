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

    public void UpdateText(string choiceText, string requireItemText = "")
    {
        this.choiceText.text = choiceText;
        this.requireItemText.text = "";

        if (requireItemText != "")
        {
            if (Items.Instance.items.ContainsKey(requireItemText))
            {
                this.requireItemText.text = "필요한 아이템: <color=green>" + requireItemText + "</color>";

            }

            else
            {
                this.requireItemText.text = "필요한 아이템: <color=red>" + requireItemText + "</color>";
                DisableInteractable();
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
    }

    public void DisableInteractable()
    {
        choiceButton.interactable = false;
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
