using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;

public class VideoOption : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Button applyButton;

    private Resolution[] resolutions;
    private bool fullscreen;
    private int selectedResolutionIndex;
    void Start()
    {
        // 해상도 리스트 가져오기
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        // Dropdown에 옵션 추가하기
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
        }
        resolutionDropdown.AddOptions(options);

        // 현재 해상도 선택
        selectedResolutionIndex = FindCurrentResolutionIndex();
        resolutionDropdown.value = selectedResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // 토글 상태 설정
        fullscreenToggle.isOn = Screen.fullScreen;

        // 버튼 클릭 시 적용
        applyButton.onClick.AddListener(ApplySettings);
    }
    // 현재 해상도의 인덱스를 찾는 메서드
    private int FindCurrentResolutionIndex()
    {
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                return i;
            }
        }
        return 0;
    }
    // 설정 적용 메서드
    public void ApplySettings()
    {
        selectedResolutionIndex = resolutionDropdown.value;
        Resolution resolution = resolutions[selectedResolutionIndex];
        fullscreen = fullscreenToggle.isOn;

        // 해상도와 전체화면 설정 적용
        Screen.SetResolution(resolution.width, resolution.height, fullscreen);
    }
}