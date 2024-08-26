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

        public void AddArea(ZipLineArea zipLineArea);
        public void DeleteArea(ZipLineArea zipLineArea);
    }

    [System.Serializable]
    public class DefaultZipLine : I_ZipLine
    {
        #region ïœêî

        // Splineä÷åW
        [SerializeField, ReadOnly] private List<ZipLineArea> zipLineAreaList = new List<ZipLineArea>();
        [SerializeField, ReadOnly] private int nearSplineNumber = 0;
        [SerializeField, ReadOnly] private SplineNearestPos nearSplinePos;
        [SerializeField, ReadOnly] private SplineContainer nearSplineContainer;
        //[SerializeField, ReadOnly] private SplinePath<Spline> nearSplinePath;// ÇÌÇ©ÇÁÇ»Ç¢
        [SerializeField, ReadOnly] private float nearSplineLength;

        // 
        private bool canRide = false;
        private bool isRide = false;
        private bool isDirectionPlus = true;

        // èÊÇËénÇﬂÇÈÇ∆Ç´ÇÃãóó£ä÷åW
        [SerializeField, ReadOnly] private float nearDistance = 0f;
        [SerializeField] private float rideRange = 3f;
        [SerializeField] private Vector3 offsetRideCenterPos = new Vector3(0f, 1.2f, 0f);

        // çdíºéûä‘
        [SerializeField] private float startWaitTime = 0.5f;
        [SerializeField] private float endWaitTime = 0.5f;
        private float nowWaitTime = 0f;

        // speedä÷åW
        [SerializeField] private float speed = 15f;
        [SerializeField, ReadOnly] private float rideStopElapsedTime = 0f;
        [SerializeField] private float stopTime = 0.2f;
        [SerializeField] private float stopTimeRotSpeed = 10f;

        // 
        [SerializeField, ReadOnly] private float nowRate;
        [SerializeField] private float dirFreezeLength = 10f;
        [SerializeField] private float edgeStartLength = 1f;
        [SerializeField] private float edgeEndLength = 1f;

        // íÕÇﬁà íu
        [SerializeField] private Vector3 playerHandPosition = new Vector3(0f, 1.62f, 0.15f);

        // 
        [SerializeField] private bool isFreezeRotation = false;
        //[SerializeField] private float jumpPowerY = 10f;
        //[SerializeField] private float jumpPowerXZ = 10f;


        [SerializeField] float JUMP_HIGHT = 3;


        //IKópïœêî
        [SerializeField] Vector3 RIGHT_HAND = new Vector3(-1.3f, 5f, 4f);
        [SerializeField] Vector3 LEFT_HAND = new Vector3(1f, 5f, 3f);
        [SerializeField] Vector3 RIGHT_LEG = new Vector3(0.06f, -1.15f, 0f);
        [SerializeField] Vector3 LEF_LEGD = new Vector3(-0.06f, -1.2f, 0f);



        #endregion


        #region PlayerStart

        public virtual void PlayerStart()
        {
            // ìríÜÇ≈ëÂÇ´Ç≥ïœÇÌÇÁÇ»Ç¢Ç»ÇÁStart
            offsetRideCenterPos.x *= instance.transform.lossyScale.x;
            offsetRideCenterPos.y *= instance.transform.lossyScale.y;
            offsetRideCenterPos.z *= instance.transform.lossyScale.z;
        }

        #endregion


        #region PlayerUpdate

        public virtual void PlayerUpdate()
        {
            DistanceZipLineUpdate();
            NearZipLineUpdate();

            bool temp = canRide;
            canRide = CanRideChangeUpdate();

            
            if (temp == false && canRide == true)
            {
                LibButtonUIInfoManager.PopIcon(ButtonType.ZipLine);
            }
            else if (temp == true && canRide == false)
            {
                LibButtonUIInfoManager.RemoveIcon(ButtonType.ZipLine);
            }
        }

        private void DistanceZipLineUpdate()
        {
            for (int i = 0; i < zipLineAreaList.Count; i++)
            {
                zipLineAreaList[i].splinePos.DistanceUpdate(offsetRideCenterPos);
            }
        }

        private void NearZipLineUpdate()
        {
            nearDistance = Mathf.Infinity;

            for (int i = 0; i < zipLineAreaList.Count; i++)
            {
                if (zipLineAreaList[i].splinePos.distance < nearDistance)
                {
                    nearDistance = zipLineAreaList[i].splinePos.distance;
                    nearSplineNumber = i;
                }
            }

            if (zipLineAreaList.Count != 0)
            {
                nearSplinePos = zipLineAreaList[nearSplineNumber].splinePos;
                nearSplineContainer = zipLineAreaList[nearSplineNumber].splineContainer;
                //nearSplinePath = zipLineAreaList[nearSplineNumber].splinePath;
                nearSplineLength = zipLineAreaList[nearSplineNumber].splineLength;
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
            if (canRide == false) return true;
            return false;
        }

        #endregion


        #region OnEnter

        public virtual void OnEnter()
        {
            isRide = true;
            nowRate = Mathf.Clamp(nearSplinePos.rate, RangeToRate(edgeStartLength), 1f - RangeToRate(edgeStartLength));

            rideStopElapsedTime = stopTime;
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
            Vector3 dir = nearSplineContainer.EvaluatePosition(nowRate + 0.01f) - nearSplineContainer.EvaluatePosition(nowRate);
            float angle = Vector3.Angle(instance.transform.forward.normalized, dir.normalized);
            isDirectionPlus = angle <= 90;
        }

        #endregion


        #region OnUpdate

        public virtual void OnUpdate()
        {
            if (EdgeEndLengthCheck() == true) return;

            if(NowRateUpdate() == false) return;

            MoveRotation();
            MovePos();
            MoveIKPos();
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

        private bool NowRateUpdate()
        {
            if (rideStopElapsedTime <= 0)
            {
                if (isDirectionPlus == true)
                {
                    nowRate += (speed / nearSplineLength) * Time.deltaTime;
                }
                else
                {
                    nowRate -= (speed / nearSplineLength) * Time.deltaTime;
                }

                return true;
            }


            instance.transform.RotFocusSpeed(MoveQuaternion(), stopTimeRotSpeed);

            instance.transform.MoveFocusTime((Vector3)nearSplineContainer.EvaluatePosition(nowRate) - OffsetPlayerPos(), ref rideStopElapsedTime);

            MoveIKPos();

            return false;
        }

        private void MoveRotation()
        {
            instance.transform.rotation = MoveQuaternion();

            if (isFreezeRotation == true)
            {
                instance.transform.rotation = Quaternion.Euler(0, instance.transform.localEulerAngles.y, 0);
            }
        }

        private Quaternion MoveQuaternion()
        {
            Vector3 forward = nearSplineContainer.EvaluateTangent(nowRate);
            if (isDirectionPlus != true) forward *= -1f;

            Vector3 up = nearSplineContainer.EvaluateUpVector(nowRate);

            Quaternion quaternion = Quaternion.LookRotation(forward, up);

            return quaternion;
        }

        private void MovePos()
        {
            instance.transform.position = nearSplineContainer.EvaluatePosition(nowRate);

            instance.transform.position -= OffsetPlayerPos();
        }

        private Vector3 OffsetPlayerPos()
        {
            Quaternion rot = Quaternion.Euler(instance.transform.localEulerAngles);

            Vector3 offsetPlayerPos;
            offsetPlayerPos.x = playerHandPosition.x * instance.transform.lossyScale.x;
            offsetPlayerPos.y = playerHandPosition.y * instance.transform.lossyScale.y;
            offsetPlayerPos.z = playerHandPosition.z * instance.transform.lossyScale.z;

            
            return rot * offsetPlayerPos;
        }

        private void MoveIKPos()
        {
            Quaternion rot = Quaternion.Euler(instance.transform.localEulerAngles);

            Vector3 temp = instance.transform.position + OffsetPlayerPos();

            instance.rightHandIKPosition = (Vector3.zero == RIGHT_HAND) ? Vector3.zero : temp + rot * RIGHT_HAND;
            instance.leftHandIKPosition = (Vector3.zero == LEFT_HAND) ? Vector3.zero : temp + rot * LEFT_HAND;
            instance.rightLegIKPosition = (Vector3.zero == RIGHT_LEG) ? Vector3.zero : temp + rot * RIGHT_LEG;
            instance.leftLegIKPosition = (Vector3.zero == LEF_LEGD) ? Vector3.zero : temp + rot * LEF_LEGD;
        }

        #endregion


        #region OnExit

        public virtual void OnExit()
        {
            isRide = false;
            instance.transform.rotation = Quaternion.Euler(0, instance.transform.localEulerAngles.y, 0);
            rideStopElapsedTime = 0f;

            nowWaitTime = endWaitTime;
            LibButtonUIInfoManager.RemoveIcon(ButtonType.ZipLine);

            //IKPosReset();
            EndZipLineJump();
        }

        private void IKPosReset()
        {
            instance.rightHandIKPosition = Vector3.zero;
            instance.leftHandIKPosition = Vector3.zero;
            instance.rightLegIKPosition = Vector3.zero;
            instance.leftLegIKPosition = Vector3.zero;
        }

        private void EndZipLineJump()
        {
            //Vector3 tempForward = instance.transform.forward.normalized;
            //Vector3 jumpPowe = tempForward * jumpPowerXZ + Vector3.up * jumpPowerY;
            //rbody.AddForce(jumpPowe, ForceMode.Impulse);
            instance._verticalVelocity = Mathf.Sqrt(JUMP_HIGHT * -2f * instance.GRAVITY);
        }

        #endregion


        #region AddArea

        public virtual void AddArea(ZipLineArea zipLineArea)
        {
            zipLineAreaList.Add(zipLineArea);
        }

        #endregion


        #region DeleteArea

        public virtual void DeleteArea(ZipLineArea zipLineArea)
        {
            zipLineAreaList.Remove(zipLineArea);
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