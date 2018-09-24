using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    [SerializeField] public PathNode next;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(transform.position, 1.0f);

        if (next != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, next.transform.position);
        }
    }
}
