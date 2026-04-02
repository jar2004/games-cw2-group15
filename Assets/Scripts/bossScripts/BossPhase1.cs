using UnityEngine;
using UnityEngine.AI;

public class BossPhase1 : BossBase
{
    private Transform player;
    private NavMeshAgent agent;
    private GunLogic gunLogic;

    private float aggroRange = 30f;
    private float fightRange = 3f;
    private bool phaseSwapped = false;

    public override void enterState(BossManager e)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = e.GetComponent<NavMeshAgent>();
        gunLogic = e.GetComponent<GunLogic>();
        phaseSwapped = false;

        Debug.Log("Boss entered Phase 1");
    }

    public override void updateState(BossManager e)
    {
        // Only switch to phase 2 once, when health drops to 50%
        if (!phaseSwapped && e.currentHealth <= e.maxHealth * 0.5f)
        {
            phaseSwapped = true;
            e.switchState(e.phase2);
            return;
        }

        float distance = Vector3.Distance(e.transform.position, player.position);

        // Too far — do nothing
        if (distance > aggroRange)
            return;

        // In fight range — stop and punch
        if (distance < fightRange)
        {
            agent.ResetPath();
            FacePlayer(e);
            gunLogic.Punch();
        }
        // Out of fight range — chase
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