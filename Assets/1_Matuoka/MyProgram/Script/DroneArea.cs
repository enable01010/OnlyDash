using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class DroneArea : MonoBehaviour
{
    public SplineNearestPos splinePos { get; private set; }// ŠeSpline‚ÌPlayer‚Æˆê”Ô‹ß‚¢ˆÊ’u
    public SplineContainer splineContainer { get; private set; }// Spline‚ÌComponent
    public float splineLength { get; private set; }// Spline‚Ì’·‚³

    [SerializeField] float margin = 5f;

    private void Awake()
    {
        splinePos = this.GetComponent<SplineNearestPos>();
        splineContainer = this.GetComponentInChildren<SplineContainer>();// InChildren‚Å‚à‰Â
        splineLength = splineContainer.CalculateLength();

        BoxCollider boxCollider = this.GetComponent<BoxCollider>();

        splineContainer.SetColliderArea(boxCollider, margin);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            player.SetDroneArea(this);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            player.DeleteDroneArea(this);
        }
    }
}
