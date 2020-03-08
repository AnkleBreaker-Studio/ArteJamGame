using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    private Camera cam;
    private NetworkTransform playerTransform;
    private Animator animator;
    private float _horizontalInput;
    private Vector3 mousePos;
    private Rigidbody rb;


    private void Start()
    {
        playerTransform = GetComponent<NetworkTransform>();
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        GetComponent<NetworkAnimator>().animator = animator;
    }

    void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        Move();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        Vector2 lookDir = mousePos - playerTransform.transform.position; // aim Vector
        //Debug.Log(lookDir);
    }


    private void Move()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        playerTransform.transform.Translate(new Vector3(_horizontalInput, 0, 0) * speed, Space.World);
    }

    private void Jump()
    {
        rb.velocity = Vector2.up * jumpForce;
        //animator.parameters.
        Debug.Log("jump!");
    }
}
