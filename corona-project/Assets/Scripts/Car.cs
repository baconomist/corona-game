using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.IsChildOf(GameManager.Instance.player.transform))
        {
            Player player = GameManager.Instance.player.GetComponent<Player>();
            player.EmptyCart();
        }
    }
}
