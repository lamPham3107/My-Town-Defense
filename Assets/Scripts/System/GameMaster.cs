using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;
    private float settingSound;
    private float settingMusic;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        LoadData();
    }

    private void LoadData()
    {
        settingSound = PlayerPrefs.GetFloat("setting_sound", 1f);
        settingMusic = PlayerPrefs.GetFloat("setting_music", 1f);
    }

    public static float SettingSound
    {
        get => instance.settingSound;
        set
        {
            instance.settingSound = value;
            PlayerPrefs.SetFloat("setting_sound", value);
            PlayerPrefs.Save();

        }
    }

    public static float SettingMusic
    {
        get => instance.settingMusic;
        set
        {
            instance.settingMusic = value;
            PlayerPrefs.SetFloat("setting_music", value);
            PlayerPrefs.Save();
        }
    }

    public static int GetLevelStars(int level)
    {
        return PlayerPrefs.GetInt($"level_{level}_stars", 0);
    }

    public static void SetLevelStars(int level, int stars)
    {
        int currentStars = GetLevelStars(level);
        if (stars > currentStars)
        {
            PlayerPrefs.SetInt($"level_{level}_stars", stars);
            PlayerPrefs.Save();
        }
    }
    public static bool IsLevelUnlocked(int level)
    {
        if (level == 1) return true;
        return PlayerPrefs.GetInt($"level_{level}_unlocked", 0) == 1;
    }

    public static void UnlockLevel(int level)
    {
        PlayerPrefs.SetInt($"level_{level}_unlocked", 1);
        PlayerPrefs.Save();
    }

    public static void SaveLevelResult(int level, int stars)
    {
        SetLevelStars(level, stars);
        UnlockLevel(level + 1);
    }

    #if UNITY_EDITOR
        [MenuItem("Tools/Reset All Data")]
        public static void ResetAll()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("All data cleared!");
        }
    #endif

}
