using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioSource musicsource;
    public Slider volumeSlider;

    private float previousVolume;

    void Start()
    {
        previousVolume = musicsource.volume;
        volumeSlider.value = previousVolume;
    }

    public void SetMusicVolume(float volume)
    {
        musicsource.volume = volume;
    }

    // 설정 적용
    public void ApplyVolumeSettings()
    {
        float volume = volumeSlider.value;
        previousVolume = musicsource.volume;
        SetMusicVolume(volume);
    }

    // 설정 취소 및 복구
    public void CancelVolumeSettings()
    {
        SetMusicVolume(previousVolume);
        // 슬라이더 값도 이전 값으로 복원
        volumeSlider.value = previousVolume;
    }
    public void PreviewVolumeChange(float volume)
    {
        SetMusicVolume(volume);
    }
}