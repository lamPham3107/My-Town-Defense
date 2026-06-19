using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private LevelButton[] _levelButtons;
    public GameObject pn_Setting;
    private void Start()
    {
        GameAudioSource.Instance.PlayBGM(GameAudioSource.Instance.bgmMainMenu);
        SetupAllLevels();
    }
    public void LoadGameScene(string sceneName)
    {
        GameAudioSource.Instance.PlaySFX(GameAudioSource.Instance.sfxBtnClick);
        SceneManager.LoadScene(sceneName);
    }
    private void SetupAllLevels()
    {
        foreach (var btn in _levelButtons)
            btn.Setup();
    }

    public void OpenSettingPanel()
    {
        GameAudioSource.Instance.PlaySFX(GameAudioSource.Instance.sfxBtnClick);
        pn_Setting.SetActive(true);
    }

    public void CloseSettingPanel()
    {
        GameAudioSource.Instance.PlaySFX(GameAudioSource.Instance.sfxBtnClick);
        pn_Setting.SetActive(false);
    }

}
