using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioMixer audioMixer; // AudioMixer 참조
    public Slider bgmSlider;      // BGM 슬라이더
    public Slider sfxSlider;      // SFX 슬라이더

    private void Start()
    {
        // 슬라이더 값 초기화
        float savedBGMVolume = PlayerPrefs.GetFloat("BGMVolume", 0.75f);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        bgmSlider.value = savedBGMVolume;
        sfxSlider.value = savedSFXVolume;

        // 초기 볼륨 설정
        SetBGMVolume(savedBGMVolume);
        SetSFXVolume(savedSFXVolume);

        // 슬라이더 이벤트 등록
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetBGMVolume(float volume)
    {
        if (volume <= 0)
        {
            audioMixer.SetFloat("BGMVolume", -80f); // 음소거
        }
        else
        {
            audioMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (volume <= 0)
        {
            audioMixer.SetFloat("SFXVolume", -80f); // 음소거
        }
        else
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
}
