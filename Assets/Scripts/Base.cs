using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Base : NetworkUnit
{
    public override void SetTeamMaterial(int mid)
    {
        // Find warden.
        Warden warden = (Warden)GameObject.FindObjectOfType(typeof(Warden));

        Renderer r = transform.GetChild(0).GetChild(0).GetComponent<Renderer>();
        r.material = warden.playerColor[mid];
    }

    public void OnDie(Damageable dmg, Damager dmger)
    {
        if (isServer)
        {
            NetworkServer.Destroy(this.gameObject);
        }
    }
}
