using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathHolder : MonoBehaviour
{
    public Vector3[] GetWaypoints()
    {
        Vector3[] pos = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            pos[i] = transform.GetChild(i).position;
        }
        return pos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }
    }
}