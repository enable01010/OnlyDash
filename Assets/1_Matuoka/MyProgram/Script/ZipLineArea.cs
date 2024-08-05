using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class ZipLineArea : MonoBehaviour
{
    public SplineNearestPos splinePos { get; private set; }// �eSpline��Player�ƈ�ԋ߂��ʒu
    public SplineContainer splineContainer { get; private set; }// Spline��Component
    //public SplinePath<Spline> splinePath { get; private set; }// �킩��Ȃ�
    public float splineLength { get; private set; }// Spline�̒���

    public List<Vector3> vertex = new List<Vector3>();

    private void Awake()
    {
        GameObject parentObj = transform.parent.gameObject;
        splinePos = parentObj.GetComponent<SplineNearestPos>();
        splineContainer = parentObj.GetComponentInChildren<SplineContainer>();
        //splinePath = new SplinePath<Spline>(splineContainer.Splines);
        //splineLength = splinePath.GetLength();
        splineLength = splineContainer.CalculateLength();

        Vector3 startPos = splineContainer.EvaluatePosition(0f);

        Vector3 offset = splineContainer.Spline[0].Position;
        offset = new Vector3(0f, offset.y, 0f);

        foreach (var splin in splineContainer.Spline)
        {
            vertex.Add((Vector3)splin.Position + startPos - offset);
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
