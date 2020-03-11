using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkTransform))]
public class CustomPlayerController : NetworkBehaviour
{
    [Header("Movement Values")]
    public float moveSpeed = 2f;
    public float jumpForce;
    public float gravity;

    [Header("Debug")]
    [SerializeField] float horizontal = 0f;
    [SerializeField] float vertical = 0f;
    [SerializeField] float jumpSpeed = 0f;
    [SerializeField] float rotationSpeed;
    [SerializeField] bool isGrounded = false;
    [SerializeField] bool isJumping = false;
    [SerializeField] bool isDead = false;
    [SerializeField] bool isShooting = false;
    [SerializeField] bool isLeftDir = false;

    float slowerInAirCoef;

    private BoxCollider collider;
    private Animator animator;
    private float rotationTime;


    void Awake()
    {
        collider = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        Camera.main.orthographic = false;
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = new Vector3(15f, 6f, 0f);
        Camera.main.transform.localEulerAngles = new Vector3(0, -90f, 0f);
        Camera.main.transform.SetParent(null);
    }

    void OnCollisionEnter(Collision other)
    {
        isGrounded = true;
        Debug.Log("collider enter");
    }

    void OnCollisionExit(Collision other)
    {
        isGrounded = false;
        Debug.Log("collider exit"); 
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        horizontal = Input.GetAxis("Horizontal");
        slowerInAirCoef = 1; // increase to slow speed in air
        isJumping = false;

        // For testing animations
        if (Input.GetMouseButtonDown(0))
            isShooting = true;
        else if (Input.GetMouseButtonUp(0))
            isShooting = false;
        if (Input.GetKey(KeyCode.E))
            animator.Play("dead");
        //isDead = true;

        jumpSpeed -= Time.deltaTime;
        vertical = Mathf.Lerp(0, jumpForce, jumpSpeed);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (horizontal == 0)
            Debug.Log("0 horizontal");

        rotationTime += Time.deltaTime * rotationSpeed;
        UpdateAnimatorVar();
    }

    void FixedUpdate()
    {
        ApplyGravity();
        if (isDead)
            return;
        RotateDir(horizontal);
        transform.Translate(new Vector3((horizontal * moveSpeed) / slowerInAirCoef, vertical, 0), Space.World);
        Camera.main.transform.Translate(new Vector3((horizontal * moveSpeed) / slowerInAirCoef, 0, 0), Space.World);
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            vertical -= gravity / 10;
            slowerInAirCoef = 1.5f; // increase to slow speed in air
        }
    }

    private void Jump()
    {
        isJumping = true;
        jumpSpeed = jumpForce;
        Debug.Log("Jump !");
    }

    private void RotateDir(float dir)
    {
        Quaternion rightDir = new Quaternion(0, 0.7f, 0, 0.7f);
        Quaternion leftDir = new Quaternion(0, -0.7f, 0, 0.7f);
        if (dir < 0)
        {
            if (isLeftDir == false)
                rotationTime = 0;
            isLeftDir = true;
            transform.rotation = leftDir;
            transform.rotation = Quaternion.Slerp(rightDir, leftDir, rotationTime);
        }

        else if (dir > 0)
        {
            if (isLeftDir == true)
                rotationTime = 0;
            isLeftDir = false;
            transform.rotation = rightDir;
            transform.rotation = Quaternion.Slerp(leftDir, rightDir, rotationTime);
        }
    }

    void GetAimDir()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 aimDir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
    }

    void UpdateAnimatorVar()
    {
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isShooting", isShooting);
        animator.SetFloat("move input", horizontal);
    }
}