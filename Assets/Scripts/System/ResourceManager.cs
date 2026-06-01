using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;
    public int Gold { get; private set; }
    public int Lives { get; private set; }

    public System.Action<int> OnGoldChanged;
    public System.Action<int> OnLivesChanged;
    public System.Action OnGameOver;

    void Awake()
    {
        if (Instance != null) { 
            Destroy(gameObject); 
            return; 
        }
        else
        {
            Instance = this;
        }
    }

    public void Init(int startGold, int startLives)
    {
        Gold = startGold;
        Lives = startLives;
        UIManager.Instance.SetGoldLive();
        OnGoldChanged?.Invoke(Gold);
        OnLivesChanged?.Invoke(Lives);
    }

    public void AddGold(int amount)
    {
        Gold = Mathf.Min(Gold + amount, 999999);
        UIManager.Instance.SetGoldLive();
        OnGoldChanged?.Invoke(Gold);
    }

    public bool SpendGold(int amount)
    {
        if (Gold < amount) return false;
        Gold -= amount;
        UIManager.Instance.SetGoldLive();
        OnGoldChanged?.Invoke(Gold);
        return true;
    }

    public void LoseLives(int amount)
    {
        Lives = Mathf.Max(0, Lives - amount);
        UIManager.Instance.SetGoldLive();
        OnLivesChanged?.Invoke(Lives);
        if (Lives <= 0) OnGameOver?.Invoke();
    }
}
