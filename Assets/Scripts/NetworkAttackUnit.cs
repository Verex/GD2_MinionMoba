using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Damager))]
public abstract class NetworkAttackUnit : NetworkUnit
{
    protected Damager damager;
    protected Damageable target;

    protected override void Start()
    {
        base.Start();

        // Get components.
        damager = GetComponent<Damager>();
    }
}
