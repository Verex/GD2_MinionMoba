using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NetworkedNavAgent : NetworkBehaviour
{
    [SerializeField] private float minimumSyncDistance = 5.0f;
    private int test;
    private Vector3 targetPosition;
    private NavMeshAgent navMeshAgent;

    public bool isMoving
    {
        get
        {
            if (navMeshAgent.hasPath && !navMeshAgent.isStopped)
            {
                if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && navMeshAgent.remainingDistance != 0)
                {
                    return true;
                }
            }

            return false;
        }
    }

    [Server]
    public void SetDestination(Vector3 destination)
    {
        // Apply destination to navmesh agent.
        navMeshAgent.destination = destination;

        // Our destination has been changed.
        SetDirtyBit(1u);
        SetDirtyBit(2u);
    }

    private void Start()
    {
        // Get components.
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public override bool OnSerialize(NetworkWriter writer, bool forceAll)
    {
        if (forceAll)
        {
            if (navMeshAgent == null)
            {
                navMeshAgent = GetComponent<NavMeshAgent>();
            }

            // Allocate byte array for destination and current position.
            byte[] bytes = new byte[1 + (4 * 6)];

            // Convert navmesh destination to byte array.
            byte[] dx = System.BitConverter.GetBytes(navMeshAgent.destination.x),
                dy = System.BitConverter.GetBytes(navMeshAgent.destination.y),
                dz = System.BitConverter.GetBytes(navMeshAgent.destination.z),
                px = System.BitConverter.GetBytes(transform.position.x),
                py = System.BitConverter.GetBytes(transform.position.y),
                pz = System.BitConverter.GetBytes(transform.position.z),
                st = System.BitConverter.GetBytes(navMeshAgent.isStopped);

            // Push floats to array.
            System.Buffer.BlockCopy(st, 0, bytes, 0, 1);
            System.Buffer.BlockCopy(dx, 0, bytes, 1, 4);
            System.Buffer.BlockCopy(dy, 0, bytes, 5, 4);
            System.Buffer.BlockCopy(dz, 0, bytes, 9, 4);
            System.Buffer.BlockCopy(px, 0, bytes, 13, 4);
            System.Buffer.BlockCopy(py, 0, bytes, 17, 4);
            System.Buffer.BlockCopy(pz, 0, bytes, 21, 4);

            // Write destination and position.
            writer.WriteBytesAndSize(bytes, bytes.Length);

            return true;
        }

        bool shouldSync = false;

        // Write sync var.
        writer.WritePackedUInt32(base.syncVarDirtyBits);

        // Check if current position should be synced.
        if ((base.syncVarDirtyBits & 1u) != 0u)
        {
            // Convert navmesh destination to byte array.
            byte[] x = System.BitConverter.GetBytes(transform.position.x),
                y = System.BitConverter.GetBytes(transform.position.y),
                z = System.BitConverter.GetBytes(transform.position.z);

            byte[] bytes = new byte[4 * 3];
            System.Buffer.BlockCopy(x, 0, bytes, 0, 4);
            System.Buffer.BlockCopy(y, 0, bytes, 4, 4);
            System.Buffer.BlockCopy(z, 0, bytes, 8, 4);

            // Write position vector to stream.
            writer.WriteBytesAndSize(bytes, bytes.Length);

            shouldSync = true;
        }

        // Check if navmesh destination should be synced.
        if ((base.syncVarDirtyBits & 2u) != 0u)
        {
            byte[] bytes = new byte[4 * 3];

            // Convert navmesh destination to byte array.
            byte[] x = System.BitConverter.GetBytes(navMeshAgent.destination.x),
                y = System.BitConverter.GetBytes(navMeshAgent.destination.y),
                z = System.BitConverter.GetBytes(navMeshAgent.destination.z);

            System.Buffer.BlockCopy(x, 0, bytes, 0, 4);
            System.Buffer.BlockCopy(y, 0, bytes, 4, 4);
            System.Buffer.BlockCopy(z, 0, bytes, 8, 4);

            // Write position vector to stream.
            writer.WriteBytesAndSize(bytes, bytes.Length);

            shouldSync = true;
        }

        // Check if navmesh destination should be synced.
        if ((base.syncVarDirtyBits & 3u) != 0u)
        {
            byte[] bytes = System.BitConverter.GetBytes(navMeshAgent.isStopped);

            // Write position vector to stream.
            writer.WriteBytesAndSize(bytes, bytes.Length);

            shouldSync = true;
        }

        if (!shouldSync)
        {
            writer.WritePackedUInt32(0);
        }

        return shouldSync;
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        if (initialState)
        {
            // Read destination and position data.
            byte[] bytes = reader.ReadBytesAndSize();

            bool isStopped = System.BitConverter.ToBoolean(bytes, 0);

            // Convert bytes to positional data.
            Vector3 destination = new Vector3(
                System.BitConverter.ToSingle(bytes, 1),
                System.BitConverter.ToSingle(bytes, 5),
                System.BitConverter.ToSingle(bytes, 9)
            ),
            position = new Vector3(
                System.BitConverter.ToSingle(bytes, 13),
                System.BitConverter.ToSingle(bytes, 17),
                System.BitConverter.ToSingle(bytes, 21)
            );

            if (navMeshAgent == null)
            {
                navMeshAgent = GetComponent<NavMeshAgent>();
            }

            navMeshAgent.Warp(position);
            navMeshAgent.SetDestination(destination);
            navMeshAgent.isStopped = isStopped;

            return;
        }

        int num = (int)reader.ReadPackedUInt32();

        // Check for position set.
        if ((num & 1) != 0)
        {
            byte[] bytes = reader.ReadBytesAndSize();

            Vector3 position = new Vector3(
                System.BitConverter.ToSingle(bytes, 0),
                System.BitConverter.ToSingle(bytes, 4),
                System.BitConverter.ToSingle(bytes, 8)
            );

            if (Vector3.Distance(transform.position, position) >= minimumSyncDistance)
            {
                // Update current position.
                navMeshAgent.Warp(position);
            }
        }

        // Check for destination set.
        if ((num & 2) != 0)
        {
            byte[] bytes = reader.ReadBytesAndSize();

            Vector3 destination = new Vector3(
                System.BitConverter.ToSingle(bytes, 0),
                System.BitConverter.ToSingle(bytes, 4),
                System.BitConverter.ToSingle(bytes, 8)
            );

            // Assign new destination.
            navMeshAgent.SetDestination(destination);
        }

        // Check for isStopped set.
        if ((num & 3) != 0)
        {
            byte[] bytes = reader.ReadBytesAndSize();

            bool isStopped = System.BitConverter.ToBoolean(bytes, 0);

            // Assign new destination.
            navMeshAgent.isStopped = isStopped;
        }
    }
}
