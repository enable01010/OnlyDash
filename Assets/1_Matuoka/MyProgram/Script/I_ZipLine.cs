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
        #region 変数

        // PlayerのComponent
        //private Rigidbody rbody;

        // Spline関係
        private SplineNearestPos[] splinePos;// 各SplineのPlayerと一番近い位置
        private SplineContainer[] splineContainer;// SplineのComponent
        private SplinePath<Spline>[] splinePath;// わからない
        private float[] splineLength;// Splineの長さ

        [SerializeField, ReadOnly] private int nearSplineNumber = 0;
        [SerializeField, ReadOnly] private SplineNearestPos nearSplinePos;
        [SerializeField, ReadOnly] private SplineContainer nearSplineContainer;
        [SerializeField, ReadOnly] private SplinePath<Spline> nearSplinePath;// わからない
        [SerializeField, ReadOnly] private float nearSplineLength;

        // 
        private bool isRide = false;
        private bool isDirectionPlus = true;

        // 乗り始めるときの距離関係
        [SerializeField, ReadOnly] private bool isRideRange = false;
        [SerializeField, ReadOnly] private float nearDistance = 0f;
        [SerializeField] private float rideRange = 10f;

        // speed関係
        [SerializeField] private float speed = 5f;
        private float oneFrameRate;
        [SerializeField, ReadOnly] private float rideElapsedTime = 0f;
        private const float stopTime = 1.0f;

        // 
        [SerializeField, ReadOnly] private float nowRate;
        [SerializeField] private float dirFreezeLength = 10f;
        [SerializeField] private float edgeStartLength = 3f;
        [SerializeField] private float edgeEndLength = 3f;

        // 掴む位置
        [SerializeField] private Transform playerHandPos;
        private Vector3 offsetPlayerPos;

        // 
        [SerializeField] private bool isFreezeRotation = false;
        [SerializeField] private float jumpPowerY = 10f;
        [SerializeField] private float jumpPowerXZ = 10f;

        // UI
        [SerializeField] private GameObject canvas;

        #endregion


        #region PlayerStart

        public virtual void PlayerStart()
        {
            //rbody = instance.gameObject.GetComponent<Rigidbody>();

            // 途中で大きさ変わらないならStart
            offsetPlayerPos.x = playerHandPos.transform.localPosition.x * instance.transform.lossyScale.x;
            offsetPlayerPos.y = playerHandPos.transform.localPosition.y * instance.transform.lossyScale.y;
            offsetPlayerPos.z = playerHandPos.transform.localPosition.z * instance.transform.lossyScale.z;

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

            bool temp = isRideRange;
            isRideRange = nearDistance < rideRange;
            if (temp == false && isRideRange == true)
            {
                // Addに書き換え！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
                canvas.SetActive(true);
            }
            else if (temp == true && isRideRange == false)
            {
                // Deleteに書き換え！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
                canvas.SetActive(false);
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

        #endregion


        #region OnTrigger

        public virtual void OnTrigger()
        {
            if (isRide == false)
            {
                CustomEvent.Trigger(instance.gameObject, "useZipLine");
            }
            else
            {
                CustomEvent.Trigger(instance.gameObject, "endZipLine");
            }
        }

        #endregion


        #region IsGuardOnTrigger

        public virtual bool IsGuardOnTrigger()
        {
            if (isRideRange == false) return true;
            return false;
        }

        #endregion


        #region OnEnter

        public virtual void OnEnter()
        {
            isRide = true;
            //rbody.isKinematic = true;
            nowRate = Mathf.Clamp(nearSplinePos.rate, RangeToRate(edgeStartLength), 1f - RangeToRate(edgeStartLength));

            StartZipLineOneFrameRate();
            StartZipLineDirection();
        }

        private void StartZipLineOneFrameRate()
        {
            oneFrameRate = (speed / nearSplineLength) * Time.fixedDeltaTime;
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

            EndZipLineJump();
        }

        private void EndZipLineJump()
        {
            Vector3 tempForward = instance.transform.forward.normalized;
            Vector3 jumpPowe = tempForward * jumpPowerXZ + Vector3.up * jumpPowerY;
            //rbody.AddForce(jumpPowe, ForceMode.Impulse);
        }

        #endregion


        #region Other

        private float RangeToRate(float num)
        {
            return Mathf.Clamp01(num / nearSplineLength);
        }

        #endregion


        #region 松岡

        #region Fields

        //// PlayerのComponent
        //private Rigidbody rbody;

        //// Spline関係
        //private SplineNearestPos[] splinePos;// 各SplineのPlayerと一番近い位置
        //private SplineContainer[] splineContainer;// SplineのComponent
        //private SplinePath<Spline>[] splinePath;// わからない
        //private float[] splineLength;// Splineの長さ

        //[SerializeField, ReadOnly] private int nearSplineNumber = 0;
        //[SerializeField, ReadOnly] private SplineNearestPos nearSplinePos;
        //[SerializeField, ReadOnly] private SplineContainer nearSplineContainer;
        //[SerializeField, ReadOnly] private SplinePath<Spline> nearSplinePath;// わからない
        //[SerializeField, ReadOnly] private float nearSplineLength;

        //// 
        //private bool isInputKey = false;
        //private bool isRide = false;
        //private bool isDirectionPlus = true;

        //// 乗り始めるときの距離関係
        //[SerializeField, ReadOnly] private bool isRideRange = false;
        //[SerializeField, ReadOnly] private float nearDistance = 0f;
        //[SerializeField] private float rideRange = 10f;

        //// speed関係
        //[SerializeField] private float speed = 5f;
        //private float oneFrameRate;
        //[SerializeField, ReadOnly] private float rideElapsedTime = 0f;
        //private const float stopTime = 0.3f;

        //// 
        //[SerializeField, ReadOnly] private float nowRate;
        //[SerializeField] private float dirFreezeLength = 10f;
        //[SerializeField] private float edgeStartLength = 3f;
        //[SerializeField] private float edgeEndLength = 3f;

        //// 掴む位置
        //[SerializeField] private Transform playerHandPos;
        //private Vector3 offsetPlayerPos;

        //// 
        //[SerializeField] private bool isFreezeRotation = false;
        //[SerializeField] private float jumpPowerY = 10f;
        //[SerializeField] private float jumpPowerXZ = 10f;

        //// UI
        //[SerializeField] private GameObject canvas;

        #endregion



        #region MonoBehaviourMethod

        //private void Start()
        //{
        //    rbody = this.gameObject.GetComponent<Rigidbody>();

        //    // 途中で大きさ変わらないならStart
        //    offsetPlayerPos.x = playerHandPos.transform.localPosition.x * this.transform.lossyScale.x;
        //    offsetPlayerPos.y = playerHandPos.transform.localPosition.y * this.transform.lossyScale.y;
        //    offsetPlayerPos.z = playerHandPos.transform.localPosition.z * this.transform.lossyScale.z;

        //    GameObject[] obj = GameObject.FindGameObjectsWithTag("ZipLine");

        //    splinePos = new SplineNearestPos[obj.Length];
        //    splineContainer = new SplineContainer[obj.Length];
        //    splinePath = new SplinePath<Spline>[obj.Length];
        //    splineLength = new float[obj.Length];

        //    for (int i = 0; i < obj.Length; i++)
        //    {
        //        splinePos[i] = obj[i].GetComponent<SplineNearestPos>();

        //        splineContainer[i] = obj[i].GetComponentInChildren<SplineContainer>();
        //        splinePath[i] = new SplinePath<Spline>(splineContainer[i].Splines);
        //        splineLength[i] = splinePath[i].GetLength();
        //    }
        //}

        //private void Update()
        //{
        //    InputUpdate();
        //}

        //private void FixedUpdate()
        //{
        //    NearZipLineUpdate();
        //    RideRangeUpdate();

        //    if (StartZipLineCheck() == true) StartZipLine();
        //    if (EndZipLineCheck() == true) EndZipLine();

        //    if (isRide == true)
        //    {
        //        NowRateUpdate();
        //        MoveRotation();
        //        MovePos();
        //    }

        //    InputReset();
        //}

        #endregion


        #region CustomMethod

        //private void InputUpdate()
        //{
        //    if (Input.GetKeyDown(KeyCode.Pause) || Input.GetKeyDown("joystick button 0")/* || Input.GetKeyDown("joystick button 1")*/)
        //    {
        //        isInputKey = true;
        //    }
        //}

        //private void InputReset()
        //{
        //    isInputKey = false;
        //}

        //private void NowRateUpdate()
        //{
        //    rideElapsedTime += Time.fixedDeltaTime;
        //    if (rideElapsedTime < stopTime)
        //    {
        //        return;
        //    }

        //    if (isDirectionPlus == true)
        //    {
        //        nowRate += oneFrameRate;
        //    }
        //    else
        //    {
        //        nowRate -= oneFrameRate;
        //    }
        //}

        //private bool StartZipLineCheck()
        //{
        //    if (isInputKey == false) return false;
        //    if (isRide == true) return false;
        //    if (isRideRange == false) return false;

        //    InputReset();
        //    return true;
        //}

        //private void StartZipLine()
        //{
        //    isRide = true;
        //    rbody.isKinematic = true;
        //    nowRate = Mathf.Clamp(nearSplinePos.rate, RangeToRate(edgeStartLength), 1f - RangeToRate(edgeStartLength));

        //    StartZipLineOneFrameRate();
        //    StartZipLineDirection();
        //}

        //private void StartZipLineDirection()
        //{
        //    if (nowRate < RangeToRate(dirFreezeLength))
        //    {
        //        isDirectionPlus = true;
        //        return;
        //    }
        //    else if (nowRate > 1f - RangeToRate(dirFreezeLength))
        //    {
        //        isDirectionPlus = false;
        //        return;
        //    }

        //    Vector3 dir = nearSplineContainer.EvaluatePosition(nearSplinePath, nowRate + 0.01f) - nearSplineContainer.EvaluatePosition(nearSplinePath, nowRate);
        //    float angle = Vector3.Angle(this.transform.forward.normalized, dir.normalized);
        //    isDirectionPlus = angle <= 90;
        //}

        //private void StartZipLineOneFrameRate()
        //{
        //    oneFrameRate = (speed / nearSplineLength) * Time.fixedDeltaTime;
        //}

        //private void EndZipLine()
        //{
        //    isRide = false;
        //    rbody.isKinematic = false;
        //    this.transform.rotation = Quaternion.Euler(0, this.transform.localEulerAngles.y, 0);
        //    rideElapsedTime = 0f;

        //    EndZipLineJump();
        //}

        //private void EndZipLineJump()
        //{
        //    Vector3 tempForward = this.transform.forward.normalized;
        //    Vector3 jumpPowe = tempForward * jumpPowerXZ + Vector3.up * jumpPowerY;
        //    rbody.AddForce(jumpPowe, ForceMode.Impulse);
        //}

        //private bool EndZipLineCheck()
        //{
        //    if (isRide == false) return false;

        //    if (EdgeEndLengthCheck() == true) return true;

        //    if (isInputKey == false) return false;

        //    InputReset();
        //    return true;
        //}

        //private bool EdgeEndLengthCheck()
        //{
        //    float tempNowRate = nowRate;
        //    if (isDirectionPlus == false)
        //    {
        //        tempNowRate = 1f - tempNowRate;
        //    }

        //    if (tempNowRate > 1f - RangeToRate(edgeEndLength))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //private void NearZipLineUpdate()
        //{
        //    nearDistance = Mathf.Infinity;

        //    for (int i = 0; i < splinePos.Length; i++)
        //    {
        //        if (splinePos[i].distance < nearDistance)
        //        {
        //            nearDistance = splinePos[i].distance;
        //            nearSplineNumber = i;
        //        }
        //    }

        //    nearSplinePos = splinePos[nearSplineNumber];
        //    nearSplineContainer = splineContainer[nearSplineNumber];
        //    nearSplinePath = splinePath[nearSplineNumber];
        //    nearSplineLength = splineLength[nearSplineNumber];
        //}

        //private void RideRangeUpdate()
        //{
        //    isRideRange = nearDistance < rideRange;
        //    canvas.SetActive(isRideRange);
        //}

        //private void MoveRotation()
        //{
        //    Vector3 forward = nearSplineContainer.EvaluateTangent(nearSplinePath, nowRate);
        //    if (isDirectionPlus != true) forward *= -1f;

        //    Vector3 up = nearSplineContainer.EvaluateUpVector(nearSplinePath, nowRate);

        //    this.transform.rotation = Quaternion.LookRotation(forward, up);

        //    if (isFreezeRotation == true)
        //    {
        //        this.transform.rotation = Quaternion.Euler(0, this.transform.localEulerAngles.y, 0);
        //    }
        //}

        //private void MovePos()
        //{
        //    this.transform.position = nearSplineContainer.EvaluatePosition(nearSplinePath, nowRate);

        //    float rotX = this.transform.localEulerAngles.x;
        //    float rotY = this.transform.localEulerAngles.y;
        //    float rotZ = this.transform.localEulerAngles.z;

        //    Quaternion rot = Quaternion.Euler(rotX, rotY, rotZ);
        //    this.transform.position -= rot * offsetPlayerPos;
        //}

        //private float RangeToRate(float num)
        //{
        //    return Mathf.Clamp01(num / nearSplineLength);
        //}

        #endregion



        #endregion

        #region 谷山

        #region 変数

        //private bool isClimging = false;
        //[SerializeField] float END_JUMP_ANIM_TIME = 1.0f;
        //float nowEndTime = 0;
        //[SerializeField] float START_CLIM_ANIM_TIME = 0.5f;
        //float startAnimTime = 0;

        ////壁用
        //private List<WallArea> wallAreaList = new List<WallArea>();
        //private WallArea wallArea;

        //[SerializeField] float START_MOVE_SPEED = 5.0f;
        //[SerializeField] Vector3 POSISION_OFFSET = new Vector3(0, 0.15f, 0.15f);

        ////スプライン用
        //private float splineRate;
        //private NativeSpline spline;
        //private float splineLength;

        ////移動用
        //[SerializeField] float SPLINE_MOVE_SPEED;

        //[SerializeField] float ROT_OFFSET;
        //[SerializeField] float ROT_SPEED;
        //[SerializeField] float JUMP_HIGHT = 2;
        //[SerializeField] float CANT_CLIM_UPPOWER = 1.0f;

        ////IK用変数
        //[SerializeField] Vector3 RIGHT_HAND_RAY_OFFSET_STOP;
        //[SerializeField] Vector3 LEFT_HAND_RAY_OFFSET_STOP;
        //[SerializeField] Vector3 RIGHT_LEG_RAY_OFFSET_STOP;
        //[SerializeField] Vector3 LEFT_LEG_RAY_OFFSET_STOP;
        //[SerializeField] Vector3 RIGHT_HAND_POS_OFFSET_STOP;
        //[SerializeField] Vector3 LEFT_HAND_POS_OFFSET_STOP;
        //[SerializeField] Vector3 RIGHT_LEG_POS_OFFSET_STOP;
        //[SerializeField] Vector3 LEFT_LEG_POSY_OFFSET_STOP;
        //[SerializeField] float IK_CHECK_LENGTH = 0.3f;
        //[SerializeField] LayerMask LAYER_MASK;

        #endregion

        #region Enter

        //public virtual void OnEnter()
        //{

        //    SetStateData();
        //    SelectedWallArea();
        //    SetAnimator();
        //}

        //private void SetStateData()
        //{
        //    isClimging = true;
        //    startAnimTime = START_CLIM_ANIM_TIME;
        //}

        //private void SelectedWallArea()
        //{
        //    int minsNumber = -1;
        //    float minsDistance = float.PositiveInfinity;
        //    Vector3 minsNearPos = Vector3.zero;
        //    int length = wallAreaList.Count;
        //    for (int i = 0; i < length; i++)
        //    {

        //        float distance = SplineUtility.GetNearestPoint(
        //            wallAreaList[i]._spline.Splines[0],
        //            (instance.transform.position - wallAreaList[i]._spline.transform.position).ChangeFloat3(),
        //            out float3 nearPos,
        //            out float nearPosRate);

        //        if (minsDistance > distance)
        //        {
        //            minsDistance = distance;
        //            minsNumber = i;
        //            splineRate = nearPosRate;
        //        }

        //    }

        //    wallArea = wallAreaList[minsNumber];
        //    splineLength = wallArea._spline.CalculateLength();
        //}

        //private void SetAnimator()
        //{
        //    instance._animator.SetTrigger(instance._animIDClimbingStart);
        //}

        #endregion

        #region Update

        //public virtual void Climbing()
        //{
        //    if (EndCheck() == true) return;

        //    TimeManagement();
        //    spline = new NativeSpline(wallArea._spline.Spline, wallArea._spline.transform.localToWorldMatrix);
        //    Vector3 nowSplinePos = GetNowSplinePos();
        //    Vector3 moveDir = GetMoveDir();
        //    Vector3 movePos = CalNextPos(nowSplinePos, moveDir);
        //    MoveManagement(movePos);
        //    Rot();
        //    SetAnimatorIK(movePos, moveDir);
        //}

        //private bool EndCheck()
        //{
        //    if (nowEndTime <= 0) return false;

        //    nowEndTime -= Time.deltaTime;
        //    if (nowEndTime <= 0)
        //    {
        //        CustomEvent.Trigger(instance.gameObject, "inClimingJump");
        //        instance._verticalVelocity = Mathf.Sqrt(instance.JUMP_HEIGHT * -2f * instance.GRAVITY);
        //    }

        //    return true;
        //}

        //private void TimeManagement()
        //{
        //    if (startAnimTime > 0)
        //    {
        //        startAnimTime -= Time.deltaTime;
        //    }
        //}

        //private Vector3 GetNowSplinePos()
        //{
        //    return SplineUtility.EvaluatePosition(spline, splineRate).ChengeVector3();
        //}

        //private Vector3 GetMoveDir()
        //{
        //    if (instance.playerMove.magnitude < 0.1f) return Vector3.zero;

        //    Vector3 moveDir = LibVector.RotationDirOfObjectFront(instance._mainCamera.transform, instance.playerMove);
        //    return moveDir;
        //}

        //private Vector3 CalNextPos(Vector3 nowSplinePos, Vector3 moveDir)
        //{
        //    if (moveDir.magnitude == 0) return nowSplinePos;

        //    float dir = LibVector.HolizontalElementOfForwardToDir(instance.transform.forward, moveDir);
        //    splineRate += SPLINE_MOVE_SPEED * Time.deltaTime * dir / splineLength;
        //    splineRate = LibMath.CastLimit(splineRate, 0, 1);

        //    return spline.EvaluatePosition(splineRate).ChengeVector3();
        //}

        //private void MoveManagement(Vector3 movePos)
        //{

        //    Vector2 dir2D = new Vector2(POSISION_OFFSET.x, POSISION_OFFSET.z);
        //    Vector3 offset = LibVector.RotationDirOfObjectFront(instance.transform, dir2D) * dir2D.magnitude;
        //    offset.y += POSISION_OFFSET.y;
        //    instance.transform.MoveFocusSpeed(movePos - offset, START_MOVE_SPEED * Time.deltaTime);
        //}

        //private void Rot()
        //{
        //    spline.Evaluate(splineRate, out float3 position, out float3 tangent, out float3 upVector);
        //    Player.instance.transform.RotFocusSpeed(Quaternion.LookRotation(tangent, Vector3.up) * Quaternion.Euler(0, ROT_OFFSET, 0), ROT_SPEED);
        //}

        //private void SetAnimatorIK(Vector3 movePos, Vector3 moveDir)
        //{
        //    float dir = (moveDir.magnitude == 0) ? 0 : LibVector.HolizontalElementOfForwardToDir(instance.transform.forward, moveDir);
        //    instance._animator.SetFloat(instance._animIDClimbing_x, dir);

        //    if (Mathf.Abs(dir) < 0.1f || true)
        //    {
        //        instance.rightHandIKPosition = IKRay(movePos, RIGHT_HAND_RAY_OFFSET_STOP) + LibVector.RotationDirOfObjectFront(instance.transform, RIGHT_HAND_POS_OFFSET_STOP) * RIGHT_HAND_POS_OFFSET_STOP.magnitude;//右手
        //        instance.leftHandIKPosition = IKRay(movePos, LEFT_HAND_RAY_OFFSET_STOP) + LibVector.RotationDirOfObjectFront(instance.transform, LEFT_HAND_POS_OFFSET_STOP) * LEFT_HAND_POS_OFFSET_STOP.magnitude; ;//左手
        //        instance.rightLegIKPosition = IKRay(movePos, RIGHT_LEG_RAY_OFFSET_STOP) + LibVector.RotationDirOfObjectFront(instance.transform, RIGHT_LEG_POS_OFFSET_STOP) * RIGHT_LEG_POS_OFFSET_STOP.magnitude; ;//右足
        //        instance.leftLegIKPosition = IKRay(movePos, LEFT_LEG_RAY_OFFSET_STOP) + LibVector.RotationDirOfObjectFront(instance.transform, LEFT_LEG_POSY_OFFSET_STOP) * LEFT_LEG_POSY_OFFSET_STOP.magnitude; ;//左足
        //    }
        //    //else
        //    //{
        //    //    instance.rightHandIKPosition = IKRay(instance._animator.GetIKPosition(AvatarIKGoal.RightHand), RIGHT_HAND_RAY_OFFSET_STOP) + LibVector.RotationDirOfObjectFront(instance.transform, RIGHT_HAND_POS_OFFSET_STOP) * RIGHT_HAND_POS_OFFSET_STOP.magnitude;//右手
        //    //    instance.leftHandIKPosition = IKRay(instance._animator.GetIKPosition(AvatarIKGoal.LeftHand), LEFT_HAND_RAY_OFFSET_STOP) + LibVector.RotationDirOfObjectFront(instance.transform, LEFT_HAND_POS_OFFSET_STOP) * LEFT_HAND_POS_OFFSET_STOP.magnitude; ;//左手
        //    //    instance.rightLegIKPosition = IKRay(movePos, RIGHT_LEG_RAY_OFFSET_STOP) + LibVector.RotationDirOfObjectFront(instance.transform, RIGHT_LEG_POS_OFFSET_STOP) * RIGHT_LEG_POS_OFFSET_STOP.magnitude; ;//右足
        //    //    instance.leftLegIKPosition = IKRay(movePos, LEFT_LEG_RAY_OFFSET_STOP) + LibVector.RotationDirOfObjectFront(instance.transform, LEFT_LEG_POSY_OFFSET_STOP) * LEFT_LEG_POSY_OFFSET_STOP.magnitude; ;//左足
        //    //}
        //}

        //private Vector3 IKRay(Vector3 movePos, Vector3 offset)
        //{
        //    Vector3 rayStartPos = movePos + LibVector.RotationDirOfObjectFront(instance.transform, offset) * offset.magnitude;
        //    Vector3 rayEndPos = rayStartPos + instance.transform.forward * IK_CHECK_LENGTH;
        //    RaycastHit hit = LibPhysics.Raycast(rayStartPos, instance.transform.forward, IK_CHECK_LENGTH, LAYER_MASK);
        //    return (hit.IsHit() == true) ? hit.point : movePos;
        //}



        #endregion

        #region OnExit

        //public virtual void OnExit()
        //{

        //    ResetStateData();
        //    ClearWallArea();
        //    ResetIKPosition();
        //}

        //private void ResetStateData()
        //{
        //    isClimging = false;
        //    nowEndTime = 0;
        //    startAnimTime = 0;
        //}

        //private void ClearWallArea()
        //{
        //    wallArea = null;
        //    splineRate = 0;
        //    splineLength = 0;
        //}

        //private void ResetIKPosition()
        //{
        //    instance.rightHandIKPosition = Vector3.zero;
        //    instance.leftHandIKPosition = Vector3.zero;
        //}

        #endregion

        #region OnTrigger

        //public virtual void OnTrigger()
        //{
        //    if (instance._verticalVelocity > CANT_CLIM_UPPOWER) return;
        //    if (instance.isGrounded == true && instance._jumpTimeoutDelta > 0.0f) return;

        //    if (isClimging == false)
        //    {
        //        StartClimbing();
        //    }
        //    else if (startAnimTime <= 0)
        //    {
        //        EndClimbing();
        //    }
        //}

        //private void StartClimbing()
        //{
        //    //TODO:トリガーのやつパクる

        //    CustomEvent.Trigger(instance.gameObject, "ClimbingStart");
        //}

        //private void EndClimbing()
        //{
        //    if (nowEndTime > 0) return;

        //    instance._animator.SetTrigger(instance._animIDClimbingDown);
        //    nowEndTime = END_JUMP_ANIM_TIME;
        //}
        #endregion

        #region IsGuard

        //public virtual bool IsGuard()
        //{
        //    if (wallAreaList.Count == 0) return true;
        //    return false;
        //}

        //public virtual void AddArea(WallArea wallArea)
        //{
        //    wallAreaList.Add(wallArea);
        //}

        //public virtual void DeleteArea(WallArea wallArea)
        //{
        //    wallAreaList.Remove(wallArea);
        //}

        #endregion


        #endregion
    }
}