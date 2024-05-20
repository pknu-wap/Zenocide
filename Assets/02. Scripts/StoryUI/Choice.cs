using UnityEngine.EventSystems;
using UnityEngine;

public class Choice : MonoBehaviour, IPointerDownHandler
{
    public Choice choice;
    // 선택지가 선택되었는지 여부 저장 변수
    public static int Selected = 0;


    public void OnPointerDown(PointerEventData eventData)
    {
        Selected = 0;
        // 동일한 선택지를 한번 더 선택 시
        if(SelectManager.instance.currentChoice == choice){
            if(choice.name == "ChoiceBoxUp")
                Selected = 1;
            else
                Selected = 2;
        }
        // 플레이어가 고른 선택지 저장
        SelectManager.instance.currentChoice = choice;
    }
}
