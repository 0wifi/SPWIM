using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class StabHitbox : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private int damage;
    [SerializeField] private int dashKnockbackStrength;

    [SerializeField] private GameObject damageNumber;
    private DamageNumber dnScript;

    private readonly List<Collider> hitEnemies = new();

    private void OnEnable()
    {
        hitEnemies.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Enemy") && !other.gameObject.CompareTag("OilRig"))
        {
            return;
        } //if hit non-enemy, thog dont care.

        if (other.gameObject.TryGetComponent(out EnemyController enemyController))
        {
            if (hitEnemies.Contains(other)) return; // Skip if this enemy has already been hit by this attack instance
            hitEnemies.Add(other); // Add this enemy to the list of hit enemies

            //apply knockback if player is dashing, otherwise just apply damage
            if (!playerMovement.IsGrounded)
            {
                Vector3 knockbackDir = (other.gameObject.transform.position - GameObject.FindWithTag("Player").transform.position).normalized;
                enemyController.Hit(damage, knockbackDir * dashKnockbackStrength);
            }
            else enemyController.Hit(damage);

            Vector3 newTransform = new Vector3(enemyController.transform.position.x, enemyController.transform.position.y + 2, enemyController.transform.position.z);
            GameObject dn = Instantiate(damageNumber, newTransform, transform.rotation);
            dnScript = dn.GetComponent<DamageNumber>();
            dnScript.UpdateText(damage.ToString());


            playerCombat.OnHitEnemy();
        }
        else if (other.gameObject.TryGetComponent(out OilRigController oilRigController))
        {
            oilRigController.Hit(damage, transform.position + transform.forward * 4.0f);
            
            //note: damage number spawn moved to oilrig hit() to allow additional check if destructible
        }
    }
}
