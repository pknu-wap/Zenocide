using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEditor.EditorTools;

public class Choice : MonoBehaviour, IPointerDownHandler
{
    public Choice choice;

    public static int Selected = 0;


    public void OnPointerDown(PointerEventData eventData)
    {
        Selected = 0;
        if(SelectManager.instance.currentChoice == choice){
            if(choice.name == "ChoiceBoxUp")
                Selected = 1;
            else
                Selected = 2;
        }
        
        SelectManager.instance.currentChoice = choice;
    }
}
