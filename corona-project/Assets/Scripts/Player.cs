using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using UnityEngine;
using TouchPhase = UnityEngine.TouchPhase;

public class Player : MonoBehaviour
{
    public GameObject modelMasked;
    public GameObject modelUnmasked;

    public float baseSpeed = 2.0f;
    public float sprintSpeed = 4.0f;
    public float itemReach = 2.0f;
    public float staminaDepleteSpeed = 50.0f;
    public float timeBeforeStaminaRegen = 1.0f;
    public float staminaRegenSpeed = 100.0f;
    public bool isMasked = false;

    private SprintBar _sprintBar;
    private Camera _camera;
    private ParticleSystem _particleSystem;
    private MultiAnimator _animator;
    private BoxCollider _boxCollider;

    private ShoppingCart _shoppingCart;

    private bool _isDead = false;
    private Vector3 _cameraOffset;
    private float _staminaLastUsedTimeStamp = 0;

    private FingersScript _fingersScript;
    private TapGestureRecognizer _doubleTapGestureRecognizer;
    private LongPressGestureRecognizer _longPressGestureRecognizer;
    private SwipeGestureRecognizer _swipeGestureRecognizer;

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


        _fingersScript = GetComponent<FingersScript>();

        _doubleTapGestureRecognizer = new TapGestureRecognizer();
        _doubleTapGestureRecognizer.NumberOfTapsRequired = 2;
        _doubleTapGestureRecognizer.StateUpdated += Controls.OnDoubleTap;

        _longPressGestureRecognizer = new LongPressGestureRecognizer();
        _longPressGestureRecognizer.MinimumDurationSeconds = 0.3f;
        _longPressGestureRecognizer.StateUpdated += Controls.OnLongPress;

        _swipeGestureRecognizer = new SwipeGestureRecognizer();
        _swipeGestureRecognizer.Direction = SwipeGestureRecognizerDirection.Up;
        _swipeGestureRecognizer.StateUpdated += Controls.OnSwipe;

        _fingersScript.AddGesture(_doubleTapGestureRecognizer);
        _fingersScript.AddGesture(_longPressGestureRecognizer);
        _fingersScript.AddGesture(_swipeGestureRecognizer);

        _fingersScript.TreatMousePointerAsFinger = !Input.touchSupported;
    }

    private void Update()
    {
        Controls.Update();
        
        float speed = baseSpeed;

        if (Controls.Sprinting() && _sprintBar.Percent > 0)
        {
            speed = sprintSpeed;
            if (!_particleSystem.isPlaying)
                _particleSystem.Play();
            _animator.SetBool("Running", true);
            _sprintBar.DecreaseBy(Time.deltaTime * staminaDepleteSpeed);

            _staminaLastUsedTimeStamp = Time.time;
        }
        else
        {
            if (_particleSystem.isPlaying)
                _particleSystem.Stop();

            _animator.SetBool("Running", false);

            if (Time.time - _staminaLastUsedTimeStamp >= timeBeforeStaminaRegen)
                _sprintBar.DecreaseBy(-Time.deltaTime * staminaRegenSpeed);
        }

        //if (Controls.Moving())
        // Always moving?
        transform.position += transform.forward * Time.deltaTime * speed;

        if (Controls.TakingItem())
            TakeNearestItem();

        transform.Rotate(0, Controls.GetTurnBy() * Time.deltaTime, 0);
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
        _isDead = true;
        
        //GameManager.Instance.Restart();
    }

    public void SetMasked(bool masked)
    {
        modelMasked.SetActive(masked);
        modelUnmasked.SetActive(!masked);
        isMasked = masked;
    }

    public void OnInfectionCylinderCollided()
    {
        if (isMasked)
        {
            SetMasked(false);
            
        }
        else if(!_isDead)
        {
            Die();
        }
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

    private static class Controls
    {
        public const bool DEBUG_KEYBOARD = true;
        public const float MOBILE_SPRINT_TIME_SEC = 1.0f;

        private static float _doubleTapTimeStamp = -99999;
        private static float _rotationTimeStamp = -99999;
        private static float _swipeTimeStamp = -99999;
        private static float _turnBy = 0;
        private static bool _takingItem = false;

        public static bool Sprinting()
        {
            bool mobile = Time.time - _doubleTapTimeStamp <= MOBILE_SPRINT_TIME_SEC;
            return (DEBUG_KEYBOARD && Input.GetKey(KeyCode.LeftShift)) || mobile;
        }

        public static bool TakingItem()
        {
            return _takingItem;
        }

        public static float GetTurnBy()
        {
            return _turnBy;
        }

        public static void Update()
        {
            _turnBy = 0;
            if (DEBUG_KEYBOARD)
            {
                if (Input.GetKey(KeyCode.D))
                    _turnBy = 100;
                if (Input.GetKey(KeyCode.A))
                    _turnBy = -100;
            }
            
            _takingItem = Input.GetKey(KeyCode.F) || (Time.time - _swipeTimeStamp <= 0.25f);
        }

        public static void OnLongPress(GestureRecognizer gesture)
        {
            if (gesture.FocusX > Screen.width / 2)
                _turnBy = 100;
            else
                _turnBy = -100;
        }

        public static void OnDoubleTap(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
                _doubleTapTimeStamp = Time.time;
        }

        public static void OnSwipe(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
                _swipeTimeStamp = Time.time;
        }
    }
}