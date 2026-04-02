using UnityEngine;

public class BossManager : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public UnityEngine.AI.NavMeshAgent agent;

    [Header("Health")]
    public float maxHealth = 200f;
    public float currentHealth;
    public bool IsDead { get; private set; } = false;

    [Header("Phases")]
    public BossBase currentState;
    public BossPhase1 phase1 = new BossPhase1();
    public BossPhase2 phase2 = new BossPhase2();

    void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        currentState = phase1;
        currentState.enterState(this);
    }

    void Update()
    {
        if (IsDead) return;
        currentState.updateState(this);
    }

    public void switchState(BossBase state)
    {
        currentState = state;
        state.enterState(this);
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;
        AudioManager.Instance.PlayEnemyHit();

        currentHealth -= damage;
        Debug.Log($"Boss took {damage} damage. Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        IsDead = true;
        agent.enabled = false;
        GetComponent<Collider>().enabled = false;
        Debug.Log("Boss died!");
        Destroy(gameObject, 3f);
    }

    void OnCollisionEnter(Collision collision)
    {

    }
}