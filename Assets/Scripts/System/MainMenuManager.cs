using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor.SearchService;

public class MainMenuManager : MonoBehaviour
{
    public void LoadGameScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
