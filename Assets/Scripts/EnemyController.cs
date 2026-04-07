using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public int Health = 100;

    private GameObject player;
    private NavMeshAgent agent;
    private Rigidbody rb;

    [Tooltip("Distance from the player at which the enemy will attempt to attack.")]
    public float TryAttackRange;

    [Tooltip("Time length of an attempted enemy attack")]
    public float AttackTime;

    private bool isAttacking = false;
    private bool canAttack = true;

    public float KnockbackRecoveryTime = 0.5f;

    [SerializeField] private float enemyHeight;
    [SerializeField] private LayerMask groundMask;
    public bool IsGrounded;

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

        if (!isAttacking && Vector3.Distance(transform.position, player.transform.position) <= TryAttackRange)
        {
            StartCoroutine(AttackPlayer());
        }
    }

    public IEnumerator AttackPlayer()
    {
        if (canAttack)
        {
            if (agent.enabled) agent.isStopped = true;
            isAttacking = true;

            yield return new WaitForSeconds(AttackTime);

            if (agent.enabled) agent.isStopped = false;
            isAttacking = false;
        }
    }

    public void Hit(int damage, Vector3 knockbackForce)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
            return;
        }

        StopCoroutine(AttackPlayer());
        isAttacking = false;
        canAttack = false;
        agent.isStopped = true;
        agent.enabled = false;

        rb.isKinematic = false;
        rb.AddForce(knockbackForce, ForceMode.Impulse);
        StartCoroutine(KnockbackRecovery());
    }

    private IEnumerator KnockbackRecovery()
    {
        yield return new WaitForSeconds(KnockbackRecoveryTime);
        rb.isKinematic = true;
        agent.enabled = true;
        agent.Warp(transform.position);
        agent.isStopped = false;
        canAttack = true;
    }

    public void Die()
    {
        //make call to enemy tracker
        Destroy(gameObject);
    }
}
