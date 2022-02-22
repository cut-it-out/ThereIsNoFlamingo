using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] AudioSource effectAudioSource;
    [SerializeField] AudioSource ambientAudioSource;

    [SerializeField] AudioClip gameoverClip;
    [SerializeField] AudioClip damageClip;
    [SerializeField] AudioClip positiveFeedbackClip;

    public bool IsAmbientEnabled { get; private set; }
    public bool IsMusicEnabled { get; private set; }
    public bool IsEffectEnabled { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        IsAmbientEnabled = true;
        IsMusicEnabled = true;
        IsEffectEnabled = true;

        Game.GetInstance().OnRealViewToggle += Audio_OnRealViewToggle;
    }

    private void Audio_OnRealViewToggle(object sender, Game.RealViewEventArgs e)
    {
        EnableMusicEffect(e.isRealViewActive);
    }

    private void EnableMusicEffect(bool value)
    {
        musicAudioSource.bypassEffects = value;
    }

    private void Start()
    {
        VolumeManager.GetInstance().InitVolumeControllers();
    }

    public void PlayGameoverSound()
    {
        if (IsEffectEnabled)
        {
            effectAudioSource.PlayOneShot(gameoverClip);
        }
    }

    public void PlayDamageSound()
    {
        if (IsEffectEnabled)
        {
            effectAudioSource.PlayOneShot(damageClip);
        }
    }
    
    public void PlayPositiveFeedbackSound()
    {
        if (IsEffectEnabled)
        {
            effectAudioSource.PlayOneShot(positiveFeedbackClip);
        }
    }

    public void SetAmbient(bool value)
    {
        if (IsAmbientEnabled == value) return; // do not run if setting is already the same

        IsAmbientEnabled = value;
        if (IsAmbientEnabled)
        {
            ambientAudioSource.Play();
        }
        else
        {
            ambientAudioSource.Stop();
        }
    }

    public void SetEffect(bool value)
    {
        IsEffectEnabled = value;
    }

    public void SetMusic(bool value)
    {
        //if (IsMusicEnabled == value) return; // do not run if setting is already the same

        IsMusicEnabled = value;
        if (IsMusicEnabled)
        {
            musicAudioSource.Play();
        }
        else
        {
            musicAudioSource.Stop();
        }
    }

    public void StopMusic()
    {
        musicAudioSource.Stop();
    }
}

