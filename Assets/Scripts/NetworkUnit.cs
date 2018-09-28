using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Damageable))]
public abstract class NetworkUnit : NetworkBehaviour
{
    [SerializeField] private float clientUpdateDelay = 0.02f;
    [SerializeField] private float serverUpdateDelay = 0.02f;
    public Warden warden;
    public int ownerIndex;
    public Damageable damageable;

    [ClientRpc]
    public void RpcSetTeamMaterial(int mid)
    {
        // Get the warden.
        warden = (Warden)GameObject.FindObjectOfType(typeof(Warden));

        // Set material.
        SetTeamMaterial(mid);
    }

    public abstract void SetTeamMaterial(int mid);
    protected virtual IEnumerator ClientUpdate() { yield return null; }
    protected virtual IEnumerator ServerUpdate() { yield return null; }

    protected virtual IEnumerator ClientUpdateLoop()
    {
        while (true)
        {
            yield return ClientUpdate();
            yield return new WaitForSeconds(clientUpdateDelay);
        }
    }

    protected virtual IEnumerator ServerUpdateLoop()
    {
        while (true)
        {
            yield return ServerUpdate();
            yield return new WaitForSeconds(serverUpdateDelay);
        }
    }

    protected virtual void Start()
    {
        // Get components.
        damageable = GetComponent<Damageable>();

        if (isServer)
        {
            StartCoroutine(ServerUpdateLoop());
        }

        if (isClient)
        {
            StartCoroutine(ClientUpdateLoop());
        }
    }
}
