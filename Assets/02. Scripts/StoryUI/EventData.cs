using UnityEngine;

[CreateAssetMenu(fileName = "EventData", menuName = "Scriptable Objects/EventData", order = 1)]
public class EventData: ScriptableObject
{
    [Header("Text 데이터")]
    public int storyText;
    public int storyName;

    [Header("후속 이벤트")]
    public TextData nextEvent;
}
