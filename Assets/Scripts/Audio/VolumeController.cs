using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [SerializeField] VolumeType type;
    
    [SerializeField] Slider slider;
    [SerializeField] float multiplier = 30f;
    [SerializeField] float defaultValue = 0.8f;
    [SerializeField] Toggle toggle;

    private bool disableToggleEvent = false;
    private string volumeParameter;
    private AudioMixer mixer;

    public void Init()
    {
        volumeParameter = VolumeManager.GetInstance().GetVolumeTypeMixerName(type);
        mixer = VolumeManager.GetInstance().GetAudioMixer();

        slider.onValueChanged.AddListener(HandleSliderValueChanged);
        toggle.onValueChanged.AddListener(HandleToggleValueChanged);

        // load and set volume
        float initVolumeValue = PlayerPrefs.GetFloat(volumeParameter, defaultValue);

        mixer.SetFloat(volumeParameter, Mathf.Log10(initVolumeValue) * multiplier);
        toggle.isOn = initVolumeValue > slider.minValue;
        slider.value = initVolumeValue;
    }

    private void OnDestroy()
    {
        //save sound levels
        PlayerPrefs.SetFloat(volumeParameter, slider.value);
    }

    private void HandleToggleValueChanged(bool enableSound)
    {
        VolumeManager.GetInstance().CheckAndHandleToggleChange(type, enableSound);

        if (disableToggleEvent) return;

        if (enableSound)
        {
            slider.value = defaultValue;
        }
        else
        {
            slider.value = slider.minValue;
        }
    }

    private void HandleSliderValueChanged(float value)
    {
        mixer.SetFloat(volumeParameter, Mathf.Log10(value) * multiplier);
        disableToggleEvent = true;
        toggle.isOn = slider.value > slider.minValue;
        disableToggleEvent = false;
    }

    public float GetVolume()
    {
        return slider.value;
    }
    
    public VolumeType GetVolumeType()
    {
        return type;
    }

    public float GetDefaultVolume()
    {
        return defaultValue;
    }
}
