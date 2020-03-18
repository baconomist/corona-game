using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pleb : MonoBehaviour
{
    public float speed = 3.0f;

    private RandomMovement _randomMovement;

    void Start()
    {
        _randomMovement = new RandomMovement(3000 + 2000 * Random.value, speed, transform);
    }

    void Update()
    {
        RandomMotion();
        //FollowPlayer();
    }

    void RandomMotion()
    {
        _randomMovement.Update();
    }

    void FollowPlayer()
    {
        // Keep the same direction to the player but keep distance constant(1.0f) via normalization
        Vector3 normalizedTranslateVec =
            Vector3.Normalize(GameManager.Instance.player.transform.position - transform.position);
        transform.position += normalizedTranslateVec * Time.deltaTime * speed;
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
            moveBy = Utils.Clamp(Utils.Lerp(new Vector3(Random.value, Random.value, Random.value), 1.0f, 2.0f), 1.0f, 2.0f) *
                     Time.deltaTime *
                     _speed * direction;
        }
    }
}