using UnityEngine;

[CreateAssetMenu(fileName = "EventData", menuName = "Scriptable Object/EventData", order = 1)]
public class EventData: ScriptableObject
{
    [Header("Event 종류 식별 ID")]
    public int eventID;

    [Header("Event 이름")]
    public string eventName;

    [Header("Text 데이터")]
    public int startIndex;
    public int endIndex;

    [Header("후속 이벤트")]
    public EventData[] nextEvent;
}
