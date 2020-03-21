using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject modelMasked;
    public GameObject modelUnmasked;

    public float baseSpeed = 2.0f;
    public float sprintSpeed = 4.0f;
    public float itemReach = 2.0f;
    public bool isMasked = false;

    private SprintBar _sprintBar;
    private Camera _camera;
    private ParticleSystem _particleSystem;
    private MultiAnimator _animator;
    private BoxCollider _boxCollider;

    private ShoppingCart _shoppingCart;

    private bool _dead = false;
    private Vector3 _cameraOffset;

    private void OnValidate()
    {
        SetMasked(isMasked);
    }

    private void Start()
    {
        _sprintBar = FindObjectOfType<SprintBar>();
        _camera = Camera.main;
        _particleSystem = GetComponent<ParticleSystem>();
        _animator = new MultiAnimator(modelMasked.GetComponentInChildren<Animator>(),
            modelUnmasked.GetComponentInChildren<Animator>());
        _boxCollider = GetComponent<BoxCollider>();

        _shoppingCart = GetComponentInChildren<ShoppingCart>();

        _cameraOffset = _camera.transform.position - transform.position;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float speed = baseSpeed;

        if (Input.GetKey(KeyCode.LeftShift) && _sprintBar.Percent > 0)
        {
            speed = sprintSpeed;
            if (!_particleSystem.isPlaying)
                _particleSystem.Play();
            _animator.SetBool("Running", true);
            _sprintBar.DecreaseBy(Time.deltaTime * 50);
        }
        else
        {
            if(_particleSystem.isPlaying)
                _particleSystem.Stop();
            
            _animator.SetBool("Running", false);
            _sprintBar.DecreaseBy(-Time.deltaTime * 30);
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

    private void TakeNearestItem()
    {
        foreach (Collider collider in Physics.OverlapSphere(transform.position, itemReach))
        {
            ShoppingCartItem shoppingCartItem = collider.gameObject.GetComponent<ShoppingCartItem>();
            if (shoppingCartItem != null)
            {
                _shoppingCart.TeleportIntoCart(collider.gameObject);
                shoppingCartItem.canAttachToCart = true;
            }
        }

        _animator.SetTrigger("TakeItem");
    }

    private void Die()
    {
        _animator.SetTrigger("Die");
        _dead = true;
    }

    private void SetMasked(bool masked)
    {
        modelMasked.SetActive(masked);
        modelUnmasked.SetActive(!masked);
        isMasked = masked;
    }

    public void EmptyCart()
    {
        _shoppingCart.EmptyItems();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<ShoppingCartItem>() != null)
            Physics.IgnoreCollision(other.collider, _boxCollider);
    }

    private class MultiAnimator
    {
        private Animator[] _animators;

        public MultiAnimator(params Animator[] animators)
        {
            _animators = animators;
        }

        public void SetBool(string name, bool val)
        {
            foreach (Animator animator in _animators)
                if (animator.gameObject.activeInHierarchy)
                    animator.SetBool(name, val);
        }

        public void SetTrigger(string name)
        {
            foreach (Animator animator in _animators)
                if (animator.gameObject.activeInHierarchy)
                    animator.SetTrigger(name);
        }
    }
}