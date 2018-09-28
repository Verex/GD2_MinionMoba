using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class Minion : NetworkOffenceUnit
{
    [SerializeField] private LayerMask targetSearchMask;
    private Animator animator;

    public Queue<Vector3> path;

    public override void SetTeamMaterial(int mid)
    {
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

    private void FindTarget()
    {
        Collider[] colliders = Physics.OverlapBox(gameObject.transform.position, Vector3.one * 8, Quaternion.identity, targetSearchMask);

        foreach (Collider collider in colliders)
        {
            if (ownerIndex != collider.GetComponent<NetworkUnit>().ownerIndex)
            {
                target = collider.GetComponent<Damageable>();
            }
        }
    }

    private IEnumerator MinionUpdate()
    {
        while (true)
        {
            if (!navMeshAgent.pathPending && navMeshAgent.hasPath && !navMeshAgent.isStopped
            && navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                // Find target.
                if (target == null)
                {
                    FindTarget();

                    // Check if destination needs to be updated.
                    if (navMeshAgent.remainingDistance < 0.5f && path.Count > 0)
                    {
                        // Move to next point in path.
                        netNavAgent.SetDestination(path.Dequeue());
                    }
                }
                else
                {
                    if (!navMeshAgent.isStopped)
                    {
                        // Stop navmesh movement.
                        netNavAgent.SetIsStopped(true);

                        yield return new WaitForSeconds(0.2f);
                    }
                }
            }
            else
            {
                if (target != null)
                {
                    damager.Damage(target);

                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    netNavAgent.SetIsStopped(false);
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
            NetworkServer.Destroy(this.gameObject);
        }
    }

    public void OnTargetKilled(Damageable dmg, Damager dmgr)
    {
        if (isServer)
        {
            target = null;
        }
    }
}
