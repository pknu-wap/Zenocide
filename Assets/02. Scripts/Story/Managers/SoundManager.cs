using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource musicsource;

    public void SetMusiccVolume(float volume)
    {
        // 인수를 받아서 오디오 소스의 볼륨 조절
        musicsource.volume = volume;
    }
}
