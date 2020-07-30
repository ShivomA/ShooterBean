using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Transform orientation;
    public Transform playerCam;

    private Rigidbody rb;

    #region Forces
    float slideForce = 500;
    float moveForce = 60;
    float jumpForce = 300;
    float counterForce = 5f;
    #endregion

    #region Multipliers
    float fallingGravityMultiplier = 3, lowJumpMultiplier = 3;
    float forceMultiplier = 10;
    float xForceMultiplier = 10f, yForceMultiplier = 2f;
    float slidingCounterForceMultiplier = 0.2f;
    #endregion

    #region InputVariables
    float x, y;
    bool jumping, crouching;
    bool grounded = false;
    bool canJump = true;
    Vector3 normalVector = Vector3.up;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        playerScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        MyInput();
        Look();
    }

    private void FixedUpdate()
    {
        Move();
    }


    private void MyInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftControl);

        if (Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();
    }
    Vector3 playerScale;
    Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    void StartCrouch()
    {
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        if(rb.velocity.magnitude >= 0.5f && grounded)
        {
            rb.AddForce(orientation.transform.forward * slideForce);
        }
    }
    void StopCrouch()
    {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
    }


    private float mouseSensitivity = 100, sensMultiplier = 1f;
    private float xRotation, yRotation;
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * sensMultiplier * Time.fixedDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * sensMultiplier * Time.fixedDeltaTime;

        Vector3 rotation = playerCam.transform.localRotation.eulerAngles;
        yRotation = rotation.y + mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        playerCam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
    }


    private void Move()
    {
        if (rb.velocity.y < 0)
            rb.AddForce(Vector3.up * Physics.gravity.y * (fallingGravityMultiplier - 1));
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump") && !grounded)
            rb.AddForce(Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1));

        if (jumping) Jump();

        Vector2 speed = FindVelRelativetoLook();
        float xSpeed = speed.x, ySpeed = speed.y;

        CounterMovement(x, y, speed);

        if (crouching && grounded && canJump)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * slideForce);
            return;
        }
        xForceMultiplier = 10;
        yForceMultiplier = 2;
        
        if (!grounded)
        {
            xForceMultiplier *= 0.5f;
            yForceMultiplier *= 0.5f;
        }

        if (grounded && crouching) yForceMultiplier = 0f;

        rb.AddForce(orientation.transform.right * x * moveForce * Time.deltaTime * forceMultiplier * xForceMultiplier);
        rb.AddForce(orientation.transform.forward * y * moveForce * Time.deltaTime * forceMultiplier * yForceMultiplier);
    }


    float jumpCooldown = 1;
    void Jump()
    {
        if(grounded && canJump)
        {
            canJump = false;
            rb.AddForce(Vector3.up * jumpForce);
            rb.AddForce(normalVector * jumpForce);

//            Vector3 vel = rb.velocity;
//            if (rb.velocity.y < 0)
//                rb.velocity = new Vector3(vel.x, 0, vel.z);
//            else if (rb.velocity.y > 0)
//                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    void ResetJump()
    {
        canJump = true;
    }


    private Vector2 FindVelRelativetoLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float speed = rb.velocity.magnitude;
        float ySpeed = speed * Mathf.Cos(u * Mathf.Deg2Rad);
        float xSpeed = speed * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xSpeed, ySpeed);
    }


    //    float threshold = 0;
    float maxSpeed = 40;
    private void CounterMovement(float x, float y, Vector2 speed)
    {
        float mySpeed = Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2));
        if (mySpeed > maxSpeed)
        {
            float fallSpeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallSpeed, n.z);
        }

        if (!grounded || jumping) return;

        if (crouching)
        {
            rb.AddForce(-rb.velocity.normalized * moveForce *slidingCounterForceMultiplier * Time.deltaTime );
            return;
        }

        //        if (Mathf.Abs(speed.x) >= threshold && Mathf.Abs(x) <= 0.1f || (speed.x < -threshold && x > 0) || (speed.x > threshold && x < 0) || true)
        rb.AddForce(orientation.transform.right * -speed.x * moveForce * counterForce * Time.deltaTime);
        //        if (Mathf.Abs(speed.y) >= threshold && Mathf.Abs(y) <= 0.1f || (speed.y < -threshold && y > 0) || (speed.y > threshold && y < 0) || true)
        rb.AddForce(orientation.transform.right * -speed.x * moveForce * counterForce * Time.deltaTime);

    }


    public LayerMask groundLayer;
    bool cancelGrounded;
    float maxSlope = 35;
    private void OnCollisionStay(Collision collision)
    {
        int layer = collision.gameObject.layer;
        if (groundLayer != (groundLayer | (1 << layer))) return;
        
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.contacts[i].normal;
            if (IsFloor(normal))
            {
                grounded = true;
                cancelGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        float delay = 3f;
        if (!cancelGrounded)
        {
            cancelGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }

    }
    bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle<maxSlope;
    }
    private void StopGrounded()
    {
        grounded = false;
    }

}
