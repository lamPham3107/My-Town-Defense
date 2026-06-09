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

    private float offsetX;
    private float offsetY;

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

        transform.position = new Vector3(
        _waypoints[0].x + offsetX,
        _waypoints[0].y + offsetY,
        _waypoints[0].z
        );

    }
    private void OnEnable()
    {
        _currentWaypointIndex = 0;
        offsetX = UnityEngine.Random.Range(-0.5f, 0.5f);
        offsetY = UnityEngine.Random.Range(-0.5f, 0.5f);

    }
    private void Update()
    {
        Move();
    }
    private void Move()
    {
        if (_currentWaypointIndex >= _waypoints.Length)
        {
            ReachEnd();
            return;
        }

        Vector3 current = _waypoints[_currentWaypointIndex];
        Vector3 prev = _currentWaypointIndex > 0
            ? _waypoints[_currentWaypointIndex - 1]
            : current;

        bool isHorizontal = Mathf.Abs(current.y - prev.y) < 0.01f;

        // Target giữ đúng lane offset của zombie này
        Vector3 target;
        if (isHorizontal)
        {
            // Đi ngang → chỉ quan tâm X của waypoint, Y giữ nguyên lane
            target = new Vector3(current.x, current.y + offsetY, current.z);
        }
        else
        {
            // Đi dọc → chỉ quan tâm Y của waypoint, X giữ nguyên lane
            target = new Vector3(current.x + offsetX, current.y, current.z);
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            _currentSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            _currentWaypointIndex++;
        }
    }


    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        OnDeath?.Invoke(this);
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
}
