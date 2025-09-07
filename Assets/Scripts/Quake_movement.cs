using UnityEngine;
using UnityEngine.InputSystem;

public class Quake_movement : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float jumpForce = 7f;
    public float groundAccelerate = 10f;
    public float airAccelerate = 5f;
    public float airControl = 2.5f;
    public float maxGroundSpeed = 10f;
    public float maxAirSpeed = 8f;

    public float stopSpeed = 2f;
    public float groundFriction = 6f;

    Vector3 camForward;

    public GameObject groundChecker;

    Generalnput playerActions;
    private Vector2 inputDirection = Vector2.zero;
    private Vector3 moveDirection = Vector3.zero;
    private Rigidbody rb;
    private bool jumpPressed = false;

    public enum PlayerState { Idle, Walking, Jumping }
    public PlayerState currentState;

    void Awake()
    {
        playerActions = new Generalnput();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void OnEnable()
    {
        playerActions.Enable();
        playerActions.Player.Movement.performed += OnMovementPerformed;
        playerActions.Player.Movement.canceled += OnMovementCanceled;
        playerActions.Player.Jump.performed += OnJumpPerformed;
    }

    void OnDisable()
    {
        playerActions.Player.Movement.performed -= OnMovementPerformed;
        playerActions.Player.Movement.canceled -= OnMovementCanceled;
        playerActions.Player.Jump.performed -= OnJumpPerformed;
        playerActions.Disable();
    }

    void Update()
    {
        camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        moveDirection = (camForward * inputDirection.y + camRight * inputDirection.x);
        if (moveDirection.sqrMagnitude > 1e-6f)
            moveDirection.Normalize();

        currentState = GetPlayerState();
    }

    void FixedUpdate()
    {
        if (IsGrounded())
            GroundMove();
        else
            AirMove();

        HandleJump();
    }

    private void OnMovementPerformed(InputAction.CallbackContext ctx)
    {
        inputDirection = ctx.ReadValue<Vector2>();
    }

    private void OnMovementCanceled(InputAction.CallbackContext ctx)
    {
        inputDirection = Vector2.zero;
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        jumpPressed = true;
    }

    void GroundMove()
    {
        Vector3 vel = rb.linearVelocity;
        ApplyFriction(ref vel);

        Vector3 wishdir = moveDirection;
        float wishspeed = moveSpeed * Mathf.Clamp01(new Vector2(inputDirection.x, inputDirection.y).magnitude);

        Accelerate(ref vel, wishdir, wishspeed, groundAccelerate);

        Vector3 horiz = new Vector3(vel.x, 0f, vel.z);
        float hspeed = horiz.magnitude;

        if (hspeed > maxGroundSpeed)
        {
            horiz = horiz.normalized * maxGroundSpeed;
            vel.x = horiz.x;
            vel.z = horiz.z;
        }

        rb.linearVelocity = new Vector3(vel.x, rb.linearVelocity.y, vel.z);
    }

    void AirMove()
    {
        Vector3 vel = rb.linearVelocity;
        Vector3 wishdir = moveDirection;

        float wishspeed = moveSpeed * Mathf.Clamp01(new Vector2(inputDirection.x, inputDirection.y).magnitude);
        float cappedWish = Mathf.Min(wishspeed, maxAirSpeed);

        AirAccelerate(ref vel, wishdir, cappedWish, airAccelerate);
        AirControl(ref vel, wishdir, wishspeed);

        rb.linearVelocity = new Vector3(vel.x, rb.linearVelocity.y, vel.z);
    }

    void Accelerate(ref Vector3 velocity, Vector3 wishdir, float wishspeed, float accel)
    {
        if (wishspeed <= 0f || wishdir.sqrMagnitude < 1e-6f) return;

        float currentspeed = Vector3.Dot(velocity, wishdir);
        float addspeed = wishspeed - currentspeed;
        if (addspeed <= 0f) return;

        float accelSpeed = accel * wishspeed * Time.fixedDeltaTime;
        if (accelSpeed > addspeed) accelSpeed = addspeed;

        velocity += wishdir * accelSpeed;
    }

    void AirAccelerate(ref Vector3 velocity, Vector3 wishdir, float wishspeed, float accel)
    {
        if (wishspeed <= 0f || wishdir.sqrMagnitude < 1e-6f) return;

        float currentspeed = Vector3.Dot(velocity, wishdir);
        float addspeed = wishspeed - currentspeed;
        if (addspeed <= 0f) return;

        float accelSpeed = accel * wishspeed * Time.fixedDeltaTime;
        if (accelSpeed > addspeed) accelSpeed = addspeed;

        velocity += wishdir * accelSpeed;
    }

    void AirControl(ref Vector3 velocity, Vector3 wishdir, float wishspeed)
    {
        if (Mathf.Abs(inputDirection.y) < 0.001f || wishdir.sqrMagnitude < 1e-6f || wishspeed <= 0f) return;

        float zspeed = velocity.y;
        velocity.y = 0f;

        float speed = velocity.magnitude;
        if (speed < 0.001f)
        {
            velocity.y = zspeed;
            return;
        }

        velocity.Normalize();
        float dot = Vector3.Dot(velocity, wishdir);
        float k = 32f * airControl * dot * dot * Time.fixedDeltaTime;

        if (dot > 0f)
            velocity = velocity * speed + wishdir * k;

        velocity.Normalize();
        velocity *= speed;
        velocity.y = zspeed;
    }

    void ApplyFriction(ref Vector3 velocity)
    {
        Vector3 horiz = new Vector3(velocity.x, 0f, velocity.z);
        float speed = horiz.magnitude;
        if (speed < 1e-6f) return;

        float control = Mathf.Max(speed, stopSpeed);
        float drop = control * groundFriction * Time.fixedDeltaTime;

        float newSpeed = Mathf.Max(speed - drop, 0f);
        if (newSpeed != speed)
        {
            newSpeed /= speed;
            velocity.x *= newSpeed;
            velocity.z *= newSpeed;
        }
    }

    void HandleJump()
    {
        if (jumpPressed && IsGrounded())
        {
            Vector3 v = rb.linearVelocity;
            v.y = 0f;
            rb.linearVelocity = v;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        jumpPressed = false;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(groundChecker.transform.position, Vector3.down, 0.1f);
    }

    private PlayerState GetPlayerState()
    {
        if (IsGrounded())
        {
            if (moveDirection == Vector3.zero)
                return PlayerState.Idle;
            return PlayerState.Walking;
        }
        else
        {
            return PlayerState.Jumping;
        }
    }
}
