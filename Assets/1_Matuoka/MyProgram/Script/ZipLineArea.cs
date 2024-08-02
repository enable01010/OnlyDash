using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class ZipLineArea : MonoBehaviour
{
    public SplineNearestPos splinePos { get; private set; }// 各SplineのPlayerと一番近い位置
    public SplineContainer splineContainer { get; private set; }// SplineのComponent
    //public SplinePath<Spline> splinePath { get; private set; }// わからない
    public float splineLength { get; private set; }// Splineの長さ

    public List<Vector3> vertex = new List<Vector3>();

    private void Awake()
    {
        GameObject parentObj = transform.parent.gameObject;
        splinePos = parentObj.GetComponent<SplineNearestPos>();
        splineContainer = parentObj.GetComponentInChildren<SplineContainer>();
        //splinePath = new SplinePath<Spline>(splineContainer.Splines);
        //splineLength = splinePath.GetLength();
        splineLength = splineContainer.CalculateLength();

        //List<Vector3> vertex = new List<Vector3>();
        foreach (var splin in splineContainer.Spline)
        {
            vertex.Add(splin.Position);
        }

        //BoxCollider boxCollider = parentObj.GetComponent<BoxCollider>();
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
