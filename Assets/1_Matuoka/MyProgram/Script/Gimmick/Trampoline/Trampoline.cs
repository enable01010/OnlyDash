using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Trampolined
{
    public void TrampolineJump(float trampolineJumpPower);
}


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

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out I_Trampolined i_Trampolined))
        {
            i_Trampolined.TrampolineJump(jumpPower);
        }
    }

    #endregion


    #region CustomMethod



    #endregion
}
