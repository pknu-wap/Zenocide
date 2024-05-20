using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour
{
    public string eventName;
    public string description;

    public Event(string name, string desc)
    {
        eventName = name;
        description = desc;
    }
}
