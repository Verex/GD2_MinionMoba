using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

[RequireComponent(
	typeof(NavMeshAgent), 
	typeof(Damageable), 
	typeof(Damager)
	)]
public class Minion : NetworkBehaviour {
	[SerializeField] public Transform target;
	private Damageable damageable;
	private Damager damager;
	private NavMeshAgent navMeshAgent;

	void Start () {
		if (isServer)
		{
			// Get components.
			navMeshAgent = GetComponent<NavMeshAgent>();
			damageable = GetComponent<Damageable>();

			navMeshAgent.SetDestination(target.position);
		}
	}

	public void OnDie(Damageable dmg, Damager dmger)
	{
		if (isServer)
		{

		}
	}
}
