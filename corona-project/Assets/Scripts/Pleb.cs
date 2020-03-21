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
    public Transform infectedPlebs;
    public Transform basePlebs;

    public float speed = 3.0f;
    public float infectedSpeed = 5.0f;
    public float infectionRadius = 3.0f;
    public float infectionSpeed = 5.0f;
    public bool infected = false;

    private RandomMovement _randomMovement;
    private Transform _currentFollowingTransform = null;
    private NavMeshAgent _nav;

    private void OnValidate()
    {
        if (infected)
            Infect();
        else
        {
            GetComponent<MeshRenderer>().sharedMaterial = baseMaterial;
        }

        GetComponent<NavMeshAgent>().speed = speed;
    }

    void Start()
    {
        _randomMovement = new RandomMovement(3000 + 2000 * Random.value, speed, transform);
        _nav = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!infected)
            RandomMotion();
        else
            FollowNearestEntity();
    }

    void RandomMotion()
    {
        _randomMovement.Update();
    }

    void FollowPlayer()
    {
        FollowTransform(GameManager.Instance.player.transform);
    }

    void FollowNearestEntity()
    {
        if (basePlebs.childCount > 0)
        {
            Transform closest = basePlebs.GetChild(0);
            foreach (Transform t in basePlebs.transform)
            {
                if (Vector3.Distance(transform.position, t.position) <
                    Vector3.Distance(transform.position, closest.position))
                {
                    closest = t;
                }
            }

            if (Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) <
                Vector3.Distance(transform.position, closest.position))
            {
                closest = GameManager.Instance.player.transform;
            }

            _currentFollowingTransform = closest;
        }
        else
        {
            _currentFollowingTransform = GameManager.Instance.player.transform;
        }


        if (_currentFollowingTransform != null)
        {
            //FollowTransform(_currentFollowingTransform);
            if (Vector3.Distance(transform.position, _currentFollowingTransform.position) <= infectionRadius)
            {
                infectionCylinder.localScale +=
                    new Vector3(Time.deltaTime * infectionSpeed, 0, Time.deltaTime * infectionSpeed);
            }
            else
            {
                infectionCylinder.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }

            _nav.speed = speed;
            _nav.SetDestination(_currentFollowingTransform.position);
        }
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
        GetComponent<MeshRenderer>().sharedMaterial = infectedMaterial;
        transform.parent = infectedPlebs.transform;
        speed = infectedSpeed;
        infected = true;
    }
    public void OnInfectionCylinderCollided(Collider other)
    {
        if(other.gameObject.GetComponent<Player>() != null)
            GameManager.Instance.Restart();
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
        }

        private void CalculateMoveBy()
        {
            int direction = Random.value > 0.5f ? -1 : 1;
            moveBy = Utils.Clamp(Utils.Lerp(new Vector3(Random.value, Random.value, Random.value), 1.0f, 2.0f), 1.0f,
                         2.0f) *
                     Time.deltaTime *
                     _speed * direction;
        }
    }
}