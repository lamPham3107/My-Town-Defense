using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Built_Tower : MonoBehaviour
{
    [SerializeField] private GameObject archer_prefab;
    [SerializeField] private GameObject gunner_prefab;
    [SerializeField] private GameObject bomber_prefab;
    [SerializeField] private GameObject electer_prefab;
    private Enable_Built_Tower_Img enable_built_tower_img;


    private Vector2 GetBuildPos()
    {
        if (Enable_Built_Tower_Img.current_Base_Selected == null)
        {
            Debug.LogWarning("Chưa chọn ô nào!");
            return Vector2.zero;
        }
        return Enable_Built_Tower_Img.current_Base_Selected.tower_pos;
    }
    public void Build_Archer()
    {
        Vector2 buildPos = GetBuildPos();
        ResourceManager.Instance.SpendGold(100);
        Instantiate(archer_prefab, buildPos, Quaternion.identity);
    }
    public void Build_Gunner()
    {
        Vector2 buildPos = GetBuildPos();
        ResourceManager.Instance.SpendGold(120);
        Instantiate(gunner_prefab, buildPos, Quaternion.identity);
    }
    public void Build_Bomber()
    {
        Vector2 buildPos = GetBuildPos();
        ResourceManager.Instance.SpendGold(150);
        Instantiate(bomber_prefab, buildPos, Quaternion.identity);
    }
    public void Build_Electer()
    {
        Vector2 buildPos = GetBuildPos();
        ResourceManager.Instance.SpendGold(120);
        Instantiate(electer_prefab, buildPos, Quaternion.identity);
    }
}
