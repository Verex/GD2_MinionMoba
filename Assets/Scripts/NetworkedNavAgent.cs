using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NetworkedNavAgent : NetworkBehaviour
{
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

        // Send client rpc.
        RpcUpdateDestination(transform.position, destination);
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
            // The first time a GameObject is sent to a client, send all the data (and no dirty bits)
            writer.WritePackedUInt32((uint)10);
            writer.WritePackedUInt32((uint)11);
            writer.Write("String");
            return true;
        }
        bool wroteSyncVar = false;
        if ((base.syncVarDirtyBits & 1u) != 0u)
        {
            if (!wroteSyncVar)
            {
                // Write dirty bits if this is the first SyncVar written
                writer.WritePackedUInt32(base.syncVarDirtyBits);
                wroteSyncVar = true;
            }
            writer.WritePackedUInt32((uint)10);
        }
        if ((base.syncVarDirtyBits & 2u) != 0u)
        {
            if (!wroteSyncVar)
            {
                // Write dirty bits if this is the first SyncVar written
                writer.WritePackedUInt32(base.syncVarDirtyBits);
                wroteSyncVar = true;
            }
            writer.WritePackedUInt32((uint)11);
        }
        if ((base.syncVarDirtyBits & 4u) != 0u)
        {
            if (!wroteSyncVar)
            {
                // Write dirty bits if this is the first SyncVar written
                writer.WritePackedUInt32(base.syncVarDirtyBits);
                wroteSyncVar = true;
            }
            writer.Write("String");
        }

        if (!wroteSyncVar)
        {
            // Write zero dirty bits if no SyncVars were written
            writer.WritePackedUInt32(0);
        }
        return wroteSyncVar;
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        if (initialState)
        {
            int int1 = (int)reader.ReadPackedUInt32();
            int int2 = (int)reader.ReadPackedUInt32();
            string str = reader.ReadString();
			Debug.Log(int1);
            return;
        }
        int num = (int)reader.ReadPackedUInt32();
        if ((num & 1) != 0)
        {
            int int1 = (int)reader.ReadPackedUInt32();
        }
        if ((num & 2) != 0)
        {
            int int2 = (int)reader.ReadPackedUInt32();
        }
        if ((num & 4) != 0)
        {
            string str = reader.ReadString();
        }
    }
}
