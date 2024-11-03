using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralCollider3D : MonoBehaviour
{
    public delegate void OnTrigger(Collider other);
    public OnTrigger onEnter;
    public OnTrigger onStay;
    public OnTrigger onExit;

    private void OnTriggerEnter(Collider other)
    {
        onEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        onStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        onExit(other);
    }
}
