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
public class Minion : NetworkAttackUnit
{
    [SerializeField] public Transform target;
    private Damageable damageable;
    private Damager damager;
    private NavMeshAgent navMeshAgent;
    private NetworkedNavAgent navAgent;
    private Animator animator;
    public Queue<Vector3> path;

    [ClientRpc]
    public void RpcSetTeamMaterial(int mid)
    {
        // Find warden.
        Warden warden = (Warden)GameObject.FindObjectOfType(typeof(Warden));

        Renderer r = transform.GetChild(0).GetComponent<Renderer>();
        r.material = warden.playerMinionMaterial[mid];
    }

    private IEnumerator UpdateAnimation()
    {
        while (true)
        {
            // Update animator.
            animator.SetBool("moving", navAgent.isMoving);

            yield return new WaitForSeconds(0.01f);
        }
    }

    private void FindTarget()
    {

    }

    private IEnumerator MinionUpdate()
    {
        while (true)
        {
            if (!navMeshAgent.pathPending && navMeshAgent.hasPath && !navMeshAgent.isStopped)
            {
                if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete
                && navMeshAgent.remainingDistance < 0.5f && path.Count > 0)
                {
                    // Move to next point in path.
                    navAgent.SetDestination(path.Dequeue());
                }
            }

            yield return new WaitForSeconds(0.08f);
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

            // Check if we have path.
            if (path != null)
            {
                // Move to first point in path.
                navAgent.SetDestination(path.Dequeue());
            }

            StartCoroutine(MinionUpdate());
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
