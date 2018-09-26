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
public class Minion : NetworkOffenceUnit
{
    private Animator animator;

    public Queue<Vector3> path;

    [ClientRpc]
    public override void RpcSetTeamMaterial(int mid)
    {
        // Find the game's warden.
        warden = (Warden)GameObject.FindObjectOfType(typeof(Warden));
        
        Renderer r = transform.GetChild(0).GetComponent<Renderer>();
        r.material = warden.playerMinionMaterial[mid];
    }

    private IEnumerator UpdateAnimation()
    {
        while (true)
        {
            // Update animator.
            animator.SetBool("moving", netNavAgent.isMoving);

            yield return new WaitForSeconds(0.01f);
        }
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
                    netNavAgent.SetDestination(path.Dequeue());
                }
            }

            yield return new WaitForSeconds(0.08f);
        }
    }

    protected override void Start()
    {
        base.Start();

        // Get shared components.
        animator = GetComponent<Animator>();

        if (isServer)
        {
            // Check if we have path.
            if (path != null)
            {
                // Move to first point in path.
                netNavAgent.SetDestination(path.Dequeue());
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
