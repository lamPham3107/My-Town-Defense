using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private Slider _soundSlider;
    [SerializeField] private Slider _musicSlider;

    private void OnEnable()
    {
        _soundSlider.value = GameMaster.SettingSound;
        _musicSlider.value = GameMaster.SettingMusic;

        _soundSlider.onValueChanged.AddListener(OnSoundChanged);
        _musicSlider.onValueChanged.AddListener(OnMusicChanged);
    }

    private void OnDisable()
    {
        _soundSlider.onValueChanged.RemoveListener(OnSoundChanged);
        _musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
    }

    private void OnSoundChanged(float value)
    {
        GameMaster.SettingSound = value;
        GameAudioSource.Instance._sfxAudio.volume = value; 
    }

    private void OnMusicChanged(float value)
    {
        GameMaster.SettingMusic = value;
        GameAudioSource.Instance._bgmAudio.volume = value; // apply ngay
    }
}