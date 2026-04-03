using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class BoomerangController : MonoBehaviour
{
    public float ThrowSpeed = 10f;
    public float ReturnSpeed = 10f;
    public float MaxTime = 3f;

    private PlayerCombat playerCombat;
    private bool returning = false;

    [Tooltip("layers that will trigger the boomerang to return to the player")]
    public LayerMask HitStopLayerMask;


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
            playerCombat.BoomerangReturned();
            Destroy(gameObject);
        }
    }

    private IEnumerator ReturnAfterTime(float maxTime)
    {
        yield return new WaitForSeconds(maxTime);
        returning = true;
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
}
