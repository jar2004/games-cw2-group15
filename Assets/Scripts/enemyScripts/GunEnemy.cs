using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class GunEnemy : BaseEnemy
{
    [Header("Pistol Settings")]
    public float preferredRange = 10f;
    public float aimDuration = 2f;
    public float retreatDistance = 5f;

    [Header("Laser")]
    private Vector3 lockedTargetPosition;
    public float laserWidth = 0.2f;
    public Color laserAimColor = Color.red;
    public Color laserFireColor = Color.white;

    private LineRenderer laserRenderer;
    public Transform headBone;
    private bool isAiming = false;

    protected override void Start()
    {
        maxHealth = 60f;
        attackDamage = 20f;
        attackRange = 15f;
        attackCooldown = 4f;
        moveSpeed = 3f;
        detectionRange = 20f;

        base.Start();
        SetupLaser();
        // FindHeadBone(); //Manually assign head bone in inspector
    }

    private void SetupLaser()
    {
        laserRenderer = gameObject.AddComponent<LineRenderer>();
        laserRenderer.startWidth = laserWidth;
        laserRenderer.endWidth = laserWidth;
        laserRenderer.material = new Material(Shader.Find("Sprites/Default"));
        laserRenderer.startColor = laserAimColor;
        laserRenderer.endColor = laserAimColor;
        laserRenderer.enabled = false;
        laserRenderer.positionCount = 2;
        laserRenderer.useWorldSpace = true;
    }

    private void FindHeadBone()
    {
        if (headBone != null) return;

        headBone = FindBoneByName(transform, "BodyGuard02_Head");

        if (headBone == null)
        {
            headBone = transform;
            Debug.LogWarning("GunEnemy: Head bone not found");
        }
    }

    private Transform FindBoneByName(Transform root, string name)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>())
        {
            if (t.name == name)
                return t;
        }
        return null;
    }

    protected override void HandleChasing()
    {
        if (isAiming) return;

        float distance = DistanceToPlayer();

        if (distance > detectionRange)
        {
            agent.SetDestination(transform.position);
            animator.SetFloat("Speed", 0f);
            currentState = EnemyState.Idle;
            return;
        }

        if (distance < preferredRange * 0.5f)
        {
            Vector3 retreatDir = (transform.position - player.position).normalized;
            agent.SetDestination(transform.position + retreatDir * retreatDistance);
            agent.speed = moveSpeed;
            animator.SetFloat("Speed", agent.velocity.magnitude);
            return;
        }

        if (distance <= preferredRange)
        {
            agent.SetDestination(transform.position);
            animator.SetFloat("Speed", 0f);
            currentState = EnemyState.Attacking;
            return;
        }

        agent.SetDestination(player.position);
        agent.speed = moveSpeed;
        animator.SetFloat("Speed", agent.velocity.magnitude);
        FacePlayer();
    }

    protected override void HandleAttacking()
    {
        if (isAiming) return;

        float distance = DistanceToPlayer();

        if (distance > preferredRange + 1f)
        {
            currentState = EnemyState.Chasing;
            return;
        }

        if (distance < preferredRange * 0.5f)
        {
            currentState = EnemyState.Chasing;
            return;
        }

        FacePlayer();

        if (CanAttack())
            StartCoroutine(PerformAttack());
    }

    protected override IEnumerator PerformAttack()
    {
        isAiming = true;
        lastAttackTime = Time.time;

        lockedTargetPosition = player.position;

        laserRenderer.enabled = true;

        float elapsed = 0f;

        while (elapsed < aimDuration)
        {
            UpdateLaser();

            float t = elapsed / aimDuration;
            Color currentColor = Color.Lerp(laserAimColor, laserFireColor, t);
            laserRenderer.startColor = currentColor;
            laserRenderer.endColor = currentColor;

            elapsed += Time.deltaTime;
            yield return null;
        }
        // Fire at the locked position
        FireRaycast();
        AudioManager.Instance.PlayGunshot();

        laserRenderer.startColor = laserFireColor;
        laserRenderer.endColor = laserFireColor;
        yield return new WaitForSeconds(0.1f);

        laserRenderer.enabled = false;
        isAiming = false;
    }

    private void UpdateLaser()
    {
        if (headBone == null) return;

        Vector3 origin = headBone.position + Vector3.up * 0.1f;
        Vector3 direction = (lockedTargetPosition - origin).normalized;

        laserRenderer.SetPosition(0, origin);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, attackRange))
            laserRenderer.SetPosition(1, hit.point);
        else
            laserRenderer.SetPosition(1, origin + direction * attackRange);
    }

    private void FireRaycast()
    {
        if (headBone == null) return;

        Vector3 origin = headBone.position + Vector3.up * 0.1f;
        Vector3 direction = (lockedTargetPosition - origin).normalized;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, attackRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                playerUI.TakeDamage((int)attackDamage);
                DamageVignette.Instance.Flash();
                AudioManager.Instance.PlayPlayerHurt();
                Debug.Log("GunEnemy hit player");
            }
        }
    }

    protected override void HandleIdle()
    {
        if (laserRenderer.enabled)
            laserRenderer.enabled = false;

        base.HandleIdle();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }
}