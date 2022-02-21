using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public enum VolumeType
{
    Master,
    Music,
    Ambient,
    Effect
}

[System.Serializable]
public class VolumeTypeItem
{
    public VolumeType volumeType;
    public string mixerExposedParamName;
}

public class VolumeManager : Singleton<VolumeManager>
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] List<VolumeTypeItem> volumeExposedParamNames = new List<VolumeTypeItem>();

    AudioManager audioManager;
    List<VolumeController> volumeControllerList;
     
    protected override void Awake()
    {
        base.Awake();
        volumeControllerList = GetComponentsInChildren<VolumeController>().ToList();
        audioManager = AudioManager.GetInstance();


    }
    public void InitVolumeControllers()
    {
        volumeControllerList.ForEach(x => x.Init());
    }

    public string GetVolumeTypeMixerName(VolumeType vt)
    {
        VolumeTypeItem desiredVolumeTypeItem = volumeExposedParamNames.Find(x => x.volumeType == vt);
        if (desiredVolumeTypeItem != null)
        {
            return desiredVolumeTypeItem.mixerExposedParamName;
        }
        else
        {
            Debug.LogWarning($"The {desiredVolumeTypeItem.volumeType} volumeExposedParamNames entry was not found!");
            return "NotFound";
        }
    }

    public AudioMixer GetAudioMixer()
    {
        return mixer;
    }

    public float GetVolume(VolumeType vt)
    {
        VolumeController desiredVolumeController = volumeControllerList.Find(x => x.GetVolumeType() == vt);
        if (desiredVolumeController != null)
        {
            return desiredVolumeController.GetVolume();
        }
        else 
        { 
            Debug.LogWarning($"The {desiredVolumeController.GetVolumeType()} volumeController was not found!");
            return 0;
        }
    }

    public void CheckAndHandleToggleChange(VolumeType vt, bool isOn)
    {
        switch (vt)
        {
            case VolumeType.Music:
                audioManager.SetMusic(isOn);
                break;
            case VolumeType.Ambient:
                audioManager.SetAmbient(isOn);
                break;
            case VolumeType.Effect:
                audioManager.SetEffect(isOn);
                break;
        }
    }
}