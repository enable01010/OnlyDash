using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public partial class Player : SingletonActionListener<Player>
{
    public interface I_ZipLine
    {
        public void PlayerStart();
        public void PlayerUpdate();
        public void OnTrigger();
        public bool IsGuardOnTrigger();
        public void OnEnter();
        public void OnUpdate();
        public void OnExit();

        //public void AddArea(WallArea wallArea);
        //public void DeleteArea(WallArea wallArea);
    }

    [System.Serializable]
    public class DefaultZipLine : I_ZipLine
    {
        #region ïœêî

        // PlayerÇÃComponent
        //private Rigidbody rbody;

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
        private bool canRideChange = false;
        private bool isRide = false;
        private bool isDirectionPlus = true;

        // èÊÇËénÇﬂÇÈÇ∆Ç´ÇÃãóó£ä÷åW
        [SerializeField, ReadOnly] private float nearDistance = 0f;
        [SerializeField] private float rideRange = 10f;

        // åªç›égÇÌÇÍÇƒÇ¢Ç»Ç¢(SplineNearestPosíºèëÇ´)
        private Vector3 rideRangeCenterPos = new Vector3(0f, 1.2f, 0f);
        private Vector3 offsetRideRangeCenterPos;

        // çdíºéûä‘
        [SerializeField] private float startWaitTime = 2f;
        [SerializeField] private float endWaitTime = 2f;
        private float nowWaitTime = 0f;

        // speedä÷åW
        [SerializeField] private float speed = 5f;
        [SerializeField, ReadOnly] private float rideElapsedTime = 0f;
        private const float stopTime = 1.0f;

        // 
        [SerializeField, ReadOnly] private float nowRate;
        [SerializeField] private float dirFreezeLength = 10f;
        [SerializeField] private float edgeStartLength = 1f;
        [SerializeField] private float edgeEndLength = 1f;

        // íÕÇﬁà íu
        [SerializeField] private Vector3 playerHandPosition;
        private Vector3 offsetPlayerPos;

        // 
        [SerializeField] private bool isFreezeRotation = false;
        [SerializeField] private float jumpPowerY = 10f;
        [SerializeField] private float jumpPowerXZ = 10f;


        [SerializeField] float JUMP_HIGHT = 2;

        #endregion


        #region PlayerStart

        public virtual void PlayerStart()
        {
            //rbody = instance.gameObject.GetComponent<Rigidbody>();
  
            // ìríÜÇ≈ëÂÇ´Ç≥ïœÇÌÇÁÇ»Ç¢Ç»ÇÁStart
            offsetPlayerPos.x = playerHandPosition.x * instance.transform.lossyScale.x;
            offsetPlayerPos.y = playerHandPosition.y * instance.transform.lossyScale.y;
            offsetPlayerPos.z = playerHandPosition.z * instance.transform.lossyScale.z;

            offsetRideRangeCenterPos.x = rideRangeCenterPos.x * instance.transform.lossyScale.x;
            offsetRideRangeCenterPos.y = rideRangeCenterPos.y * instance.transform.lossyScale.y;
            offsetRideRangeCenterPos.z = rideRangeCenterPos.z * instance.transform.lossyScale.z;

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

        #endregion


        #region PlayerUpdate

        public virtual void PlayerUpdate()
        {
            NearZipLineUpdate();

            bool temp = canRideChange;
            canRideChange = CanRideChangeUpdate();

            
            if (temp == false && canRideChange == true)
            {
                LibButtonUIInfoManager.PopIcon(ButtonType.ZipLine);
            }
            else if (temp == true && canRideChange == false)
            {
                LibButtonUIInfoManager.RemoveIcon(ButtonType.ZipLine);
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

            if (splinePos.Length != 0)
            {
                nearSplinePos = splinePos[nearSplineNumber];
                nearSplineContainer = splineContainer[nearSplineNumber];
                nearSplinePath = splinePath[nearSplineNumber];
                nearSplineLength = splineLength[nearSplineNumber];
            }
        }

        private bool CanRideChangeUpdate()
        {
            if (IsRideRangeChangeUpdate() && CanPushButtonUpdate()) return true;

            return false;
        }

        private bool IsRideRangeChangeUpdate()
        {
            return nearDistance < rideRange;
        }

        private bool CanPushButtonUpdate()
        {
            if (nowWaitTime < 0) return true;

            nowWaitTime -= Time.deltaTime;
            return false;
        }

        #endregion


        #region OnTrigger

        public virtual void OnTrigger()
        {
            if (isRide == false)
            {
                instance._animator.SetBool(instance._animIDZipLine, true);
                CustomEvent.Trigger(instance.gameObject, "useZipLine");
            }
            else
            {
                instance._animator.SetBool(instance._animIDZipLine, false);
                CustomEvent.Trigger(instance.gameObject, "endZipLine");
            }
        }

        #endregion


        #region IsGuardOnTrigger

        public virtual bool IsGuardOnTrigger()
        {
            if (canRideChange == false) return true;
            return false;
        }

        #endregion


        #region OnEnter

        public virtual void OnEnter()
        {
            isRide = true;
            //rbody.isKinematic = true;
            nowRate = Mathf.Clamp(nearSplinePos.rate, RangeToRate(edgeStartLength), 1f - RangeToRate(edgeStartLength));

            nowWaitTime = startWaitTime;
            LibButtonUIInfoManager.RemoveIcon(ButtonType.ZipLine);

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

            // 0.01f ... è≠ÇµëOï˚
            Vector3 dir = nearSplineContainer.EvaluatePosition(nearSplinePath, nowRate + 0.01f) - nearSplineContainer.EvaluatePosition(nearSplinePath, nowRate);
            float angle = Vector3.Angle(instance.transform.forward.normalized, dir.normalized);
            isDirectionPlus = angle <= 90;
        }

        #endregion


        #region OnUpdate

        public virtual void OnUpdate()
        {
            if (EdgeEndLengthCheck() == true) return;

            NowRateUpdate();
            MoveRotation();
            MovePos();
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
                instance._animator.SetBool(instance._animIDZipLine, false);
                CustomEvent.Trigger(instance.gameObject, "endZipLine");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void NowRateUpdate()
        {
            rideElapsedTime += Time.deltaTime;
            if (rideElapsedTime < stopTime)
            {
                return;
            }

            if (isDirectionPlus == true)
            {
                nowRate += (speed / nearSplineLength) * Time.deltaTime;
            }
            else
            {
                nowRate -= (speed / nearSplineLength) * Time.deltaTime;
            }
        }

        private void MoveRotation()
        {
            Vector3 forward = nearSplineContainer.EvaluateTangent(nearSplinePath, nowRate);
            if (isDirectionPlus != true) forward *= -1f;

            Vector3 up = nearSplineContainer.EvaluateUpVector(nearSplinePath, nowRate);

            instance.transform.rotation = Quaternion.LookRotation(forward, up);

            if (isFreezeRotation == true)
            {
                instance.transform.rotation = Quaternion.Euler(0, instance.transform.localEulerAngles.y, 0);
            }
        }

        private void MovePos()
        {
            instance.transform.position = nearSplineContainer.EvaluatePosition(nearSplinePath, nowRate);

            float rotX = instance.transform.localEulerAngles.x;
            float rotY = instance.transform.localEulerAngles.y;
            float rotZ = instance.transform.localEulerAngles.z;

            Quaternion rot = Quaternion.Euler(rotX, rotY, rotZ);
            instance.transform.position -= rot * offsetPlayerPos;
        }

        #endregion


        #region OnExit

        public virtual void OnExit()
        {
            isRide = false;
            //rbody.isKinematic = false;
            instance.transform.rotation = Quaternion.Euler(0, instance.transform.localEulerAngles.y, 0);
            rideElapsedTime = 0f;

            nowWaitTime = endWaitTime;
            LibButtonUIInfoManager.RemoveIcon(ButtonType.ZipLine);
            

            EndZipLineJump();
        }

        private void EndZipLineJump()
        {
            //Vector3 tempForward = instance.transform.forward.normalized;
            //ctor3 jumpPowe = tempForward * jumpPowerXZ + Vector3.up * jumpPowerY;
            //rbody.AddForce(jumpPowe, ForceMode.Impulse);
            instance._verticalVelocity = Mathf.Sqrt(JUMP_HIGHT * -2f * instance.GRAVITY);
        }

        #endregion


        #region Other

        private float RangeToRate(float num)
        {
            return Mathf.Clamp01(num / nearSplineLength);
        }

        #endregion
    }
}