using UnityEngine;

namespace Mirror.Examples.Additive
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NetworkTransform))]
    public class CustomPlayerController : NetworkBehaviour
    {
        private CharacterController characterController;

        void Awake()
        {
            if (characterController == null)
                characterController = GetComponent<CharacterController>();
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            Camera.main.orthographic = false;
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = new Vector3(15f, 6f, 0f);
            Camera.main.transform.localEulerAngles = new Vector3(0, 90f, 0f);
        }

        void OnDisable()
        {
            if (isLocalPlayer)
            {
                Camera.main.orthographic = true;
                Camera.main.transform.SetParent(null);
                Camera.main.transform.localPosition = new Vector3(0f, 70f, 0f);
                Camera.main.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            }
        }

        [Header("Movement Settings")]
        public float moveSpeed = 8f;
        public float jumpForce = 0f;
        //public float turnSensitivity = 5f;
        //public float maxTurnSpeed = 150f;

        [Header("Diagnostics")]
        public float horizontal = 0f;
        public float vertical = 0f;
        public float turn = 0f;
        public float jumpSpeed = 0f;
        public bool isGrounded = true;
        public bool isFalling = false;
        public Vector3 velocity;

        void Update()
        {
            if (!isLocalPlayer)
                return;

            horizontal = Input.GetAxis("Horizontal");
            jumpSpeed = Mathf.Lerp(jumpSpeed, jumpForce, 0.5f);
            vertical = jumpSpeed;

            //// Q and E cancel each other out, reducing the turn to zero
            //if (Input.GetKey(KeyCode.Q))
            //    turn = Mathf.MoveTowards(turn, -maxTurnSpeed, turnSensitivity);
            //if (Input.GetKey(KeyCode.E))
            //    turn = Mathf.MoveTowards(turn, maxTurnSpeed, turnSensitivity);
            //if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.E))
            //    turn = Mathf.MoveTowards(turn, 0, turnSensitivity);
            //if (!Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.E))
            //    turn = Mathf.MoveTowards(turn, 0, turnSensitivity);

            if (isGrounded)
                isFalling = false;

            if ((isGrounded || !isFalling) && Input.GetKeyDown(KeyCode.Space))
                jumpSpeed = 20f;

            else if (!isGrounded)
            {
                isFalling = true;
                jumpSpeed = 0;
            }
        }

        void FixedUpdate()
        {
            if (!isLocalPlayer || characterController == null)
                return;

            horizontal = Input.GetAxis("Horizontal");
            transform.Translate(new Vector3(horizontal, 0, 0) * moveSpeed, Space.World);
            transform.Translate(new Vector3(0, vertical, 0) * jumpForce, Space.World);
            //transform.Rotate(0f, turn * Time.fixedDeltaTime, 0f);

            //Vector3 direction = new Vector3(horizontal, jumpSpeed, vertical);
            //direction = Vector3.ClampMagnitude(direction, 1f);
            //direction = transform.TransformDirection(direction);
            //direction *= moveSpeed;

            //if (jumpSpeed > 0)
            //    characterController.Move(direction * Time.fixedDeltaTime);
            //else
            //    characterController.SimpleMove(direction);

            isGrounded = characterController.isGrounded;
            velocity = characterController.velocity;
        }
    }
}
