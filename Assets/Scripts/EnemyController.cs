using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private GameObject player;
    private NavMeshAgent agent;

    [Tooltip("Distance from the player at which the enemy will attempt to attack.")]
    public float TryAttackRange;

    [Tooltip("Time length of an attempted enemy attack")]
    public float AttackTime;

    private bool isAttacking = false;

    void Start()
    {
        try { player = GameObject.FindWithTag("Player"); } 
        catch(Exception e) {
            Debug.LogException(e);
            Debug.LogError("Player object not found. Make sure the player has the tag 'Player'.");
        }
        if (!TryGetComponent(out agent)) {
            Debug.LogError("NavMeshAgent component not found on the enemy.");
        } 
    }

    void Update()
    {
        agent.SetDestination(player.transform.position);

        if (!isAttacking && Vector3.Distance(transform.position, player.transform.position) <= TryAttackRange)
        {
            StartCoroutine(AttackPlayer());
            //Debug.Log("Enemy is trying to attack the player!");
        }
    }

    public IEnumerator AttackPlayer()
    {
        agent.isStopped = true; isAttacking = true;
        yield return new WaitForSeconds(AttackTime);
        agent.isStopped = false; isAttacking = false;
    }
}
