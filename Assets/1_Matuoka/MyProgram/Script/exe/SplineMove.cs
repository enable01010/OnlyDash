using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineMove : MonoBehaviour
{
    #region Fields

    private Rigidbody rbody;
    private bool isInputKey = false;
    private bool isRide = false;
    [SerializeField, ReadOnly] private bool isRideRange = false;
    [SerializeField, ReadOnly] private float nearDistance = 0f;

    [SerializeField] private GameObject canvas;

    [SerializeField] private float rideRange = 10f;
    

    [SerializeField] private NearestPointExample nearZipLine;
    private NearestPointExample[] zipLineArray;
    [SerializeField] private SplineContainer m_Target;

    [SerializeField] private float speed = 5f;
    private float rideElapsedTime = 0f;
    private float stopTime = 0.3f;
    [SerializeField, ReadOnly] private float nowRate;

    [SerializeField] private float dirFreezeLength = 10f;
    [SerializeField] private float edgeStartLength = 3f;
    [SerializeField] private float edgeEndLength = 3f;

    private SplinePath<Spline> m_SplinePath;

    [SerializeField, ReadOnly] private float splineLength;

    [SerializeField] private Transform playerHandPos;
    private Vector3 offsetPlayerPos;

    private bool isDirectionPlus = true;

    [SerializeField] private bool isFreezeRotation = false;

    [SerializeField] private float jumpPower = 10f;

    #endregion


    #region MonoBehaviourMethod

    private void Start()
    {
        rbody = this.gameObject.GetComponent<Rigidbody>();

        m_SplinePath = new SplinePath<Spline>(m_Target.Splines);
        splineLength = m_SplinePath.GetLength();

        offsetPlayerPos = playerHandPos.transform.position - this.transform.position;

        GameObject[] obj = GameObject.FindGameObjectsWithTag("ZipLine");
        zipLineArray = new NearestPointExample[obj.Length];
        for (int i = 0; i < obj.Length; i++)
        {
            zipLineArray[i] = obj[i].GetComponent<NearestPointExample>();
        }
        
    }

    private void Update()
    {
        InputUpdate();
    }

    private void FixedUpdate()
    {
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
            nowRate += Time.fixedDeltaTime / 5f;
        }
        else
        {
            nowRate -= Time.fixedDeltaTime / 5f;
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
        nowRate = Mathf.Clamp(nearZipLine.rate, RangeToRate(edgeStartLength), 1f - RangeToRate(edgeStartLength));

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

        Vector3 dir = m_Target.EvaluatePosition(m_SplinePath, nowRate + 0.01f) - m_Target.EvaluatePosition(m_SplinePath, nowRate);
        float angle = Vector3.Angle(this.transform.forward.normalized, dir.normalized);
        isDirectionPlus = angle <= 90;
    }

    private void EndZipLine()
    {
        isRide = false;
        rbody.isKinematic = false;
        this.transform.rotation = Quaternion.Euler(0, this.transform.localEulerAngles.y, 0);

        rbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        rideElapsedTime = 0f;
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

    private void RideRangeUpdate()
    {
        if (nearZipLine == null)
        {
            isRideRange = false;
            nearDistance = Mathf.Infinity;

            return;
        }

        nearDistance = nearZipLine.distance;
        isRideRange = nearDistance < rideRange;
        canvas.SetActive(isRideRange);
    }

    private void MovePos()
    {
        this.transform.position = m_Target.EvaluatePosition(m_SplinePath, nowRate);

        this.transform.position -= offsetPlayerPos;
    }

    private void MoveRotation()
    {
        Vector3 forward = m_Target.EvaluateTangent(m_SplinePath, nowRate);
        if (isDirectionPlus != true) forward *= -1f;

        Vector3 up = m_Target.EvaluateUpVector(m_SplinePath, nowRate);

        this.transform.rotation = Quaternion.LookRotation(forward, up);

        if (isFreezeRotation == true)
        {
            this.transform.rotation = Quaternion.Euler(0, this.transform.localEulerAngles.y, 0);
        }
    }

    private float RangeToRate(float num)
    {
        return Mathf.Clamp01(num / splineLength);
    }

    #endregion
}
