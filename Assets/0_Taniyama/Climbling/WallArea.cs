using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallArea : MonoBehaviour
{
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
