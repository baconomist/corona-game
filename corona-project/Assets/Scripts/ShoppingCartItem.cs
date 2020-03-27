using System;
using UnityEngine;

public class ShoppingCartItem : MonoBehaviour
{
    public int score = 1000;
    public bool canAttachToCart = false;
    public float interactionTimeStamp = 0;
    public bool attachedToCart = false;
    public bool scored = false;

    private void Update()
    {
        if (Time.time - interactionTimeStamp >= 2.0f)
            canAttachToCart = false;
    }
}