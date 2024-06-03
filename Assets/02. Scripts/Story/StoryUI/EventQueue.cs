using System.Collections.Generic;

public class EventQueue
{

public Queue<EventData> eventQueue = new Queue<EventData>();

    public void Enqueue(EventData eD)
    {
        eventQueue.Enqueue(eD);
    }

    public EventData Dequeue()
    {
        return eventQueue.Dequeue();
    }

    public EventData Peek()
    {
        return eventQueue.Peek();
    }

    public int Count
    {
        get {return eventQueue.Count;}
    }
}