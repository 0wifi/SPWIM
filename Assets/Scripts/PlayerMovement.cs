using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //General variables
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform orientation;
    [SerializeField] private Rigidbody rb;
    private Vector3 playerMovement;
    private Vector3 moveDir;

    //Drag-related variables
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDrag;

    //Jump-related variables
    [SerializeField] private float jumpForce;
    [SerializeField] private float airMultiplier;

    private bool canJump;
    public bool IsGrounded;


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
    }

    /// <summary>
    /// Reads jump input and causes the player to jump
    /// </summary>
    void OnJump()
    {
        if (canJump == true)
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
        SpeedControl();

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
    }

    /// <summary>
    /// The code being called every frame, in relation to delta time
    /// </summary>
    private void FixedUpdate()
    {
        //Ground vs. air movement
        if (IsGrounded == true)
        {
            rb.AddForce(moveDir.normalized * moveSpeed, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDir.normalized * moveSpeed * airMultiplier, ForceMode.Force);
        }
    }

    /// <summary>
    /// Caps movement speed
    /// </summary>
    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); //Current flat velocity

        //Limiting current velocity based on the movement speed as a max
        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }
}
