using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(NetworkedNavAgent))]
public abstract class NetworkOffenceUnit : NetworkAttackUnit
{
	protected NavMeshAgent navMeshAgent;
	protected NetworkedNavAgent netNavAgent;

	protected override void Start()
	{
		base.Start();

		// Get components.
		navMeshAgent = GetComponent<NavMeshAgent>();
		netNavAgent = GetComponent<NetworkedNavAgent>();
	}
}
