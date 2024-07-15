using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallArea : MonoBehaviour
{
    [field: SerializeField] public Vector3 rot { get; private set; }
    [field: SerializeField] public Transform pos { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Player>(out Player player))
        {
            player.SetWallArea(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            player.SetWallArea(null);
        }
    }
}
