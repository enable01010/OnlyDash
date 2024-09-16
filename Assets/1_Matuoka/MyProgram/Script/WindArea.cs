using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindArea : MonoBehaviour
{
    #region Fields

    Player.Wind_Add wind_Add = new Player.Wind_Add();



    #endregion


    #region MonoBehaviourMethod

    private void Awake()
    {
        
    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {

    }

    #endregion


    #region CustomMethod

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            player.AddAdditionalState(wind_Add);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            player.RemoveAdditionalState(wind_Add);
        }
    }

    #endregion
}
