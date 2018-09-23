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
public class Minion : NetworkBehaviour
{
    [SerializeField] public Transform target;
    private Damageable damageable;
    private Damager damager;
    private NavMeshAgent navMeshAgent;
    private NetworkedNavAgent navAgent;
	private Animator animator;

    private IEnumerator UpdateAnimation()
    {
        while (true)
        {
			// Update animator.
            animator.SetBool("moving", navAgent.isMoving);

            yield return new WaitForSeconds(0.01f);
        }
    }

    void Start()
    {
        // Get shared components.
        navAgent = GetComponent<NetworkedNavAgent>();
		animator = GetComponent<Animator>();

        if (isServer)
        {
            // Get components.
            navMeshAgent = GetComponent<NavMeshAgent>();
            damageable = GetComponent<Damageable>();

            // Set nav agent destination.
            navAgent.SetDestination(target.position);
        }

		if (isClient)
		{
			StartCoroutine(UpdateAnimation());
		}
    }

    public void OnDie(Damageable dmg, Damager dmger)
    {
        if (isServer)
        {
			
        }
    }
}
