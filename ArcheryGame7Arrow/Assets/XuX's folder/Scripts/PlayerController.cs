using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private Camera cam;
    private NetworkTransform cubeTransform;
    private float _horizontalInput;
    private float _verticalInput;
    private Vector2 mousePos;

    private void Start()
    {
        cubeTransform = GetComponent<NetworkTransform>();
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        Move();
        //Vector2 lookDir = mousePos - cubeTransform.transform.position; // aim Vector
    }


    private void Move()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        cubeTransform.transform.Translate(new Vector3(_horizontalInput, 0, _verticalInput) * speed, Space.World);
    }
}
