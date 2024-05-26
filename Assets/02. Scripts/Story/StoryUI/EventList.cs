using UnityEngine;

public class EventList : MonoBehaviour
{
    int subEventCount = 7;

    [Header("전체 이벤트 데이터 큐")]
    public static EventQueue TotalEventQueue = new EventQueue();

    [Header("메인 이벤트 데이터 큐")]
    EventQueue MainEventQueue = new EventQueue();


    void Start()
    {
        EventData[] MainSOs = Resources.LoadAll<EventData>("Assets/02. Scripts/Story/EventData SO/MainEvent");
        Debug.Log(MainSOs.Length);
        //메인 이벤트 큐에 데이터 로드
        foreach (EventData SO in MainSOs)
        {
            MainEventQueue.Enqueue(SO);
        }

        LoadSO();
        Debug.Log("Created TotalEventQueue" + TotalEventQueue.Count);
    }


    private void LoadSO()
    {
        
        EventData[] SubSOs = Resources.LoadAll<EventData>("Assets/02. Scripts/Story/EventData SO/MainEvent/");

        while(MainEventQueue.Count > 0)
        {
            //전체 이벤트 큐에 메인 이벤트 큐 데이터 로드
            EventData presentEvent = MainEventQueue.Dequeue();
            TotalEventQueue.Enqueue(presentEvent);

            //로드된 메인 이벤트에 다음 이벤트가 있다면 추가 로드
            if(presentEvent.nextEvent != null)
            {
                MainEventQueue.Enqueue(presentEvent.nextEvent);
            }

            //서브 이벤트 랜덤 로드
            for(int i = 0; i < subEventCount; i++)
            {
                EventData randomSubEvent = PickRandomElement(SubSOs);
                TotalEventQueue.Enqueue(randomSubEvent);
            }

        }            

    }

    // 제네릭 함수를 사용하여 리스트에서 랜덤 요소를 뽑기
    public T PickRandomElement<T>(T[] array)
    {
        System.Random random = new System.Random();
        int randomIndex = random.Next(array.Length);
        return array[randomIndex];
    }

}