using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("------------- Audio Source -------------")]
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource SFXSource;
    
    [Header("------------- Audio Clip -------------")]
    public AudioClip background;
    public AudioClip musicbackground;
    public AudioClip walk;
    public AudioClip attack;
    public AudioClip equip;
    public AudioClip chest;
    public AudioClip death;
    public AudioClip victory;
    public AudioClip complete;

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
        musicSource.clip = musicbackground;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
