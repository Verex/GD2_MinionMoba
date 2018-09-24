using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Tower : NetworkBehaviour
{
	[SerializeField] public static Material[] playerColor;

    [ClientRpc]
    public void RpcSetTeamMaterial(int mid)
    {
		// Find warden.
		Warden warden = (Warden)GameObject.FindObjectOfType(typeof(Warden));

        Renderer r = transform.Find("body").GetComponent<Renderer>();
        Material[] materials = r.materials;
        materials[2] = warden.playerColor[mid];
		r.materials = materials;
    }

}
