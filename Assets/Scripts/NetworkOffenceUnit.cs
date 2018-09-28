using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(NetworkedNavAgent))]
public abstract class NetworkOffenceUnit : NetworkAttackUnit
{
    protected NavMeshAgent navMeshAgent;
    protected NetworkedNavAgent netNavAgent;
    protected Animator animator;

    protected virtual void UpdateAnimation()
    {
        // Update animator.
        animator.SetBool("moving", netNavAgent.isMoving);
    }

    protected override IEnumerator ClientUpdate()
    {
        yield return base.ClientUpdate();

		// Update our animation to the navmesh.
		UpdateAnimation();
    }

    protected override void Start()
    {
        base.Start();

        // Get components.
        navMeshAgent = GetComponent<NavMeshAgent>();
        netNavAgent = GetComponent<NetworkedNavAgent>();
        animator = GetComponent<Animator>();
    }
}
