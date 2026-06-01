using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;
    [SerializeField] private Image img_select_tower;
    public TextMeshProUGUI txt_gold;
    public TextMeshProUGUI txt_life;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    public void Close_SelectTower_img()
    {
        img_select_tower.gameObject.SetActive(false);
    }

    public void SetGoldLive()
    {
        txt_gold.text = ResourceManager.Instance.Gold.ToString();
        txt_life.text = ResourceManager.Instance.Lives.ToString();
    }
}
