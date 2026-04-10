using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enable_Built_Tower_Img : MonoBehaviour
{
    [SerializeField] private Image img_select_tower;
    public Vector2 tower_pos;
    public static Enable_Built_Tower_Img current_Base_Selected;
    private void OnMouseDown()
    {
        Select_Tower();
    }
    private void Select_Tower()
    {
        current_Base_Selected = this;
        tower_pos = transform.position;
        img_select_tower.gameObject.SetActive(true);
    }




}
