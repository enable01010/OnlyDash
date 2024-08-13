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

    public Vector3[] vertexes;

    [SerializeField] float margin = 5f;
    public Vector3 min;
    public Vector3 max;


    private void Awake()
    {
        splinePos = this.GetComponent<SplineNearestPos>();
        splineContainer = this.GetComponentInChildren<SplineContainer>();// これでも可
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
        BoxColliderInit(boxCollider, min, max);
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

    // 親にCollider付いてる
    // min, max は　親のTransformの Position, Rotation, Scale が0のときの相対位置
    // 親のTransformの Position, Rotation, Scale 0じゃなくてOK
    private void BoxColliderInit(BoxCollider boxCollider, Vector3 min, Vector3 max)
    {
        Vector3 midpoint = Vector3.Lerp(min, max, 0.5f);
        boxCollider.center = midpoint;
        boxCollider.size = max - min;
        
    }

}
