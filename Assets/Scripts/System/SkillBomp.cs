using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillBomb : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public static SkillBomb Instance;
    [Header("Config")]
    [SerializeField] private float _radius = 0.5f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _cooldownTime = 60f;

    [Header("References")]
    [SerializeField] private GameObject _crosshairPrefab;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private CanvasGroup _buttonCanvasGroup; // để làm tối + chặn click
    public TextMeshProUGUI txt_CoolDown;
    public Animator animator;

    [Header("Range Circle")]
    [SerializeField] private Color _rangeColor = new Color(1f, 0.3f, 0.3f, 0.5f);
    [SerializeField] private int _circleSegments = 50;

    private GameObject _crosshairGO;
    public bool _isDragging = false;
    private bool _onCooldown = false;

    void Awake() => Instance = this;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isDragging || _onCooldown) return;

        Vector3 worldPos = ScreenToWorld(eventData.position);
        _crosshairGO = Instantiate(_crosshairPrefab, worldPos, Quaternion.identity);
        CreateRangeCircle();
        _isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging || _crosshairGO == null) return;

        Vector3 worldPos = ScreenToWorld(eventData.position);
        _crosshairGO.transform.position = worldPos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDragging || _crosshairGO == null) return;

        Vector3 worldPos = ScreenToWorld(eventData.position);
       _isDragging = false;
        Explode(worldPos);
        txt_CoolDown.gameObject.SetActive(true);
    }

    private Vector3 ScreenToWorld(Vector2 screenPos)
    {
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y,
                        Mathf.Abs(_mainCamera.transform.position.z)));
        worldPos.z = 0f;
        return worldPos;
    }

    // Vẽ vòng tròn bán kính nổ, làm con của crosshair để di chuyển theo
    private void CreateRangeCircle()
    {
        GameObject circleGO = new GameObject("RangeCircle");
        circleGO.transform.SetParent(_crosshairGO.transform, false);
        circleGO.transform.localPosition = Vector3.zero;

        var line = circleGO.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.loop = true;
        line.positionCount = _circleSegments;
        line.widthMultiplier = 0.05f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = _rangeColor;
        line.endColor = _rangeColor;

        for (int i = 0; i < _circleSegments; i++)
        {
            float angle = 2 * Mathf.PI * i / _circleSegments;
            float x = Mathf.Cos(angle) * _radius;
            float y = Mathf.Sin(angle) * _radius;
            line.SetPosition(i, new Vector3(x, y, 0f));
        }
    }

    private IEnumerator StartAnimatorBomb(GameObject crosshair)
    {
        Animator anim = crosshair.GetComponent<Animator>();
        if (anim == null)
            anim = crosshair.GetComponentInChildren<Animator>();

        if (anim != null)
        {
            anim.SetBool("isExplore", true);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Animator trên crosshair instance!");
        }

        yield return new WaitForSeconds(1f);

        if (anim != null)
            anim.SetBool("isExplore", false);

        Destroy(crosshair);
        if (_crosshairGO == crosshair)
            _crosshairGO = null;
    }
    public void Explode(Vector3 center)
    {
        StartCoroutine(StartAnimatorBomb(_crosshairGO));

        var hits = Physics2D.OverlapCircleAll(center, _radius, LayerMask.GetMask("Enemy"));
        foreach (var hit in hits)
        {
            var zombie = hit.GetComponent<ZombieController>();
            if (zombie != null && !zombie._isDead)
                zombie.TakeDamage(_damage, DamageType.Physical);
        }
        Debug.Log($"Bomb exploded at {center}, hit {hits.Length} zombies");

        _isDragging = false;

        StartCoroutine(CooldownRoutine());
    }

 

    private IEnumerator CooldownRoutine()
    {
        _onCooldown = true;

        if (_buttonCanvasGroup != null)
        {
            _buttonCanvasGroup.alpha = 0.4f;        // làm tối
            _buttonCanvasGroup.interactable = false;
            _buttonCanvasGroup.blocksRaycasts = false; // chặn click
        }

        float remaining = _cooldownTime;
        while (remaining > 0f)
        {
            if (txt_CoolDown != null)
                txt_CoolDown.text = Mathf.CeilToInt(remaining).ToString();

            yield return null;
            remaining -= Time.deltaTime;
        }

        if (txt_CoolDown != null)
            txt_CoolDown.text = "";

        if (_buttonCanvasGroup != null)
        {
            _buttonCanvasGroup.alpha = 1f;
            _buttonCanvasGroup.interactable = true;
            _buttonCanvasGroup.blocksRaycasts = true;
            txt_CoolDown.gameObject.SetActive(false);
        }

        _onCooldown = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}