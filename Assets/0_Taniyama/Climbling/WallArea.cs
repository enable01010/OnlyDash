using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class WallArea : MonoBehaviour
{
    public SplineContainer _spline { get; private set; }

    [SerializeField] float margin = 2f;

    private void Awake()
    {
        _spline = this.GetComponent<SplineContainer>();

        BoxCollider boxCollider = this.GetComponent<BoxCollider>();

        _spline.SetColliderArea(boxCollider, margin);
    }


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
            player.DeleteWallArea(this);
        }
    }
}
