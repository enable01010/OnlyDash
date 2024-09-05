using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class DroneArea : MonoBehaviour
{
    public SplineNearestPos splinePos { get; private set; }// �eSpline��Player�ƈ�ԋ߂��ʒu
    public SplineContainer splineContainer { get; private set; }// Spline��Component
    public float splineLength { get; private set; }// Spline�̒���

    [SerializeField] float margin = 5f;

    private void Awake()
    {
        splinePos = this.GetComponent<SplineNearestPos>();
        splineContainer = this.GetComponentInChildren<SplineContainer>();// InChildren�ł���
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
