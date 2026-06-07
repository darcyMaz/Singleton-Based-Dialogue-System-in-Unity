using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Input Actions
    private InputSystem_Actions actionSystem;
    private InputAction move;
    private InputAction jump;
    private InputAction dash;

    // RigidBody2D Check
    private bool UseRB = false;
    private Rigidbody2D rb;

    // Movement
    private int direction;
    [SerializeField] float movementSpeed = 6f;
    [SerializeField] float movementAcceleration = 6f;
    private float currentSpeed = 0f;
    [SerializeField] private float minSpeed = 0.05f;

    // Jump
    [SerializeField] float JumpForce = 10f;
    // Ground
    [SerializeField] private Transform feet;
    [SerializeField] private float GroundRaycastDistance = 0.095f;
    [SerializeField] LayerMask GroundLayer;
    [SerializeField] private bool GroundedRay = false;
    private bool JumpHeld;
    private bool Jumping;
    // Coyote Time
    private float CoyoteTimer;
    [SerializeField] float CoyoteTime = 0.25f;
    // Jump Buffering
    private float JumpBufferTimer;
    [SerializeField] float JumpBufferTime = 0.3f;
    private bool JumpBufferBool = false;
    // Fall feel
    private bool JumpFallFeelOnce = false;
    [SerializeField] float JumpFeelCut = 0.5f;
    [SerializeField] float TerminalSpeed = 15f;
    [SerializeField] float FallMultiplier = 2.5f;
    // [SerializeField] float LowJumpMult = 2f;
    private void Awake()
    {
        actionSystem = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        move = actionSystem.Player.Move;
        jump = actionSystem.Player.Jump;
        // dash = actionSystem.Player.Dash;

        move.Enable();
        jump.Enable();
    }
    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        direction = 1;

        if (TryGetComponent(out rb)) { UseRB = true; }

        rb = GetComponent<Rigidbody2D>();
        JumpHeld = false;
        CoyoteTimer = CoyoteTime;
        JumpBufferTimer = JumpBufferTime;
        Jumping = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // If we never found the RigidBody2D then we can't do any movement.
        if (!UseRB) return;

        Move();
        // If Jump is in the Update function, then the IsGrounded check does not work properly.
        Jump();
        // Making the fall feel heavier goes in the FixedUpdate function as it messes with gravity which is part of the games physics.
        HeavyFall();
        
    }

    private void Update()
    {

    }

    private void Move()
    {
        Vector2 movement_ = move.ReadValue<Vector2>();

        // 0 means there's no movement whereas -1 and 1 means there is movement in a direction.
        SetDirection(movement_.x);
        
        float speed_ref = 0f;

        if (movement_.x != 0f) 
            // This happens when we move
            currentSpeed = Mathf.SmoothDamp(currentSpeed, movementSpeed, ref speed_ref, movementAcceleration * Time.fixedDeltaTime);

        else
            // This happens when we let go of move.
            currentSpeed = Mathf.SmoothDamp(currentSpeed, 0, ref speed_ref, movementAcceleration * Time.fixedDeltaTime);

        // As the speed approaches zero on slowdown, set it to zero eventually.
        if (currentSpeed <= minSpeed) currentSpeed = 0f;

        // Translate towards the direction we are moving (or the direction we were previously moving towards) at the assigned speed.
        rb.linearVelocity = new Vector2(direction * currentSpeed, rb.linearVelocity.y);
    }

    private void SetDirection(float movement)
    {
        // If the input is 0 that means we are not moving so keep the direction as is.
        // If the input is 1 or -1 then we are moving and we want to change the way the player sprite is facing.
        float movementDir = RoundDirection(movement);

        // The direction is either set to the key that the player is currently holding or to the last key the player held.
        direction = (movementDir != 0) ? (int) movementDir : direction;
    }
    
    private int RoundDirection(float input)
    {
        // Although I am expecting the movement values to be -1, 0, or 1, this ensures that the behaviour of the sprite is as expected if the values span real numbers from -1 to 1.
        return (input > 0) ? (int) Math.Ceiling(input): (input < 0) ? (int) Math.Floor(input): 0;
    }
    
    private void Jump()
    {
        float jumping = jump.ReadValue<float>();
        bool IsGrounded = isGrounded();

        // If the player is grounded, then:
        //   they are not Jumping and the CoyoteTimer is reset,
        //   if a jump was buffered, a jump is automatically performed,
        //   jump feel bool is set to false.
        if (IsGrounded)
        {
            Jumping = false;
            JumpFallFeelOnce = false;

            
            // If we are Grounded and the JumpBufferBool is true, then it's the first frame that we've landed and the jumpbuffertimer has not reached zero since a buffered jump.
            if (JumpBufferBool) 
            { 
                PerformJump();
                JumpBufferBool = false;

                // Set JumpHeld to be true even if the player is not pressing the jump button on this frame.
                JumpHeld = true;
            }

            CoyoteTimer = CoyoteTime;
            JumpBufferTimer = JumpBufferTime;
        }
        // Otherwise,
        //   the CoyoteTimer ticks down to zero, and if it reaches zero it stays there
        //   the jump buffer timer begins to tick down (it stays at zero unless a jump is buffered in code below).
        else
        {
            CoyoteTimer = (CoyoteTimer > 0f) ? CoyoteTimer - Time.deltaTime : 0f;

            JumpBufferTimer -= Time.deltaTime;
            if (JumpBufferTimer <= 0f) { JumpBufferBool = false; JumpBufferTimer = 0; }
        }

        
        // Jump button activated.
        if (jumping == 1)
        {
            // First frame of jump being pressed.
            if (!JumpHeld && !Jumping && (IsGrounded || CoyoteTimer > 0f))
            {
                PerformJump();
            }
            // Player is jumping and the jump button was not being held down last frame.
            else if (Jumping && !JumpHeld)
            {
                JumpBufferTimer = JumpBufferTime;
                JumpBufferBool = true;
            }

            // This variable is made true at the end of this scope because there is behaviour between
            //      the recognition that we are holding the jump button (setting this bool true) and
            //      the first frame of pressing it.
            JumpHeld = true;

        }
        // Jump button not activated.
        else
        {
            // If the player is Jumping,
            //      and the Jump button was held on the previous frame (but not this one),
            //      and the player is moving up, then the downward velocity is doubled.
            if (Jumping && JumpHeld && rb.linearVelocityY > 0f && !JumpFallFeelOnce )
            {
                // Cut the y velocity
                rb.linearVelocityY *= JumpFeelCut;

                // Also, this can only happen once per jump.
                JumpFallFeelOnce = true;
            }

            JumpHeld = false;
        }

        
    }

    private bool isGrounded()
    {
        if (GroundedRay) Debug.DrawRay(feet.position, Vector2.down * GroundRaycastDistance, Color.red);
        return Physics2D.Raycast(feet.position, Vector2.down, GroundRaycastDistance, GroundLayer);
    }

    private void PerformJump()
    {
        // Add the jump force here.
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpForce);

        // Set the Jumping bool to true.
        Jumping = true;
    }

    private void HeavyFall()
    {
        // If linear velocity in the y direction is negative, add some extra gravity.
        // Make sure it is clamped to a terminal velocity.
        if (rb.linearVelocityY < 0f)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (FallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        
        // I'm removing this because I don't like how it feel :)
        /*
        else if (rb.linearVelocityY > 0f && JumpHeld)
        {
            rb.linearVelocity += Physics2D.gravity.y * (LowJumpMult - 1) * Time.fixedDeltaTime * Vector2.up;
        }
        */

        if (rb.linearVelocityY <= -TerminalSpeed) rb.linearVelocityY = -TerminalSpeed;
    }
}
