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

    protected override IEnumerator ServerUpdate()
    {
        yield return base.ServerUpdate();

        if (targets.Count > 0)
        {
            // Damage all targets.
            for (int i = 0; i < Mathf.Min(maxTargets, targets.Count); i++)
            {
                damager.Damage(targets[i]);
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    public void OnDie(Damageable dmg, Damager dmger)
    {
        if (isServer)
        {
            NetworkServer.Destroy(this.gameObject);
        }
    }

}
