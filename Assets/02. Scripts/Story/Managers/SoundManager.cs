using Unity.VisualScripting;
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
    
    public AudioSource storyMusicsource;
    public AudioSource battleMusicsource;

    public Slider masterVolumeSlider;
    public Slider bgmVolumeSlider;
    public Toggle muteToggle;

    private float masterVolume = 1f; 
    private float bgmVolume = 1f;
    private bool isMuted = false;

    private float lastAppliedMasterVolume;
    private float lastAppliedBgmVolume;
    private bool lastAppliedMuteState;

    public int applyCount = 0;

    [Header("재생")]
    // BGM 재생기 개수
    private const int bgmAudioCount = 1;    // 무조건 하나로 제한하겠습니다.
    private const int effectAudioCount = 5;

    [SerializeField]
    private AudioSource bgmSource;
    [SerializeField]
    private AudioSource[] effectSources;

    private void Awake()
    {
        Instance = this;

        // 슬라이더, 토글의 초기 값 설정
        masterVolumeSlider.value = masterVolume; 
        bgmVolumeSlider.value = bgmVolume;
        muteToggle.isOn = false;

        // 슬라이더, 토글의 값 변경 이벤트 등록
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume); 
        bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        muteToggle.onValueChanged.AddListener(OnMuteToggleChanged);

        UpdateAudioSources();

        AssignAudioComponent();
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

    // 뮤트 토글 변경 시 호출
    private void OnMuteToggleChanged(bool isMuted)
    {
        this.isMuted = isMuted;
        AudioListener.pause = isMuted;
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
        lastAppliedMuteState = isMuted;
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
            isMuted = false;
        }
        else
        {
            // 마지막으로 적용된 음량 복원
            bgmVolume = lastAppliedBgmVolume;
            masterVolume = lastAppliedMasterVolume;
            isMuted = lastAppliedMuteState;
        }
        bgmVolumeSlider.value = bgmVolume;
        masterVolumeSlider.value = masterVolume;
        muteToggle.isOn = isMuted;

        UpdateAudioSources();
    }

    public void SaveVolumeSettings()
    {
        // 현재 전체음량, 배경음악 값을 데이터에 저장
        DataManager.Instance.data.MasterVolume = masterVolume;
        DataManager.Instance.data.BgmVolume = bgmVolume;
        DataManager.Instance.data.IsMuted = isMuted;
    }

    public void LoadVolumeSettings()
    {
        // 데이터에서 저장된 전체음량, 배경음악 값을 불러옴
        lastAppliedMasterVolume = DataManager.Instance.data.MasterVolume;
        lastAppliedBgmVolume = DataManager.Instance.data.BgmVolume;
        lastAppliedMuteState = DataManager.Instance.data.IsMuted;

        // 로드한 볼륨 적용
        masterVolumeSlider.value = lastAppliedMasterVolume;
        bgmVolumeSlider.value = lastAppliedBgmVolume;
        muteToggle.isOn = lastAppliedMuteState;

        ApplyVolumeSettings();
    }

    #region 오디오 재생
    private void AssignAudioComponent()
    {
        // 모든 컴포넌트를 가져온다.
        AudioSource[] audioSources = GetComponents<AudioSource>();

        // 모자라면 재생기를 생성한다.
        if (audioSources.Length < bgmAudioCount + effectAudioCount)
        {
            // 모자란 만큼 생성
            for (int i = audioSources.Length; i < bgmAudioCount + effectAudioCount; ++i)
            {
                gameObject.AddComponent<AudioSource>();
            }

            // 다시 컴포넌트들을 가져온다.
            audioSources = GetComponents<AudioSource>();
        }


        // BGM 재생기 할당
        bgmSource = audioSources[0];

        // 효과음 재생기 할당
        effectSources = new AudioSource[effectAudioCount];
        for (int i = 0; i < effectAudioCount; ++i)
        {
            effectSources[i] = audioSources[i + bgmAudioCount];
        }
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

        // 모두 재생 중이라면, 에러를 띄우고 null을 반환한다.
        Debug.LogError("모든 오디오 소스가 재생 중입니다!");
        return null;
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
            currentSource = GetIdleAudioSource(effectSources);
            // 출력 믹서 변경
            // currentSource.outputAudioMixerGroup = ;
        }

        // 재생 가능 소스가 없다면 에러 메세지를 띄우고 중단
        if(currentSource == null)
        {
            Debug.LogError("재생에 실패하였습니다.");
            return;
        }

        currentSource.clip = audioClip;
        currentSource.loop = isLooping;

        currentSource.Play();
    }
    #endregion 오디오 재생
}