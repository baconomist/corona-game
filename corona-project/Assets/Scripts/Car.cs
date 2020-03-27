using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    private AudioSource _audioSource;
    
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ShoppingCartItem>() != null)
        {
            if(!other.gameObject.GetComponent<ShoppingCartItem>().scored)
                GameManager.Instance.score.AddItemScore(other.gameObject.GetComponent<ShoppingCartItem>().score);
            other.gameObject.GetComponent<ShoppingCartItem>().scored = true;
            _audioSource.Play();
            Destroy(other.gameObject);
        }
    }
}
