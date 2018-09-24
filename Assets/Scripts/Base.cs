using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Base : NetworkBehaviour
{
	[ClientRpc]
	public void RpcSetTeamMaterial(int mid)
	{
		// Find warden.
		Warden warden = (Warden)GameObject.FindObjectOfType(typeof(Warden));

        Renderer r = transform.GetChild(0).GetChild(0).GetComponent<Renderer>();
		r.material = warden.playerColor[mid];
	}

}
