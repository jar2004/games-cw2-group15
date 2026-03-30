using UnityEngine;
using UnityEngine.InputSystem.XR;

public class AnimationStateController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public CharacterController controller;
    public PlayerController movementScript;
    
    [Header("Attack Settings")]
    public float punchCooldown = 0.5f;
    public float kickCooldown = 0.8f;

    [Header("Damage Values")]
    public int punchDamage = 10;
    public int kickDamage = 20;

    [Header("Settings")]
    public float dampening = 0.1f;

    private float nextAttackTime = 0f;
    void Start()
    {
        //controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        //movementScript = GetComponent<PlayerController>();
        //I was trying to figure out why this wasnt working and it was because i hadnt linked the code to the character....
        //Debug.LogError("working");
        //checks because it doesnt seem to work
        if (animator == null) Debug.LogError("Animator not found on child");
        if (movementScript == null) Debug.LogError("PlayerController script not found!");
    }
    void Update()
    {
        AnimationState();
        Combat();
    }
    void Combat()    {
        if (Time.time >= nextAttackTime)
        {
            // Left Click or Ctrl
            if (Input.GetButtonDown("Fire1")) 
            {
                PerformAttack("Punch", punchCooldown);
            }
            // Right Click or Alt
            else if (Input.GetButtonDown("Fire2"))
            {
                PerformAttack("Kick", kickCooldown);
            }
        }
    }
    private void PerformAttack(string triggerName, float cooldown)
    {
        animator.SetTrigger(triggerName);

        nextAttackTime = Time.time + cooldown;

        Debug.Log("Performed " + triggerName + "!");
    }
    void AnimationState()
    {

        Vector3 horizontalVelocity = new(controller.velocity.x, 0, controller.velocity.z);

        float currentSpeed = horizontalVelocity.magnitude;

        //the speed goes to crazy low values like 1e-44 so added to remove that from bugging movment

        if (currentSpeed < 0.1f)

        {

            animator.SetFloat("Speed", 0);

        }

        animator.SetFloat("Speed", currentSpeed, dampening, Time.deltaTime);

        animator.SetBool("isGround", movementScript.IsGrounded);

        if (Input.GetButtonDown("Jump") && movementScript.IsGrounded)
        {
            animator.SetTrigger("Jump");
        }
        //cant use else because it updates too quickly and you loop
        if (!movementScript.IsGrounded)
        {
            animator.SetBool("Jump", false);
        }
        
    }
}