using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioMixer audioMixer; // AudioMixer ����
    public Slider bgmSlider;      // BGM �����̴�
    public Slider sfxSlider;      // SFX �����̴�

    private void Start()
    {
        // �����̴� �� �ʱ�ȭ
        float savedBGMVolume = PlayerPrefs.GetFloat("BGMVolume", 0.75f);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        bgmSlider.value = savedBGMVolume;
        sfxSlider.value = savedSFXVolume;

        // �ʱ� ���� ����
        SetBGMVolume(savedBGMVolume);
        SetSFXVolume(savedSFXVolume);

        // �����̴� �̺�Ʈ ���
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetBGMVolume(float volume)
    {
        if (volume <= 0)
        {
            audioMixer.SetFloat("BGMVolume", -80f); // ���Ұ�
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
            audioMixer.SetFloat("SFXVolume", -80f); // ���Ұ�
        }
        else
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
}
