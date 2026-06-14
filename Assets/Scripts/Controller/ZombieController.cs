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

    private float offsetY;

    private bool _isInitialized = false;

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
        _waypoints[0].x ,
        _waypoints[0].y + offsetY,
        _waypoints[0].z
        );

    }
    private void OnEnable()
    {
        _isInitialized = false;
        _currentWaypointIndex = 0;
        offsetY = UnityEngine.Random.Range(-0.5f, 0.5f);

    }
    private void Update()
    {
        if (!_isInitialized) return;
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

        // Waypoint đầu tiên → áp cả 2 offset, giữ nguyên vị trí spawn
        if (_currentWaypointIndex == 0)
        {
            Vector3 target0 = new Vector3(current.x , current.y + offsetY, current.z);
            transform.position = Vector3.MoveTowards(transform.position, target0, _currentSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target0) < 0.05f)
                _currentWaypointIndex++;
            return;
        }

        Vector3 prev = _waypoints[_currentWaypointIndex - 1];
        bool isHorizontal = Mathf.Abs(current.y - prev.y) < 0.01f;

        Vector3 target;
        if (isHorizontal)
            target = new Vector3(current.x, current.y + offsetY, current.z);
        else
            target = new Vector3(current.x, current.y, current.z);

        transform.position = Vector3.MoveTowards(transform.position, target, _currentSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.05f)
            _currentWaypointIndex++;
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
}
