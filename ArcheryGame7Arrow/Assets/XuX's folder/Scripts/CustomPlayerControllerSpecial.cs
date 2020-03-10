using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkTransform))]
public class CustomPlayerControllerSpecial : NetworkBehaviour
{
    [Header("Movement Values")]
    public float moveSpeed = 8f;
    public float jumpForce;
    public float gravity;

    [Header("Debug")]
    [SerializeField] float horizontal = 0f;
    [SerializeField] float vertical = 0f;
    [SerializeField] float jumpSpeed = 0f;
    [SerializeField] bool isGrounded = false;
    [SerializeField] bool isJumping = false;
    [SerializeField] bool isDead = false;
    [SerializeField] bool isShooting = false;
    [SerializeField] bool isLeftDir = false;
    private bool facingRight = true;

    private BoxCollider collider;

    private Animator animator;

    
    
    void Awake()
    {
        collider = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        Camera.main.orthographic = false;
        Camera.main.GetComponent<SmoothCamera2DSpe>().target = this.transform;
        /*Camera.main.transform.localPosition = new Vector3(15f, 6f, 0f);
        Camera.main.transform.localEulerAngles = new Vector3(0, -90f, 0f);*/
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
        // For testing animations
        if (Input.GetMouseButtonDown(0))
            isShooting = true;
        if (Input.GetMouseButtonUp(0))
            isShooting = false;
        if (Input.GetKey(KeyCode.E))
            animator.Play("dead");

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            Jump();

        jumpSpeed -= Time.deltaTime;
        vertical = Mathf.Lerp(0, jumpForce, jumpSpeed);

        if (jumpSpeed < 0)
            isJumping = false;

        horizontal = Input.GetAxis("Horizontal");
        if (!isGrounded)
        {
            horizontal /= 3;
        }

        UpdateAnimatorVar();
        UpdatePlayerLookingDirection();
    }

    void FixedUpdate()
    {
        if (isDead)
            return;
        //RotateDir(horizontal);
        transform.Translate(new Vector3(horizontal * moveSpeed, vertical, 0), Space.World);
        if (!isGrounded)
            transform.Translate(new Vector3(0, -(gravity/10), 0), Space.World);
    }

    private void Jump()
    {
        animator.Play("Jump / idle");
        jumpSpeed = jumpForce;
        Debug.Log("Jump !");
    }

    void UpdateAnimatorVar()
    {
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isShooting", isShooting);
        animator.SetFloat("move input", horizontal);
    }
    
    void UpdatePlayerLookingDirection()
    {
        Vector2 mosePos = Input.mousePosition;
        Vector2 characterScreenPos = Camera.main.WorldToScreenPoint(transform.position);

        float diff = mosePos.x - characterScreenPos.x;
        facingRight = diff >= 0;
        if (facingRight)
            transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        else
            transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
            
        //transform.LookAt(new Vector3(10000 * diff, transform.position.y, 0));
    }
}