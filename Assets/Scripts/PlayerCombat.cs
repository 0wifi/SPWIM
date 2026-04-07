using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    private Camera playerCam;
    private PlayerInput playerInput;
    private PlayerMovement playerMovement;
    private Rigidbody rb;

    public GameObject BoomerangPrefab;
    private bool isBoomerangOut = false;
    public float BoomerangCooldown = 2f;
    private bool isBoomerangOnCooldown = false;

    [SerializeField] private GameObject stabHitbox;
    [SerializeField] private float stabTime;
    [SerializeField] private float stabCooldown;
    [SerializeField] private float dashSpeed;
    [Tooltip("% of players velocity retained after hitting an enemy during an air dash")]
    [SerializeField] private float dashHitVelocityModifier = 0.25f;
    private bool canStab = true;
    private bool canDash = true;

    void Start()
    {
        playerCam = GetComponentInChildren<Camera>();
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();

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


    //Sets the stab hitbox to be active during the specified duration
    IEnumerator StabAttack()
    {
        stabHitbox.SetActive(true);

        //If in the air, have the player "dash" forward. This can only be done ONCE, until the player touches the ground again.
        if (playerMovement.IsGrounded == false && canDash == true)
        {
            rb.linearVelocity = playerCam.transform.forward * dashSpeed;

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

    public void OnHitEnemy()
    {
        if (!playerMovement.IsGrounded)
        {
            //cut velocity if in the air
            rb.linearVelocity = rb.linearVelocity * dashHitVelocityModifier;
        }
    }
}
