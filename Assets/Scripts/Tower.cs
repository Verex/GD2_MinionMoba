using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Tower : NetworkAttackUnit
{
    public override void SetTeamMaterial(int mid)
    {
        // Get renderer component.
        Renderer renderer = transform.Find("body").GetComponent<Renderer>();

        // Get the current materials.
        Material[] materials = renderer.materials;

        // Assign material for glow.
        materials[2] = warden.playerColor[mid];

        // Update renderer materials.
        renderer.materials = materials;
    }

    public void OnDie(Damageable dmg, Damager dmger)
    {
        if (isServer)
        {
            NetworkServer.Destroy(this.gameObject);
        }
    }

}
