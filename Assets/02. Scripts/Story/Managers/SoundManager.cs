using System;
using UnityEngine;
using UnityEngine.UI;

// 오디오 종류
public enum SoundType
{
    Bgm,        // 배경음악
    Effect,     // 효과음
}

public class SoundManager : MonoBehaviour
{
    // 싱글톤
    public static SoundManager Instance { get; private set; }

    public Slider masterVolumeSlider;
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;

    public Toggle masterMuteToggle;
    public Toggle bgmMuteToggle;
    public Toggle sfxMuteToggle;

    private float masterVolume = 1f; 
    private float bgmVolume = 1f;
    private float sfxVolume = 1f;

    private bool isMasterVolumeMuted = false;
    private bool isBgmVolumeMuted = false;
    private bool isSfxVolumeMuted = false;

    private float lastAppliedMasterVolume;
    private float lastAppliedBgmVolume;
    private float lastAppliedSfxVolume;

    private bool lastAppliedMasterMuteState;
    private bool lastAppliedBgmMuteState;
    private bool lastAppliedSfxMuteState;

    public int applyCount = 0;

    [Header("재생")]
    // BGM 재생기 개수
    private const int bgmAudioCount = 1;    // 무조건 하나로 제한하겠습니다.
    private const int sfxAudioCount = 8;

    [SerializeField]
    private AudioSource bgmSource;
    [SerializeField]
    private AudioSource[] sfxSources;

    private void Awake()
    {
        Instance = this;

        // 슬라이더, 토글의 초기 값 설정
        masterVolumeSlider.value = masterVolume; 
        bgmVolumeSlider.value = bgmVolume;
        sfxVolumeSlider.value = sfxVolume;

        masterMuteToggle.isOn = false;
        bgmMuteToggle.isOn = false;
        sfxMuteToggle.isOn = false;

        // 슬라이더의 값 변경 이벤트 등록
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume); 
        bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

        // 토글의 값 변경 이벤트 등록
        masterMuteToggle.onValueChanged.AddListener(OnMasterMuteToggleChanged);
        bgmMuteToggle.onValueChanged.AddListener(OnBGMMuteToggleChanged);
        sfxMuteToggle.onValueChanged.AddListener(OnSFXMuteToggleChanged);

        AssignAudioComponent();
        InitializeSfxTimeStamp();

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

    private void SetSFXVolume(float value)
    {
        sfxVolume = value;
        UpdateAudioSources();
    }

    // 뮤트 토글 변경 시 호출
    private void OnMasterMuteToggleChanged(bool isMuted)
    {
        isMasterVolumeMuted = isMuted;

        // 마스터가 뮤트 상태일 때 BGM과 SFX 오디오 소스도 뮤트로 설정
        if (isMasterVolumeMuted)
        {
            bgmSource.mute = true;
            foreach (var sfxSource in sfxSources)
            {
                sfxSource.mute = true;
            }
        }
        else
        {
            // 마스터가 켜지면 BGM과 SFX 상태를 토글에 맞게 설정
            if (!isBgmVolumeMuted) bgmSource.mute = false;
            if (!isSfxVolumeMuted)
            {
                foreach (var sfxSource in sfxSources)
                {
                    sfxSource.mute = false;
                }
            }
        }
    }

    private void OnBGMMuteToggleChanged(bool isMuted)
    {
        isBgmVolumeMuted = isMuted;

        // BGM의 상태에 맞게 오디오 뮤트 처리
        if (isBgmVolumeMuted)
        {
            bgmSource.mute = true;
        }
        else
        {
            // 마스터가 뮤트 상태가 아니면 BGM을 언뮤트
            if (!isMasterVolumeMuted)
            {
                bgmSource.mute = false;
            }
        }
    }

    private void OnSFXMuteToggleChanged(bool isMuted)
    {
        isSfxVolumeMuted = isMuted;

        // SFX의 상태에 맞게 오디오 뮤트 처리
        if (isMuted)
        {
            foreach (var sfxSource in sfxSources)
            {
                sfxSource.mute = true;
            }
        }
        else
        {
            // 마스터가 뮤트 상태가 아니면 SFX를 언뮤트
            if (!isMasterVolumeMuted)
            {
                foreach (var sfxSource in sfxSources)
                {
                    sfxSource.mute = false;
                }
            }
        }
    }
    // 오디오 소스의 볼륨을 업데이트
    private void UpdateAudioSources()
    {
        if (!masterMuteToggle.isOn)
        {
            // 뮤트가 아닌 경우 마스터 볼륨 적용
            bgmSource.volume = bgmVolume * masterVolume;
            for (int i = 0; i < sfxSources.Length; ++i)
            {
                sfxSources[i].volume = sfxVolume * masterVolume;
            }
        }
    }

    // 볼륨 설정을 적용
    public void ApplyVolumeSettings()
    {
        // 현재 음량 적용
        lastAppliedBgmVolume = bgmVolume;
        lastAppliedSfxVolume = sfxVolume;
        lastAppliedMasterVolume = masterVolume;

        lastAppliedMasterMuteState = isMasterVolumeMuted;
        lastAppliedBgmMuteState = isBgmVolumeMuted;
        lastAppliedSfxMuteState = isSfxVolumeMuted;

        lastAppliedMasterMuteState = masterMuteToggle.isOn;
        lastAppliedBgmMuteState = bgmMuteToggle.isOn;
        lastAppliedSfxMuteState = sfxMuteToggle.isOn;

        applyCount++;
        UpdateAudioSources();

        // 설정한 옵션을 로컬 파일로 저장
        SaveVolumeSettings();
        DataManager.Instance.SaveData();
    }

    // 볼륨 설정을 취소
    public void CancelVolumeSettings()
    {
        if (applyCount == 0)
        {
            bgmVolume = 1f;
            sfxVolume = 1f;
            masterVolume = 1f;

            isMasterVolumeMuted = false;
            isBgmVolumeMuted = false;
            isSfxVolumeMuted = false;
        }
        else
        {
            // 마지막으로 적용된 음량 복원
            bgmVolume = lastAppliedBgmVolume;
            sfxVolume = lastAppliedSfxVolume;
            masterVolume = lastAppliedMasterVolume;

            isMasterVolumeMuted = lastAppliedMasterMuteState;
            isBgmVolumeMuted = lastAppliedBgmMuteState;
            isSfxVolumeMuted = lastAppliedSfxMuteState;
        }
        bgmVolumeSlider.value = bgmVolume;
        sfxVolumeSlider.value = sfxVolume;
        masterVolumeSlider.value = masterVolume;

        masterMuteToggle.isOn = isMasterVolumeMuted;
        bgmMuteToggle.isOn = isBgmVolumeMuted;
        sfxMuteToggle.isOn = isSfxVolumeMuted;

        UpdateAudioSources();
    }

    public void SaveVolumeSettings()
    {
        // 현재 전체음량, 배경음악 값을 데이터에 저장
        DataManager.Instance.data.MasterVolume = masterVolume;
        DataManager.Instance.data.BgmVolume = bgmVolume;
        DataManager.Instance.data.SfxVolume = sfxVolume;

        DataManager.Instance.data.IsMasterVolumeMuted = isMasterVolumeMuted;
        DataManager.Instance.data.IsBgmVolumeMuted = isBgmVolumeMuted;
        DataManager.Instance.data.IsSfxVolumeMuted = isSfxVolumeMuted;

        DataManager.Instance.data.IsMasterVolumeMuted = masterMuteToggle.isOn;
        DataManager.Instance.data.IsBgmVolumeMuted = bgmMuteToggle.isOn;
        DataManager.Instance.data.IsSfxVolumeMuted = sfxMuteToggle.isOn;
    }

    public void LoadVolumeSettings()
    {
        // 데이터에서 저장된 전체음량, 배경음악 값을 불러옴
        lastAppliedMasterVolume = DataManager.Instance.data.MasterVolume;
        lastAppliedBgmVolume = DataManager.Instance.data.BgmVolume;
        lastAppliedSfxVolume = DataManager.Instance.data.SfxVolume;
        
        lastAppliedMasterMuteState = DataManager.Instance.data.IsMasterVolumeMuted;
        lastAppliedBgmMuteState = DataManager.Instance.data.IsBgmVolumeMuted;
        lastAppliedSfxMuteState = DataManager.Instance.data.IsSfxVolumeMuted;

        // 로드한 볼륨 적용
        masterVolumeSlider.value = lastAppliedMasterVolume;
        bgmVolumeSlider.value = lastAppliedBgmVolume;
        sfxVolumeSlider.value = lastAppliedSfxVolume;
        
        masterMuteToggle.isOn = lastAppliedMasterMuteState;
        bgmMuteToggle.isOn = lastAppliedBgmMuteState;
        sfxMuteToggle.isOn = lastAppliedSfxMuteState;

        ApplyVolumeSettings();
    }

    #region 오디오 재생
    private void AssignAudioComponent()
    {
        // 모든 컴포넌트를 가져온다.
        AudioSource[] audioSources = GetComponents<AudioSource>();

        // 모자라면 재생기를 생성한다.
        if (audioSources.Length < bgmAudioCount + sfxAudioCount)
        {
            // 모자란 만큼 생성
            for (int i = audioSources.Length; i < bgmAudioCount + sfxAudioCount; ++i)
            {
                gameObject.AddComponent<AudioSource>();
            }

            // 다시 컴포넌트들을 가져온다.
            audioSources = GetComponents<AudioSource>();
        }


        // BGM 재생기 할당
        bgmSource = audioSources[0];

        // 효과음 재생기 할당
        sfxSources = new AudioSource[sfxAudioCount];
        for (int i = 0; i < sfxAudioCount; ++i)
        {
            sfxSources[i] = audioSources[i + bgmAudioCount];
        }
    }

    // 오디오 클립을 재생한다.
    public void Play(AudioClip audioClip, SoundType type = SoundType.Effect, bool isLooping = false)
    {
        AudioSource currentSource;

        if (type == SoundType.Bgm)
        {
            // 같은 클립이 재생 중이라면
            if (audioClip == bgmSource.clip)
            {
                // 중단한다.
                return;
            }

            // 그 외엔 BGM 소스 선택
            currentSource = bgmSource;
            // 출력 믹서 변경
            // currentSource.outputAudioMixerGroup = ;
        }

        else
        {
            // 쉬고 있는 오디오 소스를 찾는다.
            currentSource = GetIdleAudioSource(sfxSources);

            // 출력 믹서 변경
            // currentSource.outputAudioMixerGroup = ;

            // 재생 가능 소스가 없다면 가장 오래된 소스를 가져온다.
            if (currentSource == null)
            {
                currentSource = GetOldestAudioSource();
            }

            // 최종 선택된 오디오 소스에 시간을 체크한다.
            int playedSfxIndex = Array.IndexOf(sfxSources, currentSource);
            CheckSfxTimeStemp(playedSfxIndex);
        }

        currentSource.clip = audioClip;
        currentSource.loop = isLooping;

        currentSource.Play();
    }

    // 효과음별 재생된 시간 기록
    int[] sfxPlayedTime = new int[sfxAudioCount];
    // 효과음 재생 시간
    int sfxTimeStamp = 0;
    // sfxTimeStamp가 가질 수 있는 최대 값 (오버플로우 방지)
    int sfxMaxTimeStamp = 2000000000;

    private void InitializeSfxTimeStamp()
    {
        Array.Clear(sfxPlayedTime, 0, sfxAudioCount);
    }

    // 현재 쉬고 있는 AudioSource를 찾아 반환한다.
    private AudioSource GetIdleAudioSource(AudioSource[] audioSources)
    {
        // 모든 오디오 소스 중에
        for (int i = 0; i < audioSources.Length; ++i)
        {
            // 재생 중이지 않은 오디오 소스를 찾아서
            if (audioSources[i].isPlaying == false)
            {
                // 반환한다.
                return audioSources[i];
            }
        }

        // 모두 재생 중이라면 null을 반환한다.
        return null;
    }

    private AudioSource GetOldestAudioSource()
    {
        int oldestIndex = GetIndexOfMin(sfxPlayedTime);

        return sfxSources[oldestIndex];
    }


    private void CheckSfxTimeStemp(int playedSfxIndex)
    {
        sfxPlayedTime[playedSfxIndex] = ++sfxTimeStamp;

        if(sfxTimeStamp > sfxMaxTimeStamp)
        {
            for(int i = 0; i < sfxPlayedTime.Length; ++i)
            {
                sfxPlayedTime[i] -= sfxMaxTimeStamp;
            }
        }
    }

    private int GetIndexOfMin(int[] array)
    {
        if(array.Length == 1)
        {
            return 0;
        }

        int minIndex = 0;

        for(int i = 1; i < array.Length; ++i)
        {
            if (array[i] < array[minIndex])
            {
                minIndex = i;
            }
        }

        return minIndex;
    }
    #endregion 오디오 재생
}