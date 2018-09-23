using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private Mesh viewMesh;
    [SerializeField] public int playerID = 0;

    void OnDrawGizmos()
    {
		Gizmos.color = Color.gray;
        Gizmos.DrawMesh(viewMesh, transform.position, transform.rotation);

        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 5;
        Gizmos.DrawRay(transform.position, direction);
    }
}
