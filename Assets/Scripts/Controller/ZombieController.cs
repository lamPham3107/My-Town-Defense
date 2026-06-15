using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(SpriteRenderer))]
public class ZombieController : MonoBehaviour
{
    // Data
    [SerializeField] private ZombieData _data;
    public ZombieData Data => _data; // cho phep doc gia tri tu ngoai class nhung khong cho phep thay doi gia tri
    // Runtime State
    public float _currentHp { get; private set; }
    public float _currentSpeed { get; private set; }
    public bool _isDead { get; private set; }

    // Paths
    private Vector3[] _waypoints;
    public int _currentWaypointIndex;

    // Effects

    // Events
    public System.Action<ZombieController> OnDeath;
    public System.Action<ZombieController> OnReachEnd;

    // Components
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _hpBarRoot;
    [SerializeField] private SpriteRenderer _hpBarFill;
    private Coroutine _hpBarCoroutine;
    public Animator _zbAnimator;

    private float offsetX;
    private float offsetY;

    private bool _isInitialized = false;
    private bool _facingRight = true; // hướng mặc định

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(ZombieData data, Vector3[] waypoints)
    {
        _data = data;
        _waypoints = waypoints;
        _currentHp = Data.maxHp;
        _currentSpeed = Data.speed;
        _isDead = false;
        _spriteRenderer.sprite = Data.sprite;
        _hpBarRoot.SetActive(false);
        _isInitialized = true;
        transform.position = new Vector3(
        _waypoints[0].x + offsetX,
        _waypoints[0].y + offsetY,
        _waypoints[0].z
        );
        _facingRight = true;          // ← reset hướng
        _spriteRenderer.flipX = false; // ← reset flip

    }
    private void OnEnable()
    {
        _isInitialized = false;
        _currentWaypointIndex = 0;
        offsetX = UnityEngine.Random.Range(-0.2f, 0.2f);
        offsetY = UnityEngine.Random.Range(-0.5f, 0.5f);

    }
    private void Update()
    {
        if (!_isInitialized) return;
        Move();
        UpdateSortingOrder();
    }
    private void Move()
    {
        if (_currentWaypointIndex >= _waypoints.Length)
        {
            ReachEnd();
            return;
        }

        Vector3 current = _waypoints[_currentWaypointIndex];

        if (_currentWaypointIndex == 0)
        {
            Vector3 target0 = new Vector3(current.x + offsetX, current.y + offsetY, current.z);
            transform.position = Vector3.MoveTowards(transform.position, target0, _currentSpeed * Time.deltaTime);

            // Flip dùng waypoint gốc, không dùng target có offset
            FlipSprite(current);

            if (Vector3.Distance(transform.position, target0) < 0.05f)
                _currentWaypointIndex++;
            return;
        }

        Vector3 prev = _waypoints[_currentWaypointIndex - 1];
        bool isHorizontal = Mathf.Abs(current.y - prev.y) < 0.01f;

        Vector3 target;
        if (isHorizontal)
            target = new Vector3(current.x + offsetX, current.y + offsetY, current.z);
        else
            target = new Vector3(current.x, current.y, current.z);

        // Flip dùng waypoint gốc, không dùng target có offset
        FlipSprite(current);

        transform.position = Vector3.MoveTowards(transform.position, target, _currentSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.05f)
            _currentWaypointIndex++;
    }

    private void FlipSprite(Vector3 waypointTarget)
    {
        // Tính hướng từ waypoint trước đến waypoint hiện tại
        float dirX = waypointTarget.x - (_currentWaypointIndex > 0
                     ? _waypoints[_currentWaypointIndex - 1].x
                     : _waypoints[0].x);

        if (Mathf.Abs(dirX) > 0.01f)
        {
            _facingRight = dirX > 0;
            _spriteRenderer.flipX = !_facingRight;
        }
        // Đi dọc → giữ nguyên hướng cũ
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;
        _zbAnimator.SetTrigger("isDied");
        OnDeath?.Invoke(this);

        //PoolManager.instance.ReturnZombie(this);
    }
    public void OnDieAnimationFinished()
    {
        PoolManager.instance.ReturnZombie(this);
    }
    private void ReachEnd()
    {
        if (_isDead) return;
        _isDead = true;
        OnReachEnd?.Invoke(this);
        PoolManager.instance.ReturnZombie(this);
    }

    public void TakeDamage(float amount, DamageType type)
    {
        if (_isDead) return;

        float finalDamage = type switch
        {
            DamageType.Physical => amount * (1 - Data.armor / 100),
            DamageType.Magical => amount * (1 - Data.magicResist / 100),
            _ => amount
        };
        _currentHp = Mathf.Max(0, _currentHp - finalDamage);
        UpdateHpBar();

        if (_currentHp <= 0)
        {
            Die();
        }
    }

    private void UpdateHpBar()
    {
        if (_hpBarFill != null)
        {
            _hpBarFill.transform.localScale = new Vector3(_currentHp / Data.maxHp, 1, 1);
        }
        _hpBarRoot.SetActive(true);
        if (_hpBarCoroutine != null) StopCoroutine(_hpBarCoroutine);
        _hpBarCoroutine = StartCoroutine(HideHpBarAfterDelay(1f));

    }

    private IEnumerator HideHpBarAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _hpBarRoot.SetActive(false);
    }

    private void UpdateSortingOrder()
    {
        int order = Mathf.RoundToInt(-transform.position.y * 100);
        _spriteRenderer.sortingOrder = order;
        // HP bar hiện trên zombie
        if (_hpBarFill != null)
            _hpBarFill.sortingOrder = order + 2;

        // Nếu hpBarRoot có SpriteRenderer
        var hpBarSr = _hpBarRoot.GetComponent<SpriteRenderer>();
        if (hpBarSr != null)
            hpBarSr.sortingOrder = order + 1;
    }
}

