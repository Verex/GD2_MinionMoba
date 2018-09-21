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
	private NetworkedNavAgent navAgent;

	private IEnumerator UpdateTargetPosition()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.2f);

			Debug.Log(navAgent.IsMoving);

			navAgent.SetDestination(target.position);
		}
	}

	void Start () {
		if (isServer)
		{
			// Get components.
			navMeshAgent = GetComponent<NavMeshAgent>();
			navAgent = GetComponent<NetworkedNavAgent>();
			damageable = GetComponent<Damageable>();

			navAgent.SetDestination(target.position);

			StartCoroutine(UpdateTargetPosition());
		}
	}

	public void OnDie(Damageable dmg, Damager dmger)
	{
		if (isServer)
		{
			StartCoroutine(UpdateTargetPosition());
		}
	}
}
