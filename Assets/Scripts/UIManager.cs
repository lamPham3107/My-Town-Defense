using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image img_select_tower;


    public void Close_SelectTower_img()
    {
        img_select_tower.gameObject.SetActive(false);
    }
}
