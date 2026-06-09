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

    [SerializeField] private GameObject archer_prefab;
    [SerializeField] private GameObject gunner_prefab;
    [SerializeField] private GameObject bomber_prefab;
    [SerializeField] private GameObject electer_prefab;

    public GameObject BtnStartWave;
    private Enable_Built_Tower_Img enable_built_tower_img;

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

    private Vector2 GetBuildPos()
    {
        if (Enable_Built_Tower_Img.current_Base_Selected == null)
        {
            Debug.LogWarning("Chưa chọn ô nào!");
            return Vector2.zero;
        }
        return Enable_Built_Tower_Img.current_Base_Selected.tower_pos;
    }
    public void Build_Archer(int cost)
    {
        Debug.Log("Attempting to build Archer. Cost: " + cost);
        Close_SelectTower_img();
        if (!ResourceManager.Instance.SpendGold(cost))
        {
            return;
        }
        Vector2 buildPos = GetBuildPos();
        Transform parent = Enable_Built_Tower_Img.current_Base_Selected.transform;
        Instantiate(archer_prefab, buildPos, Quaternion.identity,parent);
    }
    public void Build_Gunner(int cost)
    {
        Close_SelectTower_img();
        if(!ResourceManager.Instance.SpendGold(cost))
        {
            return;
        }
        Vector2 buildPos = GetBuildPos();
        Transform parent = Enable_Built_Tower_Img.current_Base_Selected.transform;
        Instantiate(gunner_prefab, buildPos, Quaternion.identity, parent);
    }
    public void Build_Bomber(int cost)
    {
        Close_SelectTower_img();
        if (!ResourceManager.Instance.SpendGold(cost))
        {
            return;
        }
        Vector2 buildPos = GetBuildPos();
        Transform parent = Enable_Built_Tower_Img.current_Base_Selected.transform;
        Instantiate(bomber_prefab, buildPos, Quaternion.identity, parent);
    }
    public void Build_Electer(int cost)
    {
        Close_SelectTower_img();
        if (!ResourceManager.Instance.SpendGold(cost))
        {
            return;
        }
        Vector2 buildPos = GetBuildPos();
        Transform parent = Enable_Built_Tower_Img.current_Base_Selected.transform;
        Instantiate(electer_prefab, buildPos, Quaternion.identity, parent);
    }

    public void StartWave()
    {
        HideButtonStartWave();
        WaveManager.instance.StartNextWave();
    }
    public void ShowButtonStartWave()
    {
        BtnStartWave.SetActive(true);
    }
    public void HideButtonStartWave()
    {
        BtnStartWave.SetActive(false);
    }

}


