using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class ZipLineArea : MonoBehaviour
{
    public SplineNearestPos splinePos { get; private set; }// ŠeSpline‚ÌPlayer‚Æˆê”Ô‹ß‚¢ˆÊ’u
    public SplineContainer splineContainer { get; private set; }// Spline‚ÌComponent
    public float splineLength { get; private set; }// Spline‚Ì’·‚³

    private Vector3[] vertexes;

    [SerializeField] float margin = 5f;
    private Vector3 min;
    private Vector3 max;


    private void Awake()
    {
        splinePos = this.GetComponent<SplineNearestPos>();
        splineContainer = this.GetComponentInChildren<SplineContainer>();// InChildren‚Å‚à‰Â
        splineLength = splineContainer.CalculateLength();

        List<Vector3> tempVertexes = new List<Vector3>();
        foreach (var splin in splineContainer.Spline)
        {
            tempVertexes.Add(splin.Position);
        }
        vertexes = tempVertexes.ToArray();

        LibVector.GetRange(vertexes, out min, out max);
        LibVector.AddSpaceToRange(margin, ref min, ref max);
        

        BoxCollider boxCollider = this.GetComponent<BoxCollider>();
        boxCollider.SetColliderAreaOfLocal(min, max);
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
