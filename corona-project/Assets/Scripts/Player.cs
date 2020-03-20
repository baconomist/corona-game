using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float baseSpeed = 2.0f;
    public float sprintSpeed = 4.0f;

    private Camera _camera;
    private ParticleSystem _particleSystem;
    private Animator _animator;
    private BoxCollider _boxCollider;

    private bool _dead = false;
    private Vector3 _cameraOffset;

    void Start()
    {
        _camera = Camera.main;
        _particleSystem = GetComponent<ParticleSystem>();
        _animator = GetComponentInChildren<Animator>();
        _boxCollider = GetComponent<BoxCollider>();

        _cameraOffset = _camera.transform.position - transform.position;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float speed = baseSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = sprintSpeed;
            if (!_particleSystem.isPlaying)
                _particleSystem.Play();
            _animator.SetBool("Running", true);
        }
        else if (_particleSystem.isPlaying)
        {
            _particleSystem.Stop();
            _animator.SetBool("Running", false);
        }

        if (Input.GetKey(KeyCode.W))
            transform.position += transform.forward * Time.deltaTime * speed;

        if (Input.GetKey(KeyCode.F))
            TakeNearestItem();

        transform.Rotate(0, Input.GetAxis("Mouse X"), 0);
    }
    
    private void FixedUpdate()
    {
        _camera.transform.position = transform.position + _cameraOffset;
    }

    void TakeNearestItem()
    {
        _animator.SetTrigger("TakeItem");
    }

    void Die()
    {
        _animator.SetTrigger("Die");
        _dead = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.GetComponent<ShoppingCartItem>() != null)
            Physics.IgnoreCollision(other.collider, _boxCollider);
    }
}