using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using Unity.VisualScripting;

public class Choice : MonoBehaviour, IPointerDownHandler
{
    public List<string> BackPackItems = new List<string>();

    public bool isClicked = false;
    public GameObject Obj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isClicked = true;
        Obj.SetActive(true);
    }
}
