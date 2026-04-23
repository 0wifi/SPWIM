using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class PlayerCombat : MonoBehaviour
{
    private Camera playerCam;
    private PlayerInput playerInput;
    private PlayerMovement playerMovement;
    private Rigidbody rb;
    [SerializeField] private PlayerHealth playerHealth;

    [SerializeField] private Animator leftArmAnimator;
    [SerializeField] private Animator rightArmAnimator;

    public GameObject BoomerangPrefab;
    public CooldownTimer BoomerangCooldownTimer;
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

    public bool IsBlocking = false;

    [SerializeField] private GameObject shieldObject;
    [SerializeField] private float maxDegreesToBlock;
    [SerializeField] private float shieldHitKnockbackStrength;

    [SerializeField] private GameObject cameraObject;

    private bool shieldBroken = false;

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
            if (rightArmAnimator != null)
                rightArmAnimator.SetTrigger("Stab");

            StartCoroutine(StabAttack());
            canStab = false;

            IsBlocking = false;
            //shieldObject.SetActive(false);

            if (leftArmAnimator != null)
                leftArmAnimator.SetBool("IsBlocking", false);
        }
    }

    void OnBlockStarted()
    {
        if (canStab == true && isBoomerangOut == false && shieldBroken == false)
        {
            IsBlocking = true;
            //shieldObject.SetActive(true);

            if (leftArmAnimator != null)
                leftArmAnimator.SetBool("IsBlocking", true);
                
            playerHealth.StartCoroutine(playerHealth.ShieldRecharge());
        }
    }

    void OnBlockCanceled()
    {
        IsBlocking = false;
        //shieldObject.SetActive(false);

        if (leftArmAnimator != null)
            leftArmAnimator.SetBool("IsBlocking", false);
    }

    public void OnBoomerang()
    {
        if (!isBoomerangOut && !isBoomerangOnCooldown)
        {
            if (leftArmAnimator != null)
                leftArmAnimator.SetTrigger("ThrowShield");

            StartCoroutine(ThrowBoomerang());

            if (leftArmAnimator != null)
                leftArmAnimator.SetBool("IsBlocking", false);

            BoomerangCooldownTimer.Used.Invoke();
        }
    }

    private IEnumerator ThrowBoomerang()
    {
        isBoomerangOut = true;
        IsBlocking = false;
        //shieldObject.SetActive(false);

        yield return new WaitForSeconds(.4f);

        GameObject boomerangInstance = Instantiate(BoomerangPrefab, transform.position, playerCam.transform.rotation);

        //Invoke audio event
        AudioEvents.BoomerangThrown.Invoke(boomerangInstance);
    }
    public void BoomerangReturned()
    {
        isBoomerangOut = false;

        if (leftArmAnimator != null)
            leftArmAnimator.SetTrigger("CatchShield");

        //Invoke audio event
        AudioEvents.BoomerangCaught.Invoke();

        BoomerangCooldownTimer.StartCooldown.Invoke(BoomerangCooldown);
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

        //Invoke audio event
        AudioEvents.PlayerDidAttack.Invoke();

        //If in the air, have the player "dash" forward. This can only be done ONCE, until the player touches the ground again.
        if (playerMovement.IsGrounded == false && canDash == true && playerHealth.IsHealing == false)
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

    private void OnTriggerEnter(Collider other)
    {
        //When the player is hit by an enemy attack
        if (other.gameObject.CompareTag("EnemyAttack"))
        {
            if (other.gameObject.TryGetComponent(out EnemyAttackHitbox hitbox))
            {
                if (hitbox.HasHitPlayerYet) return; //hitbox has already attempted to hit player in this attack instance
                hitbox.HasHitPlayerYet = true; 

                if (IsBlocking == true) //If blocking, check if the hit was in the right angle, and if so no damage is applied
                {
                    //Getting the angular range of the block area
                    Vector3 a = hitbox.enemyController.transform.position - cameraObject.transform.position;
                    Vector3 flatA = new Vector3(a.x, 0, a.z);
                    Vector3 b = cameraObject.transform.forward;
                    Vector3 flatB = new Vector3(b.x, 0, b.z);

                    //Debug.Log(Vector3.Angle(flatA, flatB));

                    if (Vector3.Angle(flatA, flatB) <= maxDegreesToBlock)
{
                        //ATTACK BLOCKED

                        //Invoke audio event
                        AudioEvents.PlayerBlockedHit.Invoke();

                        //apply stagger to enemy
                        Vector3 knockbackDir = (other.gameObject.transform.position - GameObject.FindWithTag("Player").transform.position).normalized;
                        hitbox.enemyController.Hit(0, knockbackDir * shieldHitKnockbackStrength);

                        //The actual damaging
                        playerHealth.PlayerShield -= hitbox.AttackDamage;
                        if (playerHealth.PlayerShield <= 0)
                        {
                            playerHealth.PlayerShield = 0;
                            playerHealth.StartCoroutine(playerHealth.ShieldBreak());
                        }

                        //Shield text update + start the regen process
                        //playerHealth.ShieldDisplay.text = "Shield: " + playerHealth.PlayerShield;
                        UpdateShieldDamage(hitbox);
                        UpdateShieldStatus();
                        playerHealth.StartCoroutine(playerHealth.ShieldRecharge());
                    }
                    else
                    {
                        //BLOCK MISSED -- ATTACK HIT

                        //Invoke audio event
                        AudioEvents.PlayerGotHit.Invoke(); 

                        playerHealth.HealthUpdate(false, hitbox.AttackDamage);
                    }
                }
                else
                {
                    //ATTACK HIT

                    //Invoke audio event
                    AudioEvents.PlayerGotHit.Invoke();

                    playerHealth.HealthUpdate(false, hitbox.AttackDamage);
                }
            }
        }

    }

    public void UpdateShieldStatus()
    {
        playerHealth.ShieldDisplay.text = "Shield: " + playerHealth.PlayerShield;

        //Change shield text based on current status
        if (playerHealth.PlayerShield >= (playerHealth.PlayerShieldMax * 0.75))
        {
            //No damage
            playerHealth.DrDisplay.text = "Damage Reduction: 100%";
            playerMovement.leftArmAnimator.SetFloat("SpinSpeed", 1f);
        }
        else if (playerHealth.PlayerShield < (playerHealth.PlayerShieldMax * 0.75) && playerHealth.PlayerShield >= (playerHealth.PlayerShieldMax * 0.5))
        {
            playerHealth.DrDisplay.text = "Damage Reduction: 75%";
            playerMovement.leftArmAnimator.SetFloat("SpinSpeed", 0.75f);
        }
        else if (playerHealth.PlayerShield < (playerHealth.PlayerShieldMax * 0.5) && playerHealth.PlayerShield >= (playerHealth.PlayerShieldMax * 0.25))
        {
            playerHealth.DrDisplay.text = "Damage Reduction: 50%";
            playerMovement.leftArmAnimator.SetFloat("SpinSpeed", 0.5f);
        }
        else if (playerHealth.PlayerShield < (playerHealth.PlayerShieldMax * 0.25) && playerHealth.PlayerShield > 0)
        {
            playerHealth.DrDisplay.text = "Damage Reduction: 25%";
            playerMovement.leftArmAnimator.SetFloat("SpinSpeed", 0.25f);

            shieldBroken = false;
        }
        else
        {
            playerHealth.DrDisplay.text = "Damage Reduction: BROKEN";
            playerMovement.leftArmAnimator.SetFloat("SpinSpeed", 0f);

            shieldBroken = true;
            OnBlockCanceled();
        }
    }

    private void UpdateShieldDamage(EnemyAttackHitbox hitbox)
    {
        //Damage shield and damage the player based on durability
        if (playerHealth.PlayerShield >= (playerHealth.PlayerShieldMax * 0.75))
        {
            //No damage
        }
        else if (playerHealth.PlayerShield < (playerHealth.PlayerShieldMax * 0.75) && playerHealth.PlayerShield >= (playerHealth.PlayerShieldMax * 0.5))
        {
            playerHealth.HealthUpdate(false, Mathf.Ceil(hitbox.AttackDamage * 0.25f));
        }
        else if (playerHealth.PlayerShield < (playerHealth.PlayerShieldMax * 0.5) && playerHealth.PlayerShield >= (playerHealth.PlayerShieldMax * 0.25))
        {
            playerHealth.HealthUpdate(false, Mathf.Ceil(hitbox.AttackDamage * 0.5f));
        }
        else if (playerHealth.PlayerShield < (playerHealth.PlayerShieldMax * 0.25) && playerHealth.PlayerShield > 0)
        {
            playerHealth.HealthUpdate(false, Mathf.Ceil(hitbox.AttackDamage * 0.75f));
        }
        else
        {
            playerHealth.HealthUpdate(false, hitbox.AttackDamage);
        }
    }

    private void OnDestroy()
    {
        playerInput.actions["Block"].started -= ctx => OnBlockStarted();
        playerInput.actions["Block"].canceled -= ctx => OnBlockCanceled();
    }
}
