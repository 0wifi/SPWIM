using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    public int Health = 100;

    private GameObject player;
    private NavMeshAgent agent;
    private Rigidbody rb;
    [SerializeField] private GameObject attackHitbox;
    public int AttackDamage;

    [Tooltip("Distance from the player at which the enemy will attempt to attack.")]
    public float TryAttackRange;

    [Tooltip("Time length of an attempted enemy attack")]
    public float AttackWindUptime;
    public float AttackHitboxTime;
    public float AttackCooldown;

    private bool canAttack = true;

    public float KnockbackRecoveryTime = 0.5f;

    [SerializeField] private float enemyHeight;
    [SerializeField] private LayerMask groundMask;
    public bool IsGrounded;

    private Coroutine currentAttackCycle;
    private Coroutine currentStaggerCycle;

    void Start()
    {
        try { player = GameObject.FindWithTag("Player"); }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.LogError("Player object not found. Make sure the player has the tag 'Player'.");
        }
        if (!TryGetComponent(out agent)) Debug.LogError("NavMeshAgent component not found on the enemy.");
        if (!TryGetComponent(out rb)) Debug.LogError("Rigidbody component not found on the enemy.");
    }

    void Update()
    {
        if (agent.enabled)
        {
            agent.SetDestination(player.transform.position);
        }

        if (currentAttackCycle == null) //if not currently in an attack cycle
        {
            if (canAttack && Vector3.Distance(transform.position, player.transform.position) <= TryAttackRange) //if player is in range to attack
            {
                currentAttackCycle = StartCoroutine(AttackPlayer());
            }
        }
    }

    public IEnumerator AttackPlayer()
    {
        //Debug.Log($"<color=green>Enemy attack started</color> {Time.time}");

        if (agent.enabled) agent.isStopped = true;
        canAttack = false;

        yield return new WaitForSeconds(AttackWindUptime);

        attackHitbox.SetActive(true);
        yield return new WaitForSeconds(AttackHitboxTime);
        attackHitbox.SetActive(false);

        if (agent.enabled) agent.isStopped = false;

        //Debug.Log($"<color=red>Enemy attack ended</color> {Time.time}");

        currentAttackCycle = null;
        StartCoroutine(AttackCooldownTimer());
    }

    public IEnumerator AttackCooldownTimer()
    {
        yield return new WaitForSeconds(AttackCooldown);
        canAttack = true;
    }

    public void Hit(int damage)
    {
        // deal damage
        Health -= damage;
        if (Health <= 0)
        {
            Die();
            return;
        }
    }

    public void Hit(int damage, Vector3 knockbackForce)
    {
        // deal damage
        Health -= damage;
        if (Health <= 0)
        {
            Die();
            return;
        }

        //start stagger cycle, if already in stagger throw out old one and begin new cycle 
        if (currentStaggerCycle != null)
        {
            StopCoroutine(currentStaggerCycle);
        }
        currentStaggerCycle = StartCoroutine(StaggerCycle(knockbackForce));
    }

    private IEnumerator StaggerCycle(Vector3 knockbackForce)
    {
        if (currentAttackCycle != null) //CANCEL ATTACK CYCLE
        {
            StopCoroutine(currentAttackCycle);
            currentAttackCycle = null;
            attackHitbox.SetActive(false);
            //Debug.Log($"<color=yellow>Enemy attack canceled</color> {Time.time}");
        }

        if (agent.enabled) //stop movement
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        canAttack = false; //disable attacking

        rb.isKinematic = false; //apply knockback
        rb.AddForce(knockbackForce, ForceMode.Impulse);

        yield return new WaitForSeconds(KnockbackRecoveryTime);

        rb.isKinematic = true; //end knockback

        //restart movement
        agent.enabled = true;
        agent.Warp(transform.position);
        agent.isStopped = false;

        canAttack = true; //enable attacking

        currentStaggerCycle = null; //end stagger cycle
    }

    public void Die()
    {
        StopAllCoroutines();
        //make call to wave spawner
        FindFirstObjectByType<WaveSpawningSystem>().WaveEnemyDied(this);
        Destroy(gameObject);
    }
}
