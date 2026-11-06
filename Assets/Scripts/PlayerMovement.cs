using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed; // Speed at which the player moves

    public float groundDrag; // Drag applied to the player when grounded

    public float playerHeight; // Height of the player (used for ground check)
    public LayerMask whatIsGround; // Layer mask to identify the ground for collision detection
    bool grounded; // Flag to check if the player is grounded

    public Transform orientation; // Reference to the player's orientation (used for movement direction)

    float horizontalInput; // Horizontal input value (A/D or Left/Right arrow keys)
    float verticalInput; // Vertical input value (W/S or Up/Down arrow keys)

    Vector3 moveDirection; // Direction in which the player will move

    Rigidbody rb; // Reference to the player's Rigidbody component

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is grounded by casting a ray downwards from the player's position
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        UserInput();
    }

    void FixedUpdate()
    {
        // adding drag to the player when grounded to prevent sliding
        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0f;
        MovePlayer();
    }

    private void UserInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            horizontalInput = 0f;
            verticalInput = 0f;
            return;
        }

        horizontalInput =
            (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1f : 0f) -
            (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1f : 0f);
        verticalInput =
            (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1f : 0f) -
            (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? 1f : 0f);
    }

    private void MovePlayer()
    {
        // Calculate the movement direction based on the player's orientation and input
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Apply the calculated movement direction to the Rigidbody's velocity
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }
    
}
