using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class VideoOption : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    private Resolution[] resolutions;
    private List<string> options = new List<string>();
    void Start()
    {
        // Dropdown 초기화
        resolutions = Screen.resolutions;
        // 현재 해상도 Index 초기화
        int currentResolutionIndex = 0;
        // 모든 해상도 순회하며 Dropdown에 options 추가
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        // Dropdown에 옵션들을 추가하고 현재 해상도를 선택함
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        // 전체화면 설정을 Toggle에 반영
        fullscreenToggle.isOn = Screen.fullScreen;
    }
    // 설정 저장 함수
    public void SaveSettings()
    {
        // 해상도 설정
        int resolutionIndex = resolutionDropdown.value;
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, fullscreenToggle.isOn);
        // 전체화면 설정
        Screen.fullScreen = fullscreenToggle.isOn;
    }
}
