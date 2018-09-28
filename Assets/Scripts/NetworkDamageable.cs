using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Damageable))]
public class NetworkDamageable : NetworkBehaviour
{
    [SyncVar] public float currentHealth;
    [SyncVar] public float maxHealth;

    private Damageable damageable;

    private IEnumerator UpdateHealth()
    {
        if (currentHealth != damageable.currentHealth)
        {
            currentHealth = damageable.currentHealth;
        }
        
        if (maxHealth != damageable.maxHealth)
        {
            maxHealth = damageable.maxHealth;
        }

        yield return new WaitForSeconds(0.1f);
    }

    void Start()
    {
        // Get components.
        damageable = GetComponent<Damageable>();

        // Get initial values.
        currentHealth = damageable.currentHealth;
        maxHealth = damageable.maxHealth;

        if (isServer)
        {
            StartCoroutine(UpdateHealth());
        }
    }
}
