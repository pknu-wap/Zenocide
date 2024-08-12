using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    // 싱글톤
    public static SoundManager Instance { get; private set; }
    private void Awake() => Instance = this;
    
    public AudioSource storyMusicsource;
    public AudioSource battleMusicsource;

    public Slider masterVolumeSlider;
    public Slider bgmVolumeSlider;

    private float masterVolume = 1f; 
    private float bgmVolume = 1f;

    private float lastAppliedMasterVolume;
    private float lastAppliedBgmVolume;

    public int applyCount = 0;

    private void Start()
    {
        // 슬라이더의 초기 값 설정
        masterVolumeSlider.value = masterVolume; 
        bgmVolumeSlider.value = bgmVolume;

        // 슬라이더의 값 변경 이벤트 등록
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume); 
        bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);

        UpdateAudioSources(); 
    }

    // 전체 음량 슬라이더 값 변경 시 호출
    private void SetMasterVolume(float value)
    {
        masterVolume = value;  
        UpdateAudioSources();
    }

    // 배경 음악 슬라이더 값 변경 시 호출
    private void SetBGMVolume(float value)
    {
        bgmVolume = value;
        UpdateAudioSources(); 
    }

    // 오디오 소스의 볼륨을 업데이트
    private void UpdateAudioSources()
    {
        // 전체 음량이 0일 경우 다른 음량도 0으로 설정
        if (masterVolume <= 0f)
        {
            storyMusicsource.volume = 0f;
            battleMusicsource.volume = 0f;
            return;
        }

        // 전체 음량이 0이 아닐 경우 전체음량에 비례해서 음량 설정
        storyMusicsource.volume = bgmVolume * masterVolume;
        battleMusicsource.volume = bgmVolume * masterVolume;
    }

    // 볼륨 설정을 적용
    public void ApplyVolumeSettings()
    {
        // 현재 음량 적용
        lastAppliedBgmVolume = bgmVolume;
        lastAppliedMasterVolume = masterVolume; 
        // ApplyVolumeSettings가 호출될때마다 applyCount 1씩 증가
        applyCount++;
        UpdateAudioSources();

        // 설정한 옵션을 로컬 파일로 저장
        SaveVolumeSettings();
        DataManager.Instance.SaveData();
    }

    // 볼륨 설정을 취소
    public void CancelVolumeSettings()
    {
        // applyCount가 0인 경우에는 모든 음량 1f로 설정
        if (applyCount == 0)
        {
            bgmVolume = 1f;
            masterVolume = 1f;
        }
        else
        {
            // 마지막으로 적용된 음량 복원
            bgmVolume = lastAppliedBgmVolume;
            masterVolume = lastAppliedMasterVolume;
        }
        bgmVolumeSlider.value = bgmVolume;
        masterVolumeSlider.value = masterVolume;
        UpdateAudioSources();
    }

    public void SaveVolumeSettings()
    {
        // 현재 전체음량, 배경음악 값을 데이터에 저장
        DataManager.Instance.data.MasterVolume = masterVolume;
        DataManager.Instance.data.BgmVolume = bgmVolume;
    }

    public void LoadVolumeSettings()
    {
        // 데이터에서 저장된 전체음량, 배경음악 값을 불러옴
        masterVolume = DataManager.Instance.data.MasterVolume;
        bgmVolume = DataManager.Instance.data.BgmVolume;

        // 로드한 볼륨 적용
        masterVolumeSlider.value = masterVolume;
        bgmVolumeSlider.value = bgmVolume;

        ApplyVolumeSettings();
    }
}