using UnityEngine;
using UnityEngine.AI;

public class StabHitbox : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement; 
    [SerializeField] private int damage;
    [SerializeField] private int knockback;
    [SerializeField] private int dashKnockback;
    [SerializeField] private int airKnock;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (other.gameObject.GetComponent<HealthAndDamage>() != null)
            {
                other.gameObject.GetComponent<HealthAndDamage>().Health -= damage;

                //Temporarily disables the NavMesh so the knockback can be applied.
                other.gameObject.GetComponent<NavMeshAgent>().enabled = false;

                Vector3 knockbackDir = (other.gameObject.transform.position - gameObject.transform.position).normalized;
                knockbackDir.y = airKnock;
                var rb = other.gameObject.GetComponent<Rigidbody>();

                //Different knockback applied for if the player stabbed the enemy versus dash stabbed
                if (playerMovement.IsGrounded == true)
                {
                    rb.AddForce(knockbackDir * knockback, ForceMode.VelocityChange);
                }
                else
                {
                    rb.AddForce(knockbackDir * knockback, ForceMode.VelocityChange);
                }
            }
        }
    }
}
