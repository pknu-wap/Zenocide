using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class Choice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public bool marked = false;
    public bool selected = false;
    [Header("선택 표시 오브젝트")]
    public GameObject[] Sign = new GameObject[2];

    void Start()
    {
        Sign[0].SetActive(false);
        Sign[1].SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        selected = marked ? true : false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        marked = true;       
        Sign[0].SetActive(true);
        Sign[1].SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        marked = false;
        Sign[0].SetActive(false);
        Sign[1].SetActive(false);
    }
}
