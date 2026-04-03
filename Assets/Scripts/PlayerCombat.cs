using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public PlayerInput playerInput;
    private Camera playerCam;

    public GameObject BoomerangPrefab;
    private bool isBoomerangOut = false;
    public float BoomerangCooldown = 2f;
    private bool isBoomerangOnCooldown = false;

    void Start()
    {
        playerCam = GetComponentInChildren<Camera>();
        playerInput.actions["Block"].started += ctx => OnBlockStarted();
        playerInput.actions["Block"].canceled += ctx => OnBlockCanceled();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnAttack()
    {

    }

    void OnBlockStarted()
    {

    }

    void OnBlockCanceled()
    {

    }

    public void OnBoomerang()
    {
        if (!isBoomerangOut && !isBoomerangOnCooldown)
        {
            Instantiate(BoomerangPrefab, transform.position, playerCam.transform.rotation);
            isBoomerangOut = true;
        }
    }
    public void BoomerangReturned()
    {
        isBoomerangOut = false;
        StartCoroutine(DoBoomerangCooldown());
    }
    private IEnumerator DoBoomerangCooldown()
    {
        isBoomerangOnCooldown = true;
        yield return new WaitForSeconds(BoomerangCooldown); // example cooldown duration
        isBoomerangOnCooldown = false;
    }

}
