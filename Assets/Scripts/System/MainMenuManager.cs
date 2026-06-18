using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private LevelButton[] _levelButtons;
    private void Start()
    {
        SetupAllLevels();
    }
    public void LoadGameScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    private void SetupAllLevels()
    {
        foreach (var btn in _levelButtons)
            btn.Setup();
    }


}
