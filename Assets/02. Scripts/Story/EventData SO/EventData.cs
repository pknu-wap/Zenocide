using UnityEngine;

public enum EventType
{
    Main,
    Sub,
    Relation
};

[CreateAssetMenu(fileName = "EventData", menuName = "Scriptable Object/EventData", order = 1)]
public class EventData: ScriptableObject
{
    [Header("Event 종류 식별 ID")]
    public EventType eventID;

    [Header("Text 데이터 인덱스")]
    public int startIndex;
    public int endIndex;

    [Header("선택지 이벤트")]
    public EventData[] relationEvent;

    [Header("직후에 실행할 이벤트")]
    public EventData nextEvent;

    [Header("리스트에 추가할 이벤트")]
    public EventData[] addEvent;
}
