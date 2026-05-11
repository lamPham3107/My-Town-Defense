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


    private float offsetX;
    private float offsetY;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(ZombieData data , Vector3[] waypoints)
    {
        _data = data;
        _waypoints = waypoints;
        _currentHp = Data.maxHp;
        _currentSpeed = Data.speed;
        _isDead = false;

        _spriteRenderer.sprite = Data.sprite;

    }
    private void OnEnable()
    {
        _currentWaypointIndex = 0;
        offsetX = UnityEngine.Random.Range(-0.2f, 0.2f);
        offsetY = UnityEngine.Random.Range(-0.2f, 0.2f);

    }
    private void Start()
    {
        
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
        Vector3 target = GetOffsetPosition(_currentWaypointIndex);

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
    private Vector3 GetOffsetPosition(int index)
    {
        if (index >= _waypoints.Length)
            return transform.position;
        Vector3 pos = _waypoints[index];
        // Xác định hướng đi đến waypoint này
        // So sánh với waypoint trước để biết đang đi ngang hay dọc
        if (index == 0)
        {
            // Điểm đầu: áp cả 2 offset
            return new Vector3(pos.x + offsetX, pos.y + offsetY, pos.z);
        }

        Vector3 prevPos = _waypoints[index - 1];
        bool isMovingHorizontal = Mathf.Abs(pos.y - prevPos.y) < 0.01f;

        if (isMovingHorizontal)
        {
            // Đi ngang → giữ offsetY, không lệch X tại điểm rẽ
            return new Vector3(pos.x, pos.y + offsetY, pos.z);
        }
        else
        {
            // Đi dọc → giữ offsetX, không lệch Y tại điểm rẽ
            return new Vector3(pos.x + offsetX, pos.y, pos.z);
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
        if(_isDead) return;
        _isDead = true;
        OnReachEnd?.Invoke(this);
        PoolManager.instance.ReturnZombie(this);
    }

    public void TakeDamage(float amount , DamageType type)
    {
        if (_isDead) return;

        float finalDamage = type switch
        {
            DamageType.Physical => amount * (1 - Data.armor / 100),
            DamageType.Magical => amount * (1 - Data.magicResist / 100),
            _ => amount
        };
        _currentHp = Mathf.Max(0, _currentHp - finalDamage);


        if (_currentHp <= 0)
        {
            Die();
        }
    }
}
