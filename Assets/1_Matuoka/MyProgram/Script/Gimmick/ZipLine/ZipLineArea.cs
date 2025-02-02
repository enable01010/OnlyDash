using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class ZipLineArea : MonoBehaviour
{
    public SplineNearestPos splinePos { get; private set; }// 各SplineのPlayerと一番近い位置
    public SplineContainer splineContainer { get; private set; }// SplineのComponent
    public float splineLength { get; private set; }// Splineの長さ

    [SerializeField] float margin = 5f;

    private void Awake()
    {
        splinePos = this.GetComponent<SplineNearestPos>();
        splineContainer = this.GetComponentInChildren<SplineContainer>();// InChildrenでも可
        splineLength = splineContainer.CalculateLength();

        BoxCollider boxCollider = this.GetComponent<BoxCollider>();

        splineContainer.SetColliderArea(boxCollider, margin);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            player.SetZipLineArea(this);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            player.DeleteZipLineArea(this);
        }
    }
}
