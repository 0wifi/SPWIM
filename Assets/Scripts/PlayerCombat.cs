using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject mainCamera;

    [SerializeField] private GameObject stabHitbox;
    [SerializeField] private float stabTime;
    [SerializeField] private float stabCooldown;
    [SerializeField] private float dashSpeed;
    private bool canStab = true;
    private bool canDash = true;

    void Start()
    {
        playerInput.actions["Block"].started += ctx => OnBlockStarted();
        playerInput.actions["Block"].canceled += ctx => OnBlockCanceled();
    }

    // Update is called once per frame
    void Update()
    {
        //If the player is on the ground, reset the dash state
        if (playerMovement.IsGrounded == true)
        {
            canDash = true;
        }
    }

    //If the player's stab ability is ready, perform it
    void OnAttack()
    {
        if (canStab == true)
        {
            StartCoroutine(StabAttack());
            canStab = false;
        }
    }

    void OnBlockStarted()
    {

    }

    void OnBlockCanceled()
    {

    }

    void OnBoomerang()
    {

    }


    //Sets the stab hitbox to be active during the specified duration
    IEnumerator StabAttack()
    {
        stabHitbox.SetActive(true);

        //If in the air, have the player "dash" forward. This can only be done ONCE, until the player touches the ground again.
        if (playerMovement.IsGrounded == false && canDash == true)
        {
            rb.AddForce(mainCamera.transform.forward * dashSpeed, ForceMode.Impulse);

            //Cut vertical velocity to prevent vertical movement with the dash
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            canDash = false;
        }

        yield return new WaitForSeconds(stabTime);
        stabHitbox.SetActive(false);

        //After the specified cooldown, the player can stab again
        yield return new WaitForSeconds(stabCooldown);
        canStab = true;
    }
}
