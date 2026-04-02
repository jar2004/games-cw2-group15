using UnityEngine;

public class PunchHitbox : MonoBehaviour
{
    private int damage;
    private bool isActive = false;

    void Start()
    {
        GetComponent<Collider>().enabled = false;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public void EnableHitbox()
    {
        isActive = true;
        GetComponent<Collider>().enabled = true;
    }

    public void DisableHitbox()
    {
        isActive = false;
        GetComponent<Collider>().enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        BaseEnemy enemy = other.GetComponentInParent<BaseEnemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            DisableHitbox();
        }
    }
}