using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShoppingCart : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private BoxCollider[] _boxColliders;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _boxColliders = GetComponents<BoxCollider>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.GetComponent<ShoppingCartItem>() == null)
            foreach (BoxCollider boxCollider in _boxColliders)
                Physics.IgnoreCollision(other.collider, boxCollider);
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ShoppingCartItem>() == null)
            return;

        if (other.gameObject.GetComponent<ShoppingCartItem>().inCart)
        {
            if (other.gameObject.GetComponent<FixedJoint>() == null)
                other.gameObject.AddComponent<FixedJoint>();
            FixedJoint joint = other.gameObject.GetComponent<FixedJoint>();
            joint.connectedBody = GetComponent<Rigidbody>();
            other.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
        }
        else
        {
            other.gameObject.transform.position = transform.position + new Vector3(Random.value * _meshRenderer.bounds.size.x / 4, _meshRenderer.bounds.size.y / 2, Random.value * _meshRenderer.bounds.size.z / 4);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<ShoppingCartItem>() == null)
            return;

        other.gameObject.GetComponent<ShoppingCartItem>().inCart = true;
    }
}