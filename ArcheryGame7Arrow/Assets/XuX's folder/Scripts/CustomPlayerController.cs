using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkTransform))]
public class CustomPlayerController : NetworkBehaviour
{
    [Header("Movement Values")]
    public float moveSpeed = 8f;
    public float jumpForce;
    public float gravity;
    //public float turnSensitivity = 5f;
    //public float maxTurnSpeed = 150f;

    [Header("Debug")]
    [SerializeField] float horizontal = 0f;
    [SerializeField] float vertical = 0f;
    [SerializeField] float jumpSpeed = 0f;
    [SerializeField] bool isGrounded = false;
    [SerializeField] bool isJumping = false;
    [SerializeField] bool isDead = false;
    [SerializeField] bool isShooting = false;

    private BoxCollider collider;

    private Animator animator;
    //public float turn = 0f;
    //public CharacterController characterController;

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
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (isDead)
            return;

        horizontal = Input.GetAxis("Horizontal");
        // For Debug/ test animations
        if (Input.GetMouseButtonDown(0))
            isShooting = true;
        if (Input.GetKey(KeyCode.E))
            isDead = true;

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            jumpSpeed = jumpForce;
            isJumping = true;
        }

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
    }

    void FixedUpdate()
    {
        //if (!isLocalPlayer)
        //    return;
        if (isDead)
            return;

        transform.Translate(new Vector3(horizontal * moveSpeed, vertical, 0), Space.World);
        if (!isGrounded)
            transform.Translate(new Vector3(0, -(gravity/10), 0), Space.World);


        //if (jumpSpeed > 0)
        //    characterController.Move(direction * Time.fixedDeltaTime);
        //else
        //    characterController.SimpleMove(direction);

        //isGrounded = characterController.isGrounded;
        //velocity = characterController.velocity;
    }

    void UpdateAnimatorVar()
    {
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isDead", isDead);
        animator.SetBool("isShooting", isShooting);
        animator.SetFloat("move input", horizontal);
    }


}