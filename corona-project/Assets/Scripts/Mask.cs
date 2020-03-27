using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mask : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.IsChildOf(GameManager.Instance.player.transform))
        {
            GameManager.Instance.player.SetMasked(true);
            Destroy(transform.parent.gameObject);
        }
    }
}
