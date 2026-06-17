using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enable_Built_Tower_Img : MonoBehaviour
{
    [SerializeField] private Image img_select_tower;
    public Vector2 tower_pos;
    public static Enable_Built_Tower_Img current_Base_Selected;

    public void Select()
    {
        if (GetComponentInChildren<TowerController>() != null) return;

        current_Base_Selected = this;
        tower_pos = transform.position;
        img_select_tower.gameObject.SetActive(true);
    }

    public static void Deselect()
    {
        if (current_Base_Selected != null)
        {
            current_Base_Selected.img_select_tower.gameObject.SetActive(false);
            current_Base_Selected = null;
        }
    }

}
