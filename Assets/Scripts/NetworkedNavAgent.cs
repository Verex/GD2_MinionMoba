using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NetworkedNavAgent : NetworkBehaviour
{
    private int test;
    private Vector3 targetPosition;
    private NavMeshAgent navMeshAgent;

    private void UpdateDestination(Vector3 currentPosition, Vector3 destination)
    {
        Debug.Log("position set");
        transform.position = currentPosition;
        navMeshAgent.SetDestination(destination);
    }

    [ClientRpc]
    public void RpcUpdateDestination(Vector3 currentPosition, Vector3 destination)
    {
        UpdateDestination(currentPosition, destination);
    }

    [TargetRpc]
    public void TargetUpdateDestination(NetworkConnection target, Vector3 currentPosition, Vector3 destination)
    {
        UpdateDestination(currentPosition, destination);
    }

    [Command]
    public void CmdGetDestination()
    {
        // Update client with current pathing.
        TargetUpdateDestination(connectionToClient, transform.position, navMeshAgent.destination);
    }

    [Server]
    public void SetDestination(Vector3 destination)
    {
        // Apply destination to navmesh agent.
        navMeshAgent.SetDestination(destination);

        // Our destination has been changed.
        SetDirtyBit(1u);
        SetDirtyBit(2u);
    }

    IEnumerator update()
    {
        yield return new WaitForSeconds(5.0f);

        // We want position to be updated.
        SetDirtyBit(2u);
    }

    private void Start()
    {
        // Get components.
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (isServer)
        {
            StartCoroutine(update());
        }
    }
    public override bool OnSerialize(NetworkWriter writer, bool forceAll)
    {
        if (forceAll)
        {
            // Allocate byte array for destination and current position.
            byte[] bytes = new byte[4 * 6];

            // Convert navmesh destination to byte array.
            byte[] dx = System.BitConverter.GetBytes(navMeshAgent.destination.x),
                dy = System.BitConverter.GetBytes(navMeshAgent.destination.y),
                dz = System.BitConverter.GetBytes(navMeshAgent.destination.z),
                px = System.BitConverter.GetBytes(transform.position.x),
                py = System.BitConverter.GetBytes(transform.position.y),
                pz = System.BitConverter.GetBytes(transform.position.z);

            // Push floats to array.
            System.Buffer.BlockCopy(dx, 0, bytes, 0, 4);
            System.Buffer.BlockCopy(dy, 0, bytes, 4, 4);
            System.Buffer.BlockCopy(dz, 0, bytes, 8, 4);
            System.Buffer.BlockCopy(px, 0, bytes, 12, 4);
            System.Buffer.BlockCopy(py, 0, bytes, 16, 4);
            System.Buffer.BlockCopy(pz, 0, bytes, 20, 4);

            // Write destination and position.
            writer.WriteBytesAndSize(bytes, bytes.Length);

            return true;
        }

        bool shouldSync = false;

        // Write sync var.
        writer.WritePackedUInt32(base.syncVarDirtyBits);

        // Check if navmesh destination should be synced.
        if ((base.syncVarDirtyBits & 1u) != 0u)
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

        // Check if current position should be synced.
        if ((base.syncVarDirtyBits & 2u) != 0u)
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

            // Convert bytes to positional data.
            Vector3 destination = new Vector3(
                System.BitConverter.ToSingle(bytes, 0),
                System.BitConverter.ToSingle(bytes, 4),
                System.BitConverter.ToSingle(bytes, 8)
            ),
            position = new Vector3(
                System.BitConverter.ToSingle(bytes, 12),
                System.BitConverter.ToSingle(bytes, 16),
                System.BitConverter.ToSingle(bytes, 20)
            );

            return;
        }

        int num = (int)reader.ReadPackedUInt32();

        // Check for destination set.
        if ((num & 1) != 0)
        {
            byte[] bytes = reader.ReadBytesAndSize();

            Vector3 destination = new Vector3(
                System.BitConverter.ToSingle(bytes, 0),
                System.BitConverter.ToSingle(bytes, 4),
                System.BitConverter.ToSingle(bytes, 8)
            );

            float x = System.BitConverter.ToSingle(bytes, 0),
                y = System.BitConverter.ToSingle(bytes, 4),
                z = System.BitConverter.ToSingle(bytes, 8);

            // Set new destination.
            navMeshAgent.destination = new Vector3(x, y, z);
        }

        // Check for position set.
        if ((num & 2) != 0)
        {
            byte[] bytes = reader.ReadBytesAndSize();

            Vector3 position = new Vector3(
                System.BitConverter.ToSingle(bytes, 0),
                System.BitConverter.ToSingle(bytes, 4),
                System.BitConverter.ToSingle(bytes, 8)
            );

            // Update current position.
            transform.position = position;
        }
    }
}
