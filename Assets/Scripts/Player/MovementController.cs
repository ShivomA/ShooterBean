using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Transform orientation;
    public Transform playerCam;
    public Player player;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerScale = transform.localScale;
        player = GetComponent<Player>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    float horizontal, vertical;
    private void FixedUpdate()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        
        if (Mathf.Abs(horizontal) >= 0.1 || Mathf.Abs(vertical) >= 0.1)
        {
            counterForce = 0.1f;
            Move();
        }
        else counterForce = 2f;
        
        Vector2 speed = FindVelRelativetoLook();
        CounterMovement(speed);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) FindObjectOfType<LevelManager>().LoadScene(0);

        Look();

        if (Input.GetKey(KeyCode.Space) && grounded && !crouching) Jump();
        if (Input.GetKeyDown(KeyCode.LeftControl))
            if (!crouching) StartCrouch();
            else StopCrouch();
    }

    #region Look

    public float mouseSensitivity = 100, sensMultiplier = 1f;
    private float xRotation, yRotation;
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * sensMultiplier * Time.fixedDeltaTime / Time.timeScale;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * sensMultiplier * Time.fixedDeltaTime / Time.timeScale;

        Vector3 rotation = playerCam.transform.localRotation.eulerAngles;
        yRotation = rotation.y + mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        playerCam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
    }
    #endregion


    #region Movement

    float moveForce = 100;
    float counterForce = 0f;
    float forceMultiplier = 10;
    float xForceMultiplier = 10f;
    float zForceMultiplier = 2f;
    float slidingCounterForceMultiplier = 0.2f;

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
    
    private void CounterMovement(Vector2 speed)
    {
        float maxSpeed;
        float mySpeed = Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2));

        if (crouching) maxSpeed = 10;
        else maxSpeed = 30;

        if (mySpeed > maxSpeed)
        {
            float fallSpeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallSpeed, n.z);
        }

        if (crouching)
        {
            Vector3 dragForce = -rb.velocity * moveForce * slidingCounterForceMultiplier * Time.deltaTime;
            rb.AddForce(dragForce / Time.timeScale);
            return;
        }
        
        Vector3 xForce = orientation.transform.right * -speed.x * moveForce * counterForce * Time.deltaTime;
        Vector3 zForce = orientation.transform.forward * -speed.y * moveForce * counterForce * Time.deltaTime;

        rb.AddForce((xForce + zForce) / Time.timeScale);
    }

    private void Move()
    {
        Vector3 yForce = Vector3.zero;

        if (crouching && grounded && canJump) yForce = Vector3.down * Time.deltaTime * slideForce;

        xForceMultiplier = 3;
        zForceMultiplier = 3;

        if (grounded && crouching)
        {
            xForceMultiplier *= 1.75f;
            zForceMultiplier *= 1.75f;
        }
        Vector3 xForce = orientation.transform.right * horizontal * moveForce * Time.deltaTime * forceMultiplier * xForceMultiplier;
        Vector3 zForce = orientation.transform.forward * vertical * moveForce * Time.deltaTime * forceMultiplier * zForceMultiplier;

        rb.AddForce((xForce + yForce + zForce) / Time.timeScale);
    }
    #endregion


    #region Jump

    float jumpForce = 600, jumpCooldown = 1;
    bool canJump = true, grounded = true;
    void Jump()
    {
        if(grounded && canJump)
        {
            canJump = false;
            rb.AddForce(Vector3.up * jumpForce / Time.timeScale);
            rb.AddForce(normalVector * jumpForce / Time.timeScale);
            
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void ResetJump()
    {
        canJump = true;
    }
    #endregion


    #region Crouch

    Vector3 playerScale;
    Vector3 crouchScale = new Vector3(1, 0.75f, 1);
    float slideForce = 500;
    bool crouching = false;

    void StartCrouch()
    {
        crouching = true;
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        transform.localScale = crouchScale;
        transform.position = newPosition;
        if (rb.velocity.magnitude >= 0.5f && grounded)
        {
            Vector3 crouchForce = orientation.transform.forward * slideForce;
            rb.AddForce(crouchForce / Time.timeScale);
        }
    }

    void StopCrouch()
    {
        crouching = false;
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        transform.localScale = playerScale;
        transform.position = newPosition;
    }
    #endregion


    #region Checking Grounded

    public LayerMask groundLayer;
    bool cancelGrounded;
    float maxSlope = 135;
    Vector3 normalVector = Vector3.up;

    private void OnCollisionEnter(Collision collision)
    {
        player.TakeDamage((int)Mathf.Pow(collision.relativeVelocity.magnitude / 50, 3));
    }

    private void OnCollisionStays(Collision collision)
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
    
    void StopGrounded()
    {
        grounded = false;
    }
    
    bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle<maxSlope;
    }
    #endregion

}
