using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Damageable))]
public abstract class NetworkUnit : NetworkBehaviour
{
    public Warden warden;
    public int ownerIndex;
    protected Damageable damageable;

    [ClientRpc]
    public void RpcSetTeamMaterial(int mid)
    {
        // Get the warden.
        warden = (Warden)GameObject.FindObjectOfType(typeof(Warden));

        // Set material.
        SetTeamMaterial(mid);
    }

    public abstract void SetTeamMaterial(int mid);

    protected virtual void Start()
    {
        // Get components.
        damageable = GetComponent<Damageable>();
    }
}
