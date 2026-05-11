using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectTileController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private TowerController _owner;
    [SerializeField] private ZombieController _target;

    public void Init(TowerController owner , ZombieController target)
    {
        _owner = owner;
        _target = target;
    }

    private void Update()
    {
        if (_target == null || _target._isDead)
        {
            PoolManager.instance.ReturnProjectTile(this);
            return;
        }
        Vector3 dir = (_target.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        // Check va cham
        if (Vector3.Distance(transform.position, _target.transform.position) < 0.1f)
        {
            OnHit();
        }
    }

    private void OnHit()
    {
        
    }
}
