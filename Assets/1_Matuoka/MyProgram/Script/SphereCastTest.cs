using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCastTest : MonoBehaviour
{
    [SerializeField] Vector3 origin;
    [SerializeField] float radius;
    [SerializeField] Vector3 direction;
    [SerializeField] float distance;
    [SerializeField] LayerMask groundLayers = 0;
    [SerializeField] int vertex = 64;

    void Start()
    {
        
    }

    
    void Update()
    {
        RaycastHit temp = LibPhysics.SphereCast(
            origin,
            radius,
            direction,
            distance,
            groundLayers
        );

        if (temp.IsHit())//.transform != null)
        {
            Debug.Log(temp.transform.gameObject.name);
        }
    }
}
