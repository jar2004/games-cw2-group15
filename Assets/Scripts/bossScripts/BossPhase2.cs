using UnityEngine;
using UnityEngine.AI;

public class BossPhase2 : BossBase
{
    private Transform player;
    private NavMeshAgent agent;
    private GunLogic gunLogic;

    private float aggroRange = 30f;
    private float shootRange = 10f;
    private float fleeRange = 5f;

    public override void enterState(BossManager e)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = e.GetComponent<NavMeshAgent>();
        gunLogic = e.GetComponent<GunLogic>();

        // Speed up in phase 2
        agent.speed = 6f;

        Debug.Log("Boss entered Phase 2");
    }

    public override void updateState(BossManager e)
    {
        float distance = Vector3.Distance(e.transform.position, player.position);

        // Too far — do nothing
        if (distance > aggroRange)
            return;

        // Too close — flee using NavMesh
        if (distance < fleeRange)
        {
            Vector3 fleeDirection = (e.transform.position - player.position).normalized;
            Vector3 fleeTarget = e.transform.position + fleeDirection * 5f;

            // Sample a valid NavMesh position near the flee target
            if (UnityEngine.AI.NavMesh.SamplePosition(fleeTarget, out UnityEngine.AI.NavMeshHit hit, 3f, UnityEngine.AI.NavMesh.AllAreas))
                agent.SetDestination(hit.position);

            FacePlayer(e);
            gunLogic.Shoot();
        }
        // In shoot range — stop and shoot
        else if (distance < shootRange)
        {
            agent.ResetPath();
            FacePlayer(e);
            gunLogic.Shoot();
        }
        // Out of shoot range — chase
        else
        {
            agent.SetDestination(player.position);
        }
    }

    public override void OnCollsionEnter(BossManager e)
    {

    }

    private void FacePlayer(BossManager e)
    {
        Vector3 direction = (player.position - e.transform.position).normalized;
        direction.y = 0;
        e.transform.rotation = Quaternion.LookRotation(direction);
    }
}