using UnityEngine;

[CreateAssetMenu(fileName = "EventData", menuName = "Scriptable Objects/EventData", order = 1)]
public class EventData: ScriptableObject
{
    [Header("Event Index")]
    public int eventID;

    [Header("Text 데이터")]
    public int startIndex;
    public int endIndex;

    [Header("후속 이벤트")]
    public string nextEvent;
}
