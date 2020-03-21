using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionCylinder : MonoBehaviour
{
    [HideInInspector]
    public Pleb pleb;
    
    void Start()
    {
        pleb = transform.parent.gameObject.GetComponent<Pleb>();
    }

    private void OnTriggerEnter(Collider other)
    {
        pleb.OnInfectionCylinderCollided(other);
    }
}
