using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerMovement : MonoBehaviour
{
    //General variables
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private float moveForce = 85f;
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private Transform orientation;
    [SerializeField] private Rigidbody rb;
    private Vector3 playerMovement;
    private Vector3 moveDir;
    [SerializeField] private PlayerHealth playerHealth;

    //Drag-related variables
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDrag;

    //Jump-related variables
    [SerializeField] private float jumpForce;

    [SerializeField] private Animator leftArmAnimator;
    [SerializeField] private Animator rightArmAnimator;

    private bool canJump;
    public bool IsGrounded;
    public bool IsMoving;


    /// <summary>
    /// The code to be called on initial object creation
    /// </summary>
    void Start()
    {
        //Setting up player inputs
        //playerInput.currentActionMap.Enable();
    }


    /// <summary>
    /// Read movement inputs and causes the player to move
    /// </summary>
    /// <param name="iValue"> The input read </param>
    void OnMove(InputValue iValue)
    {
        Vector2 inputMovementValue = iValue.Get<Vector2>(); //Reads the input value

        //X and Y input are applied to player X and Z
        playerMovement.x = inputMovementValue.x;
        playerMovement.z = inputMovementValue.y;

        IsMoving = inputMovementValue != Vector2.zero;
    }

    /// <summary>
    /// Reads jump input and causes the player to jump
    /// </summary>
    void OnJump()
    {
        if (canJump == true && playerHealth.IsHealing == false)
        {
            //Reset Y velocity
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            //Apply upwards force
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            canJump = false;
        }
    }


    /// <summary>
    /// The code being called every frame
    /// </summary>
    void Update()
    {
        //SpeedControl();

        //Calculates movement direction based on the camera's direction
        moveDir = orientation.forward * playerMovement.z + orientation.right * playerMovement.x;

        //Check for ground using raycasting, using half the player's height plus a little more
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);

        //Drag control (if on the ground, apply the drag; otherwise don't)
        if (IsGrounded == true)
        {
            rb.linearDamping = groundDrag;

            canJump = true; //When on the ground, also reset the jump
        }
        else
        {
            rb.linearDamping = 0;

            canJump = false; //When off the ground, disable jumping
        }

        if (leftArmAnimator != null)
            leftArmAnimator.SetBool("IsWalking", IsMoving);

        if (rightArmAnimator != null)
            rightArmAnimator.SetBool("IsWalking", IsMoving);
    }

    /// <summary>
    /// The code being called every frame, in relation to delta time
    /// </summary>
    private void FixedUpdate()
    {
        if (playerHealth.IsHealing == false)
        {
            //Ground vs. air movement
            if (IsGrounded == true)
            {
                rb.AddForce(moveDir.normalized * moveForce, ForceMode.Force);
            }
            else
            {
                if (moveDir.magnitude < 0.1f)
                    return;//Only do this if theres an input otherwise math go bad

                Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                Vector3 targetAirSpeed = moveDir.normalized * maxSpeed;

                bool belowSpeedCap = flatVelocity.magnitude < maxSpeed;
                bool opposingMovement = Vector3.Dot(moveDir.normalized, flatVelocity.normalized) < 0f;

                if (belowSpeedCap || opposingMovement)
                {
                    // When close to the speed cap, scale force down to avoid overshooting
                    Vector3 velocityDelta = targetAirSpeed - flatVelocity;
                    Vector3 clampedForce = Vector3.ClampMagnitude(velocityDelta, moveForce);

                    rb.AddForce(clampedForce, ForceMode.Force);
                }
            }
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    /// <summary>
    /// Caps movement speed
    /// </summary>
    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); //Current flat velocity

        //Limiting current velocity based on the movement speed as a max
        if (flatVelocity.magnitude > maxSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }
}
