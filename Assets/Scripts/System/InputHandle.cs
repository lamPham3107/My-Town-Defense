using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandle : MonoBehaviour
{
    private void Update()
    {
        // Kiểm tra có input không (mouse hoặc touch)
        if (!GetInputDown(out Vector2 screenPos)) return;

        // Chặn nếu đang click vào UI
        if (EventSystem.current.IsPointerOverGameObject()) return;
        // Touch cần check riêng
        if (IsTouchOverUI()) return;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPos, Vector2.zero);

        // Loop 1: ưu tiên Tower
        foreach (var hit in hits)
        {
            var tower = hit.collider.GetComponent<TowerController>();
            if (tower != null)
            {
                HideAll();
                TowerPanelMenu.Instance.Show(tower);
                return;
            }
        }

        // Loop 2: check Base slot
        foreach (var hit in hits)
        {
            var baseSlot = hit.collider.GetComponent<Enable_Built_Tower_Img>();
            if (baseSlot != null)
            {
                HideAll();
                baseSlot.Select();
                return;
            }
        }

        HideAll();
    }

    // Trả về true nếu có click/touch, kèm vị trí screen
    private bool GetInputDown(out Vector2 screenPos)
    {
        // Mouse
        if (Input.GetMouseButtonDown(0))
        {
            screenPos = Input.mousePosition;
            return true;
        }

        // Touch
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            screenPos = Input.GetTouch(0).position;
            return true;
        }

        screenPos = Vector2.zero;
        return false;
    }

    // Check touch có đang đè lên UI không
    private bool IsTouchOverUI()
    {
        if (Input.touchCount == 0) return false;
        return EventSystem.current.IsPointerOverGameObject(
            Input.GetTouch(0).fingerId);
    }

    void HideAll()
    {
        TowerPanelMenu.Instance.Hide();
        Enable_Built_Tower_Img.Deselect();
    }
}