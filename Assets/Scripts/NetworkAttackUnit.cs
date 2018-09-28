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
                    Damageable dmg = collider.GetComponent<Damageable>();

                    // Add damageables to targets.
                    targets.Add(dmg);

                    // Listen for target's death.
                    dmg.OnDie.AddListener(OnTargetKilled);
                }
            }
        }
    }

    protected override void Start()
    {
        base.Start();

        // Get components.
        damager = GetComponent<Damager>();

        if (isServer)
        {
            // Set up target list.
            targets = new List<Damageable>();
        }
    }

    public virtual void OnTargetKilled(Damageable dmg, Damager dmgr)
    {
        if (isServer)
        {
            // Remove target from our list.
            targets.Remove(dmg);
        }
    }
}
