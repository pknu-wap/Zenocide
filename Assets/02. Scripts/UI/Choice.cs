using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using Unity.VisualScripting;

public class Choice : MonoBehaviour, IPointerDownHandler
{
    public List<string> BackPackItems = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        BackPackItems = Items.Inst.items;
        Debug.Log(BackPackItems);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Button Clicked");
        GameObject SelectedObj = EventSystem.current.currentSelectedGameObject;
    }
}
