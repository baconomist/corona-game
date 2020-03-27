using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShoppingCart : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private BoxCollider[] _boxColliders;
    private List<GameObject> _cartObjects;

    private void Start()
    {
        _meshRenderer = transform.parent.GetComponent<MeshRenderer>();
        _boxColliders = GetComponents<BoxCollider>();
        _cartObjects = new List<GameObject>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<ShoppingCartItem>() == null)
        {
            foreach (BoxCollider boxCollider in _boxColliders)
                Physics.IgnoreCollision(other.collider, boxCollider);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ShoppingCartItem>() == null)
            return;

        if (other.gameObject.GetComponent<ShoppingCartItem>().canAttachToCart)
        {
            if (other.gameObject.GetComponent<FixedJoint>() == null)
                other.gameObject.AddComponent<FixedJoint>();
            
            FixedJoint joint = other.gameObject.GetComponent<FixedJoint>();
            joint.connectedBody = GetComponent<Rigidbody>();

            other.gameObject.GetComponent<BoxCollider>().isTrigger = true;
            other.gameObject.GetComponent<ShoppingCartItem>().attachedToCart = true;
            
            _cartObjects.Add(other.gameObject);
        }
        else
        {
            GameManager.Instance.player.TakeNearestItems();
        }

//        else
//        {
//            TeleportIntoCart(other.gameObject);
//        }
    }

    public void TeleportIntoCart(GameObject g)
    {
        g.transform.position = transform.position +
                                              new Vector3(Random.value * _meshRenderer.bounds.size.x / 4,
                                                  _meshRenderer.bounds.size.y / 2,
                                                  Random.value * _meshRenderer.bounds.size.z / 4);
    }

    private void OnTriggerExit(Collider other)
    {
//        if (other.gameObject.GetComponent<ShoppingCartItem>() == null)
//            return;
//
//        other.gameObject.GetComponent<ShoppingCartItem>().canAttachToCart = true;
    }
}