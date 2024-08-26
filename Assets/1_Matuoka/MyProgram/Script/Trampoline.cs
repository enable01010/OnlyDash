using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    #region Fields

    [SerializeField] private float jumpPower = 10f;

    #endregion


    #region MonoBehaviourMethod

    private void Start()
    {

    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsGuardOnCollision(collision) == true) return;
    }

    #endregion


    #region CustomMethod

    // CustomMethod
    private void CustomMethod()
    {

    }

    private bool IsGuardOnCollision(Collision collision)
    {
        if (collision.gameObject.tag != "Player") return true;
        //if (collision.gameObject.tag != "Player") return true;

        return false;
    }

    #endregion
}
