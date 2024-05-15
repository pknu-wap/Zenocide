using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventManager : MonoBehaviour, IPointerDownHandler
{
   public void OnPointerDown(PointerEventData eventData)
   {
       Debug.Log("Button Clicked");
   }
}
