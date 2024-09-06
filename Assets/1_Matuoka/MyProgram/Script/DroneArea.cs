using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class DroneArea : MonoBehaviour
{
    public SplineNearestPos splinePos { get; private set; }// 各SplineのPlayerと一番近い位置
    public SplineContainer splineContainer { get; private set; }// SplineのComponent
    public float splineLength { get; private set; }// Splineの長さ

    [SerializeField] float margin = 5f;

    public GameObject droneObj { get; private set; }
    public GameObject droneObj2 { get; private set; }

    private void Awake()
    {
        splinePos = this.GetComponent<SplineNearestPos>();
        splineContainer = this.GetComponentInChildren<SplineContainer>();// InChildrenでも可
        splineLength = splineContainer.CalculateLength();

        BoxCollider boxCollider = this.GetComponent<BoxCollider>();

        splineContainer.SetColliderArea(boxCollider, margin);

        droneObj = transform.GetChild(0).gameObject;
        droneObj.transform.position = splineContainer.EvaluatePosition(0);

        droneObj = transform.GetChild(1).gameObject;
        droneObj.transform.position = splineContainer.EvaluatePosition(1);
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
