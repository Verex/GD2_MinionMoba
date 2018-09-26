using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkEntity : NetworkBehaviour
{
    protected Damageable damageable;

    void Start()
    {
        // Get components.
        damageable = GetComponent<Damageable>();
    }
}
