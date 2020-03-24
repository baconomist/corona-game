using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Pleb : MonoBehaviour
{
    public Material baseMaterial;
    public Material infectedMaterial;
    public Transform infectionCylinder;

    public float speed = 3.0f;
    public float infectedSpeed = 5.0f;
    public float infectionRadius = 3.0f;
    public float infectionSpeed = 5.0f;
    public bool infected = false;
    
    [Header("Random Do Not Touch!")]
    public bool follower = Random.value > 0.8f;

    private Transform _infectedPlebs;
    private Transform _basePlebs;

    private RandomMovement _randomMovement;
    private NavMeshAgent _nav;

    private Vector3 _cylinderStartScale;

    private void OnValidate()
    {
        if (infected)
            GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials = new Material[]
                {GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials[0], infectedMaterial};
        else
            GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials = new Material[]
                {GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials[0], baseMaterial};
    }

    void Start()
    {
        _randomMovement = new RandomMovement(3000 + 2000 * Random.value, speed, transform);
        _nav = GetComponent<NavMeshAgent>();

        _infectedPlebs = GameManager.Instance.infectedPlebs;
        _basePlebs = GameManager.Instance.basePlebs;

        _cylinderStartScale = infectionCylinder.localScale;

        follower = Random.value > 0.8f;

        if (infected)
            Infect();
    }

    void Update()
    {
        if (!GameManager.Instance.running)
            return;

        if (!infected)
             RandomMotion();
        else if(follower)
             FollowPlayerEntity();
        else
        {
            // Disable AI for "non- _followers", its probably hard enough as is
            RandomMotion();

            if (infected)
            {
                bool nearSomething = false;
                foreach (Transform t in _basePlebs.transform)
                {
                    if (Vector3.Distance(transform.position, t.position) <= infectionRadius)
                    {
                        infectionCylinder.localScale +=
                            new Vector3(Time.deltaTime * infectionSpeed, 0, Time.deltaTime * infectionSpeed);
                        nearSomething = true;
                    }
                }

                if (Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) <=
                    infectionRadius)
                {
                    infectionCylinder.localScale +=
                        new Vector3(Time.deltaTime * infectionSpeed, 0, Time.deltaTime * infectionSpeed);
                    nearSomething = true;
                }

                if (!nearSomething)
                {
                    infectionCylinder.localScale = _cylinderStartScale;
                }
            }
        }
    }

    void RandomMotion()
    {
        _randomMovement.Update();
    }

    void FollowPlayer()
    {
        FollowTransform(GameManager.Instance.player.transform);
    }

    void FollowPlayerEntity()
    {
        //FollowTransform(_currentFollowingTransform);
        
        if (Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) <= infectionRadius)
        {
            infectionCylinder.localScale +=
                new Vector3(Time.deltaTime * infectionSpeed, 0, Time.deltaTime * infectionSpeed);
        }
        else
        {
            infectionCylinder.localScale = _cylinderStartScale;
        }

        _nav.speed = speed;
        _nav.SetDestination(GameManager.Instance.player.transform.position);
        
    }

    void FollowTransform(Transform t)
    {
        // Keep the same direction to the player but keep distance constant(1.0f) via normalization
        Vector3 normalizedTranslateVec =
            Vector3.Normalize(t.position - transform.position);
        transform.position += normalizedTranslateVec * Time.deltaTime * speed;
    }

    void Infect()
    {
        GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials = new Material[]
            {GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials[0], infectedMaterial};
        if (_infectedPlebs.transform != null)
            transform.parent = _infectedPlebs.transform;
        speed = infectedSpeed;
        infected = true;
    }

    public void OnInfectionCylinderCollided(Collider other)
    {
        if (!infected)
            return;

        if (other.gameObject.GetComponent<Player>() != null)
        {
            GameManager.Instance.player.OnInfectionCylinderCollided();

            // Reset cylinder size to give player a chance to live after they lose their mask
            foreach (Transform pleb in _infectedPlebs)
            {
                pleb.GetComponent<Pleb>().infectionCylinder.localScale = _cylinderStartScale;
            }

            infectionCylinder.localScale = _cylinderStartScale;
        }
        else if (other.gameObject.GetComponent<Pleb>() != null)
            other.gameObject.GetComponent<Pleb>().Infect();
    }

    class RandomMovement
    {
        private float _duration;
        private float _speed;
        private float _timer = 0;

        private Vector3 moveBy;

        private Transform _transform;

        public RandomMovement(float durationUntilNewMS, float speed, Transform transform)
        {
            _duration = durationUntilNewMS;
            _speed = speed;
            _transform = transform;

            CalculateMoveBy();
        }

        public void Update()
        {
            if (_timer * 1000 < _duration)
                _transform.position += moveBy;
            else
            {
                CalculateMoveBy();
                _timer = 0;
            }

            _timer += Time.deltaTime;

            Vector3 vec = _transform.position + moveBy;
            _transform.LookAt(new Vector3(vec.x, _transform.position.y, vec.z));
        }

        private void CalculateMoveBy()
        {
            int direction = Random.value > 0.5f ? -1 : 1;
            moveBy = Utils.Clamp(Utils.Lerp(new Vector3(Random.value, 0, Random.value), 1.0f, 2.0f), 1.0f,
                         2.0f) *
                     Time.deltaTime *
                     _speed * direction;
        }
    }
}