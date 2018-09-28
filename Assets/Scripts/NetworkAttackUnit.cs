using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Damager))]
public abstract class NetworkAttackUnit : NetworkUnit
{
    [SerializeField] protected float targetRange = 1.0f;
    [SerializeField] private LayerMask targetSearchMask;
    [SerializeField] protected int maxTargets = 1;

    protected Damager damager;
    protected List<Damageable> targets;
    protected virtual bool shouldFindTargets { get; set; }

    protected override IEnumerator ServerUpdate()
    {
        yield return base.ServerUpdate();

        // Find targets.
        GetAllTargets();
    }

    protected void GetAllTargets()
    {
        if (!shouldFindTargets) return;

        Collider[] colliders = Physics.OverlapBox(gameObject.transform.position,
        Vector3.one * targetRange, Quaternion.identity, targetSearchMask);

        // Loop through all hit colliders.
        foreach (Collider collider in colliders)
        {
            // Check if collider is an enemy unit.
            if (ownerIndex != collider.GetComponent<NetworkUnit>().ownerIndex)
            {
                Damageable target = collider.GetComponent<Damageable>();
                if (!targets.Exists((Damageable t) => t == target))
                {
                    // Add damageables to targets.
                    targets.Add(collider.GetComponent<Damageable>());

                    Debug.Log("target found");
                }
            }
        }
    }

    protected override void Start()
    {
        base.Start();

        // Get components.
        damager = GetComponent<Damager>();

        // Set up target list.
        targets = new List<Damageable>();
    }
}
