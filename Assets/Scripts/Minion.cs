using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class Minion : NetworkOffenceUnit
{
    public Queue<Vector3> path;

    public override void SetTeamMaterial(int mid)
    {
        Renderer r = transform.GetChild(0).GetComponent<Renderer>();
        r.material = warden.playerMinionMaterial[mid];
    }

    protected override bool shouldFindTargets
    {
        get
        {
            return targets.Count == 0;
        }
    }

    protected override IEnumerator ServerUpdate()
    {
        yield return base.ServerUpdate();

        // Chheck if we're moving along the path.
        if (!navMeshAgent.pathPending && navMeshAgent.hasPath && !navMeshAgent.isStopped
        && navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            // Check for targets.
            if (targets.Count == 0)
            {
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
            if (targets.Count > 0)
            {
                // Damage all targets.
                for (int i = 0; i < Mathf.Min(maxTargets, targets.Count); i++)
                {
                    Debug.Log("Attacked!");
                    damager.Damage(targets[i]);
                }

                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                netNavAgent.SetIsStopped(false);
            }
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
            // Remove target from our list.
            targets.Remove(dmg);
        }
    }
}
