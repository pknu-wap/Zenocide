using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public GameObject storyCamera;
    public GameObject battleCamera;
    public Option option;

    private void Start()
    {
        // 시작은 스토리
        SwitchToStoryScene();
    }

    public void SwitchToStoryScene()
    {
        storyCamera.SetActive(true);
        battleCamera.SetActive(false);
    }

    public void SwitchToBattleScene()
    {
        storyCamera.SetActive(false);
        battleCamera.SetActive(true);
    }
}
