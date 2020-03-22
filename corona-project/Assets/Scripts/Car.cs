using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ShoppingCartItem>() != null)
        {
            if(!other.gameObject.GetComponent<ShoppingCartItem>().scored)
                GameManager.Instance.score.AddItemScore(other.gameObject.GetComponent<ShoppingCartItem>().score);
            other.gameObject.GetComponent<ShoppingCartItem>().scored = true;
            Destroy(other.gameObject);
        }
    }
}
