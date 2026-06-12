using UnityEngine;
using UnityEngine.EventSystems;

public class SkillBomb : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public static SkillBomb Instance;
    [Header("Config")]
    [SerializeField] private float _radius = 2.5f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private int _manaCost = 1;
    [Header("References")]
    [SerializeField] private GameObject _crosshairPrefab;
    [SerializeField] private Camera _mainCamera;

    private GameObject _crosshairGO;
    public bool _isDragging = false;

    void Awake() => Instance = this;

    // Người chơi nhấn xuống nút Skill
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isDragging) return;

        Vector3 worldPos = ScreenToWorld(eventData.position);
        _crosshairGO = Instantiate(_crosshairPrefab, worldPos, Quaternion.identity);
        _isDragging = true;
    }

    // Người chơi kéo (vẫn đang giữ tay)
    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging || _crosshairGO == null) return;

        Vector3 worldPos = ScreenToWorld(eventData.position);
        _crosshairGO.transform.position = worldPos;
    }

    // Người chơi nhả tay -> nổ
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDragging || _crosshairGO == null) return;

        Vector3 worldPos = ScreenToWorld(eventData.position);
        Explode(worldPos);
    }

    private Vector3 ScreenToWorld(Vector2 screenPos)
    {
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y,
                        Mathf.Abs(_mainCamera.transform.position.z)));
        worldPos.z = 0f;
        return worldPos;
    }

    private void Explode(Vector3 center)
    {
        var hits = Physics2D.OverlapCircleAll(center, _radius, LayerMask.GetMask("Enemy"));
        foreach (var hit in hits)
        {
            var zombie = hit.GetComponent<ZombieController>();
            if (zombie != null && !zombie._isDead)
                zombie.TakeDamage(_damage, DamageType.Physical);
        }
        Debug.Log($"Bomb exploded at {center}, hit {hits.Length} zombies");

        Destroy(_crosshairGO);
        _crosshairGO = null;
        _isDragging = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}