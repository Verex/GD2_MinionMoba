using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Tower : NetworkAttackUnit
{
    [ClientRpc]
    public override void RpcSetTeamMaterial(int mid)
    {
        // Find the game's warden.
        warden = (Warden)GameObject.FindObjectOfType(typeof(Warden));

        Renderer r = transform.Find("body").GetComponent<Renderer>();

        Material[] materials = r.materials;

        materials[2] = warden.playerColor[mid];

		r.materials = materials;
    }

}
