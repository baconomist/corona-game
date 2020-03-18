using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float baseSpeed = 2.0f;
    public float sprintSpeed = 4.0f;
    
    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        float speed = baseSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
            speed = sprintSpeed;
        
        if (Input.GetKey(KeyCode.W))
            transform.position += transform.forward * Time.deltaTime * speed;
        
        transform.Rotate(0, Input.GetAxis("Mouse X"), 0);
    }

    private void FixedUpdate()
    {
        _camera.transform.position = new Vector3(transform.position.x, _camera.transform.position.y, transform.position.z);
    }
}
