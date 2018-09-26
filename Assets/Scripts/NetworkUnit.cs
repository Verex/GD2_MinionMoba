using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Damageable))]
public abstract class NetworkUnit : NetworkBehaviour
{
    public Warden warden;
    protected Damageable damageable;

    [ClientRpc] public abstract void RpcSetTeamMaterial(int mid);

    protected virtual void Start()
    {
        // Get components.
        damageable = GetComponent<Damageable>();
    }
}
