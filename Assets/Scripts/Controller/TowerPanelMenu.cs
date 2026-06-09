using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerPanelMenu : MonoBehaviour
{
    public static TowerPanelMenu Instance;

    public Button btnUpgrade;
    public Button btnSell;
    public Canvas canvas;
    private TowerController currentTower;
    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(TowerController tower)
    {
        currentTower = tower;

        // Convert world position → screen position → UI position
        Vector3 worldPos = tower.transform.position + Vector3.up * 1.5f;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPos,
            canvas.worldCamera,
            out Vector2 localPos
        );

        GetComponent<RectTransform>().localPosition = localPos;

        // Gán lại event mỗi lần show
        btnUpgrade.onClick.RemoveAllListeners();
        btnSell.onClick.RemoveAllListeners();

        btnUpgrade.onClick.AddListener(() => currentTower.Upgrade());
        btnSell.onClick.AddListener(() => currentTower.Sell());

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        currentTower = null;
        gameObject.SetActive(false);
    }
}
