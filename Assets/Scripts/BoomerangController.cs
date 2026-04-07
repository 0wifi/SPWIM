using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;

public class BoomerangController : MonoBehaviour
{
    public int Damage = 20;

    public float ThrowSpeed = 10f;
    public float ReturnSpeed = 10f;
    public float MaxTime = 3f;

    private PlayerCombat playerCombat;
    private bool returning = false;

    [Tooltip("layers that will trigger the boomerang to return to the player")]
    public LayerMask HitStopLayerMask;

    private readonly List<Collider> hitColliders = new();

    private void Awake()
    {
        playerCombat = FindFirstObjectByType<PlayerCombat>();
        StartCoroutine(ReturnAfterTime(MaxTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        // if collided object's layer is in the HitStopLayerMask
        if ((HitStopLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            returning = true;
        }

        if (returning && other.gameObject.CompareTag("Player"))
        {
            ReturnedToPlayer();
        }

        if(other.gameObject.CompareTag("Enemy"))
        {
            if (hitColliders.Contains(other)) return;
            hitColliders.Add(other);

            StartCoroutine(EnemyHitCD(other)); // start cooldown for hitting this enemy again

            other.GetComponent<EnemyController>().Hit(20, Vector3.zero);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (returning && other.gameObject.CompareTag("Player"))
        {
            ReturnedToPlayer();
        }
    }

    private IEnumerator ReturnAfterTime(float maxTime)
    {
        yield return new WaitForSeconds(maxTime);
        returning = true;
    }
    private IEnumerator EnemyHitCD(Collider col)
    {
        yield return new WaitForSeconds(0.5f); // cooldown time for hitting the same enemy again
        hitColliders.Remove(col); // remove the collider from the list after cooldown
    }

    private void Update()
    {
        switch (returning)
        {
            case false:
                transform.Translate(ThrowSpeed * Time.deltaTime * transform.forward, Space.World);
                break;
            case true:
                Vector3 directionToPlayer = (playerCombat.transform.position - transform.position).normalized;
                transform.Translate(ReturnSpeed * Time.deltaTime * directionToPlayer, Space.World);
                break;
        }
    }

    private void ReturnedToPlayer()
    {
        playerCombat.BoomerangReturned();
        Destroy(gameObject);
    }
}
