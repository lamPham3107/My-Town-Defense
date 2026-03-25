using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ZombieMovement : MonoBehaviour
{
    private int currentWaypointIndex = 0;
    public float speed = 2f;
    private Transform[] wayPoints;
    private float offsetX;
    private float offsetY;
    private void OnEnable()
    {
        wayPoints = WaypointManager.instance.Waypoints;
        currentWaypointIndex = 0;
        offsetX = Random.Range(-0.2f, 0.2f);
        offsetY = Random.Range(-0.2f, 0.2f);

        transform.position = GetOffsetPosition(0);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (currentWaypointIndex >= wayPoints.Length)
        {
            ReturnToPool();
            return;
        }

        Vector3 target = GetOffsetPosition(currentWaypointIndex);

            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                speed * Time.deltaTime
            );
        
        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            currentWaypointIndex++;
        }
    }

    private Vector3 GetOffsetPosition(int index)
    {
        if (index >= wayPoints.Length)
            return transform.position;
        Vector3 pos = wayPoints[index].position;
        // Xác định hướng đi đến waypoint này
        // So sánh với waypoint trước để biết đang đi ngang hay dọc
        if (index == 0)
        {
            // Điểm đầu: áp cả 2 offset
            return new Vector3(pos.x + offsetX, pos.y + offsetY, pos.z);
        }

        Vector3 prevPos = wayPoints[index - 1].position;
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

    void ReturnToPool()
    {
        ZombiesPooling.instance.ReturnPool_Zombie_01(gameObject);
    }
}
