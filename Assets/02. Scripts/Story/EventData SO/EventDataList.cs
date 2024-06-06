using UnityEngine;

[CreateAssetMenu(fileName = "EventDataList", menuName = "Scriptable Object/Event Data List", order = 11)]
public class EventDataList : ScriptableObject
{
    public EventData[] list;
}
