using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StabHitbox : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement; 
    [SerializeField] private int damage;
    [SerializeField] private int knockbackStrength;
    [SerializeField] private int dashKnockbackStrength;
    [SerializeField] private int airKnock;

    private readonly List<Collider> hitEnemies = new();

    private void OnEnable()
    {
        hitEnemies.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (hitEnemies.Contains(other)) return; // Skip if this enemy has already been hit by this attack instance
            hitEnemies.Add(other); // Add this enemy to the list of hit enemies

            if (other.gameObject.TryGetComponent(out EnemyController enemyController))
            {
                Vector3 knockbackDir = (other.gameObject.transform.position - GameObject.FindWithTag("Player").transform.position).normalized;
                knockbackDir.y = airKnock;

                Vector3 knockback = playerMovement.IsGrounded ? knockbackDir * knockbackStrength : knockbackDir * dashKnockbackStrength;

                enemyController.Hit(damage, knockback);
            }
        }
    }
}
