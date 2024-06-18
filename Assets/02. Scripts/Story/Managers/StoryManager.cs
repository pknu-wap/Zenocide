using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public StoryNotification notification;

    private void Awake()
    {
        notification = GameObject.Find("Story Notification Panel").GetComponent<StoryNotification>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            notification.ShowGetItemMessage("총");
        }
    }
}
