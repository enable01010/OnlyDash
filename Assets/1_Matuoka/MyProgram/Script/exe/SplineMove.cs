using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineMove : MonoBehaviour
{
    #region Fields

    // PlayerÇÃComponent
    private Rigidbody rbody;

    // Splineä÷åW
    private SplineNearestPos[] splinePos;// äeSplineÇÃPlayerÇ∆àÍî‘ãﬂÇ¢à íu
    private SplineContainer[] splineContainer;// SplineÇÃComponent
    private SplinePath<Spline>[] splinePath;// ÇÌÇ©ÇÁÇ»Ç¢
    private float[] splineLength;// SplineÇÃí∑Ç≥

    [SerializeField, ReadOnly] private int nearSplineNumber = 0;
    [SerializeField, ReadOnly] private SplineNearestPos nearSplinePos;
    [SerializeField, ReadOnly] private SplineContainer nearSplineContainer;
    [SerializeField, ReadOnly] private SplinePath<Spline> nearSplinePath;// ÇÌÇ©ÇÁÇ»Ç¢
    [SerializeField, ReadOnly] private float nearSplineLength;

    // 
    private bool isInputKey = false;
    private bool isRide = false;
    private bool isDirectionPlus = true;

    // èÊÇËénÇﬂÇÈÇ∆Ç´ÇÃãóó£ä÷åW
    [SerializeField, ReadOnly] private bool isRideRange = false;
    [SerializeField, ReadOnly] private float nearDistance = 0f;
    [SerializeField] private float rideRange = 10f;

    // speedä÷åW
    [SerializeField] private float speed = 5f;
    private float oneFrameRate;
    [SerializeField, ReadOnly] private float rideElapsedTime = 0f;
    private const float stopTime = 0.3f;

    // 
    [SerializeField, ReadOnly] private float nowRate;
    [SerializeField] private float dirFreezeLength = 10f;
    [SerializeField] private float edgeStartLength = 3f;
    [SerializeField] private float edgeEndLength = 3f;

    // íÕÇﬁà íu
    [SerializeField] private Transform playerHandPos;
    private Vector3 offsetPlayerPos;

    // 
    [SerializeField] private bool isFreezeRotation = false;
    [SerializeField] private float jumpPowerY = 10f;
    [SerializeField] private float jumpPowerXZ = 10f;

    // UI
    [SerializeField] private GameObject canvas;

    #endregion


    #region MonoBehaviourMethod

    private void Start()
    {
        rbody = this.gameObject.GetComponent<Rigidbody>();

        // ìríÜÇ≈ëÂÇ´Ç≥ïœÇÌÇÁÇ»Ç¢Ç»ÇÁStart
        offsetPlayerPos.x = playerHandPos.transform.localPosition.x * this.transform.lossyScale.x;
        offsetPlayerPos.y = playerHandPos.transform.localPosition.y * this.transform.lossyScale.y;
        offsetPlayerPos.z = playerHandPos.transform.localPosition.z * this.transform.lossyScale.z;

        GameObject[] obj = GameObject.FindGameObjectsWithTag("ZipLine");

        splinePos = new SplineNearestPos[obj.Length];
        splineContainer = new SplineContainer[obj.Length];
        splinePath = new SplinePath<Spline>[obj.Length];
        splineLength = new float[obj.Length];

        for (int i = 0; i < obj.Length; i++)
        {
            splinePos[i] = obj[i].GetComponent<SplineNearestPos>();

            splineContainer[i] = obj[i].GetComponentInChildren<SplineContainer>();
            splinePath[i] = new SplinePath<Spline>(splineContainer[i].Splines);
            splineLength[i] = splinePath[i].GetLength();
        }
    }

    private void Update()
    {
        InputUpdate();
    }

    private void FixedUpdate()
    {
        NearZipLineUpdate();
        RideRangeUpdate();

        if (StartZipLineCheck() == true) StartZipLine();
        if (EndZipLineCheck() == true) EndZipLine();

        if (isRide == true)
        {
            NowRateUpdate();
            MoveRotation();
            MovePos();
        }

        InputReset();
    }

    #endregion


    #region CustomMethod

    private void InputUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Pause) || Input.GetKeyDown("joystick button 0")/* || Input.GetKeyDown("joystick button 1")*/)
        {
            isInputKey = true;
        }
    }

    private void InputReset()
    {
        isInputKey = false;
    }

    private void NowRateUpdate()
    {
        rideElapsedTime += Time.fixedDeltaTime;
        if (rideElapsedTime < stopTime)
        {
            return;
        }

        if (isDirectionPlus == true)
        {
            nowRate += oneFrameRate;
        }
        else
        {
            nowRate -= oneFrameRate;
        }
    }

    private bool StartZipLineCheck()
    {
        if (isInputKey == false) return false;
        if (isRide == true) return false;
        if (isRideRange == false) return false;

        InputReset();
        return true;
    }

    private void StartZipLine()
    {
        isRide = true;
        rbody.isKinematic = true;
        nowRate = Mathf.Clamp(nearSplinePos.rate, RangeToRate(edgeStartLength), 1f - RangeToRate(edgeStartLength));
        
        StartZipLineOneFrameRate();
        StartZipLineDirection();
    }

    private void StartZipLineDirection()
    {
        if (nowRate < RangeToRate(dirFreezeLength))
        {
            isDirectionPlus = true;
            return;
        }
        else if (nowRate > 1f - RangeToRate(dirFreezeLength))
        {
            isDirectionPlus = false;
            return;
        }

        Vector3 dir = nearSplineContainer.EvaluatePosition(nearSplinePath, nowRate + 0.01f) - nearSplineContainer.EvaluatePosition(nearSplinePath, nowRate);
        float angle = Vector3.Angle(this.transform.forward.normalized, dir.normalized);
        isDirectionPlus = angle <= 90;
    }

    private void StartZipLineOneFrameRate()
    {
        oneFrameRate = (speed / nearSplineLength) * Time.fixedDeltaTime;
    }

    private void EndZipLine()
    {
        isRide = false;
        rbody.isKinematic = false;
        this.transform.rotation = Quaternion.Euler(0, this.transform.localEulerAngles.y, 0);
        rideElapsedTime = 0f;

        EndZipLineJump();
    }

    private void EndZipLineJump()
    {
        Vector3 tempForward = this.transform.forward.normalized;
        Vector3 jumpPowe = tempForward * jumpPowerXZ + Vector3.up * jumpPowerY;
        rbody.AddForce(jumpPowe, ForceMode.Impulse);
    }

    private bool EndZipLineCheck()
    {
        if (isRide == false) return false;

        if (EdgeEndLengthCheck() == true) return true;

        if (isInputKey == false) return false;

        InputReset();
        return true;
    }

    private bool EdgeEndLengthCheck()
    {
        float tempNowRate = nowRate;
        if (isDirectionPlus == false)
        {
            tempNowRate = 1f - tempNowRate;
        }

        if (tempNowRate > 1f - RangeToRate(edgeEndLength))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void NearZipLineUpdate()
    {
        nearDistance = Mathf.Infinity;

        for (int i = 0; i < splinePos.Length; i++)
        {
            if (splinePos[i].distance < nearDistance)
            {
                nearDistance = splinePos[i].distance;
                nearSplineNumber = i;
            }
        }

        nearSplinePos = splinePos[nearSplineNumber];
        nearSplineContainer = splineContainer[nearSplineNumber];
        nearSplinePath = splinePath[nearSplineNumber];
        nearSplineLength = splineLength[nearSplineNumber];
    }

    private void RideRangeUpdate()
    {
        isRideRange = nearDistance < rideRange;
        canvas.SetActive(isRideRange);
    }

    private void MoveRotation()
    {
        Vector3 forward = nearSplineContainer.EvaluateTangent(nearSplinePath, nowRate);
        if (isDirectionPlus != true) forward *= -1f;

        Vector3 up = nearSplineContainer.EvaluateUpVector(nearSplinePath, nowRate);

        this.transform.rotation = Quaternion.LookRotation(forward, up);

        if (isFreezeRotation == true)
        {
            this.transform.rotation = Quaternion.Euler(0, this.transform.localEulerAngles.y, 0);
        }
    }

    private void MovePos()
    {
        this.transform.position = nearSplineContainer.EvaluatePosition(nearSplinePath, nowRate);

        float rotX = this.transform.localEulerAngles.x;
        float rotY = this.transform.localEulerAngles.y;
        float rotZ = this.transform.localEulerAngles.z;

        Quaternion rot = Quaternion.Euler(rotX, rotY, rotZ);
        this.transform.position -= rot * offsetPlayerPos;
    }

    private float RangeToRate(float num)
    {
        return Mathf.Clamp01(num / nearSplineLength);
    }

    #endregion
}
