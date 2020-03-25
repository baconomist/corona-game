using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using UnityEngine;
using TouchPhase = UnityEngine.TouchPhase;

public class Player : MonoBehaviour
{
    [Header("Assets")] public GameObject modelMasked;
    public GameObject modelUnmasked;
    public AudioClip dyingAudio;
    public AudioClip pickupAudio;
    public AudioClip footStepAudio;

    [Header("Attributes")] public float baseSpeed = 2.0f;
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
    private Rigidbody _rigidbody;
    private AudioSource _audioSource1;
    private AudioSource _audioSource2;

    private ShoppingCart _shoppingCart;

    private bool _isDead = false;
    private Vector3 _cameraOffset;
    private float _staminaLastUsedTimeStamp = 0;

    private FingersScript _fingersScript;
    private TapGestureRecognizer _tapRecognizer;
    private LongPressGestureRecognizer _longPressGestureRecognizer;

    private float _deathTimeStamp = -1;

    private void OnValidate()
    {
        SetMasked(isMasked);
    }

    private void Awake()
    {
        _audioSource1 = gameObject.AddComponent<AudioSource>();
        _audioSource1.clip = footStepAudio;
        
        _audioSource2 = gameObject.AddComponent<AudioSource>();
        _audioSource2.clip = pickupAudio;
    }

    private void Start()
    {
        _sprintBar = FindObjectOfType<SprintBar>();
        _camera = Camera.main;
        _particleSystem = GetComponent<ParticleSystem>();
        _animator = new MultiAnimator(modelMasked.GetComponentInChildren<Animator>(),
            modelUnmasked.GetComponentInChildren<Animator>());
        _boxCollider = GetComponent<BoxCollider>();
        _rigidbody = GetComponent<Rigidbody>();

        _shoppingCart = GetComponentInChildren<ShoppingCart>();

        _cameraOffset = _camera.transform.position - transform.position;

        _fingersScript = GetComponent<FingersScript>();

        _tapRecognizer = new TapGestureRecognizer();
        _tapRecognizer.NumberOfTapsRequired = 1;
        _tapRecognizer.StateUpdated += Controls.OnTap;

        _longPressGestureRecognizer = new LongPressGestureRecognizer();
        _longPressGestureRecognizer.MinimumDurationSeconds = 0.1f;
        _longPressGestureRecognizer.StateUpdated += Controls.OnLongPress;

        _fingersScript.AddGesture(_tapRecognizer);
        _fingersScript.AddGesture(_longPressGestureRecognizer);

        _fingersScript.TreatMousePointerAsFinger = !Input.touchSupported;
    }

    private void Update()
    {
        if (!GameManager.Instance.running || _isDead)
        {
            if (_isDead)
            {
                if (_deathTimeStamp <= -1)
                    _deathTimeStamp = Time.time;

                if (_audioSource1.clip != dyingAudio)
                {
                    _audioSource1.clip = dyingAudio;
                    _audioSource1.loop = false;
                    _audioSource1.Play();
                }
                else if (!_audioSource1.isPlaying && Time.time - _deathTimeStamp >= 6.0f)
                {
                    GameManager.Instance.Restart();
                }
            }

            return;
        }

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

            if (!_audioSource1.isPlaying)
                _audioSource1.Play();
            _audioSource1.pitch = 1.5f;
        }
        else
        {
            if (_particleSystem.isPlaying)
                _particleSystem.Stop();

            _animator.SetBool("Running", false);

            if (Time.time - _staminaLastUsedTimeStamp >= timeBeforeStaminaRegen)
                _sprintBar.DecreaseBy(-Time.deltaTime * staminaRegenSpeed);

            _audioSource1.Stop();
        }

        _rigidbody.velocity = Vector3.zero;

        //if (Controls.Moving())
        // Always moving?
        transform.position += transform.forward * Time.deltaTime * speed;

        transform.Rotate(0, Controls.GetTurnBy() * Time.deltaTime, 0);
    }

    private void FixedUpdate()
    {
        _camera.transform.position = transform.position + _cameraOffset;
    }

    public void TakeNearestItems()
    {
        bool playAnim = false;

        foreach (Collider collider in Physics.OverlapSphere(transform.position, itemReach))
        {
            ShoppingCartItem shoppingCartItem = collider.gameObject.GetComponent<ShoppingCartItem>();
            if (shoppingCartItem != null)
            {
                if (Vector3.Distance(transform.position, shoppingCartItem.transform.position) > 9.0f)
                    playAnim = true;

                _shoppingCart.TeleportIntoCart(collider.gameObject);
                shoppingCartItem.interactionTimeStamp = Time.time;
                shoppingCartItem.canAttachToCart = true;
            }
        }

        if (playAnim)
        {
            _animator.SetTrigger("TakeItem");
            _audioSource2.Play();
        }
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

        if(_audioSource2 != null)
            _audioSource2.Play();
    }

    public void OnInfectionCylinderCollided()
    {
        if (isMasked)
        {
            SetMasked(false);
        }
        else if (!_isDead)
        {
            Die();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<ShoppingCartItem>() != null || other.gameObject.GetComponent<Pleb>() != null)
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
        public const bool DEBUG_KEYBOARD = false;
        public const float MOBILE_SPRINT_TIME_SEC = 1.0f;

        private static float _sprintTimeStamp = -99999;
        private static float _rotationTimeStamp = -99999;
        private static float _turnBy = 0;

        public static bool Sprinting()
        {
            bool mobile = Time.time - _sprintTimeStamp <= MOBILE_SPRINT_TIME_SEC;
            return (DEBUG_KEYBOARD && Input.GetKey(KeyCode.LeftShift)) || mobile;
        }

        public static float GetTurnBy()
        {
            return _turnBy;
        }

        public static void Update()
        {
            if (Time.time - _rotationTimeStamp >= 0.1f)
                _turnBy = 0;

            if (DEBUG_KEYBOARD)
            {
                if (Input.GetKey(KeyCode.D))
                    _turnBy = 100;
                if (Input.GetKey(KeyCode.A))
                    _turnBy = -100;
            }
        }

        public static void OnLongPress(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Began || gesture.State == GestureRecognizerState.Executing)
            {
                if (gesture.FocusX >= Screen.width * 0.7f)
                {
                    _turnBy = 100;
                    _rotationTimeStamp = Time.time;
                }
                else if (gesture.FocusX <= Screen.width * 0.3f)
                {
                    _turnBy = -100;
                    _rotationTimeStamp = Time.time;
                }
            }
        }

        public static void OnTap(GestureRecognizer gesture)
        {
            if (gesture.FocusX > Screen.width * 0.3f && gesture.FocusX < Screen.width * 0.7f)
                _sprintTimeStamp = Time.time;
        }
    }
}