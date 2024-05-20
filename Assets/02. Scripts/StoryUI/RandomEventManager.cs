using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventManager : MonoBehaviour
{
    private Stack<Event> eventStack = new Stack<Event>();
    private List<Event> eventList = new List<Event>();

    void Start()
    {
        // 예시 이벤트 추가
        AddEvent(new Event("SpawnEnemy", "An enemy has spawned."));
        AddEvent(new Event("GrantItem", "You have received an item."));
        AddEvent(new Event("ChangeWeather", "The weather has changed."));

        // 이벤트를 랜덤으로 뽑기
        Event randomEvent = DrawRandomEvent();
        if (randomEvent != null)
        {
            // 랜덤 이벤트 발생
        }
    }

    // 이벤트 추가
    public void AddEvent(Event newEvent)
    {
        eventStack.Push(newEvent);
        eventList.Add(newEvent);
    }

    // 랜덤 이벤트 뽑기
    public Event DrawRandomEvent()
    {
        if (eventStack.Count == 0) return null;

        int randomIndex = Random.Range(0, eventList.Count);
        Event randomEvent = eventList[randomIndex];

        // 스택과 리스트에서 해당 이벤트 제거
        Stack<Event> tempStack = new Stack<Event>();
        while (eventStack.Count > 0)
        {
            Event topEvent = eventStack.Pop();
            if (topEvent == randomEvent)
            {
                eventList.Remove(topEvent);
                break;
            }
            else
            {
                tempStack.Push(topEvent);
            }
        }

        // 남아있는 이벤트를 다시 스택에 추가
        while (tempStack.Count > 0)
        {
            eventStack.Push(tempStack.Pop());
        }

        return randomEvent;
    }
}
