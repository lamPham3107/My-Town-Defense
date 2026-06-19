using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioSource : MonoBehaviour
{
    public static GameAudioSource Instance { get; private set; }
    public AudioSource _sfxAudio;
    public AudioSource _bgmAudio;

    public AudioClip sfxZombieDie;
    public AudioClip sfxPlaceTower;
    public AudioClip sfxVictory;
    public AudioClip sfxGameOver;
    public AudioClip sfxBtnClick;
    public AudioClip sfxSellTower;

    public AudioClip bgmGamePlay;
    public AudioClip bgmMainMenu;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        ApplySavedVolume();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        _sfxAudio.PlayOneShot(clip);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        _bgmAudio.clip = clip;
        _bgmAudio.Play();
    }

    public void ApplySavedVolume()
    {
        if (GameMaster.instance == null) return;
        _sfxAudio.volume = GameMaster.SettingSound;
        _bgmAudio.volume = GameMaster.SettingMusic;
    }
}
