using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public partial class Player : SingletonActionListener<Player>
{
    public interface I_Climbing
    {
        public void CanUseCheck();
        public bool IsGuard();
        public void OnEnter();
        public void Climbing();
        public void OnExit();
        public void OnTrigger();
        public void AddArea(WallArea wallArea);
        public void DeleteArea(WallArea wallArea);
    }

    [System.Serializable]
    public class DefaultClimbing : I_Climbing
    {
        private bool isClimging = false;
        [SerializeField] float END_JUMP_ANIM_TIME = 0.2f;
        float nowEndTime = 0;
        [SerializeField] float START_CLIM_ANIM_TIME = 0.5f;
        float startAnimTime = 0;

        //壁用
        private List<WallArea> wallAreaList = new List<WallArea>();
        private WallArea wallArea;
        [SerializeField] float CAN_USE_CLIMING_RANGE_START = 1.0f;
        [SerializeField] float CAN_USE_CLIMING_RANGE_END = 1.2f;
        bool canUse = false;
        [SerializeField] Vector3 WALL_CHECK_OFFSEST = new Vector3(0,1.65f,0.38f);

        [SerializeField] float START_MOVE_SPEED = 10.0f;
        [SerializeField] Vector3 POSISION_OFFSET = new Vector3(0, 1.65f, 0.38f);
        
        //スプライン用
        private float splineRate;
        private  NativeSpline spline;
        private float splineLength;

        //移動用
        [SerializeField] float SPLINE_MOVE_SPEED = 0.45f; 
        [SerializeField] float ROT_OFFSET = -90.0f;
        [SerializeField] float ROT_SPEED = 5.0f;
        [SerializeField] float JUMP_HIGHT = 2 ;
        [SerializeField] float CANT_CLIM_UPPOWER = 1.0f;

        //IK用変数
        [SerializeField] Vector3 RIGHT_HAND_RAY_OFFSET_STOP = new Vector3(0.2f,0,-0.1f);
        [SerializeField] Vector3 LEFT_HAND_RAY_OFFSET_STOP = new Vector3(-0.2f, 0, -0.1f);
        [SerializeField] Vector3 RIGHT_LEG_RAY_OFFSET_STOP;
        [SerializeField] Vector3 LEFT_LEG_RAY_OFFSET_STOP;
        [SerializeField] Vector3 RIGHT_HAND_POS_OFFSET_STOP = new Vector3(0,0,-0.05f);
        [SerializeField] Vector3 LEFT_HAND_POS_OFFSET_STOP = new Vector3(0, 0, -0.05f);
        [SerializeField] Vector3 RIGHT_LEG_POS_OFFSET_STOP;
        [SerializeField] Vector3 LEFT_LEG_POSY_OFFSET_STOP;
        [SerializeField] float IK_CHECK_LENGTH = 0.3f;
        [SerializeField] LayerMask LAYER_MASK;

        #region Enter

        public virtual void OnEnter()
        {
            
            SetStateData();
            SelectedWallArea();
            SetAnimator();
        }

        private void SetStateData()
        {
            isClimging = true;
            startAnimTime = START_CLIM_ANIM_TIME;
        }

        private void SelectedWallArea()
        {
            int minsNumber = -1;
            float minsDistance = float.PositiveInfinity;
            Vector3 minsNearPos = Vector3.zero;
            int length = wallAreaList.Count;
            for (int i = 0; i < length; i++)
            {

                float distance = SplineUtility.GetNearestPoint(
                    wallAreaList[i]._spline.Splines[0],
                    (instance.transform.position + WALL_CHECK_OFFSEST - wallAreaList[i]._spline.transform.position).ChangeFloat3(),
                    out float3 nearPos,
                    out float nearPosRate);

                if (minsDistance > distance)
                {
                    minsDistance = distance;
                    minsNumber = i;
                    splineRate = nearPosRate;
                }

            }

            wallArea = wallAreaList[minsNumber];
            splineLength = wallArea._spline.CalculateLength();
        }

        private void SetAnimator()
        {
            instance._animator.SetTrigger(instance._animIDClimbingStart);
        }

        #endregion

        #region Update

        public virtual void Climbing()
        {
            if (EndCheck() == true) return;

            TimeManagement();
            spline = new NativeSpline(wallArea._spline.Spline, wallArea._spline.transform.localToWorldMatrix);
            Vector3 nowSplinePos = GetNowSplinePos();
            Vector3 moveDir = GetMoveDir();
            Vector3 movePos = CalNextPos(nowSplinePos, moveDir);
            MoveManagement(movePos);
            Rot();
            SetAnimatorIK(movePos, moveDir);
        }

        private bool EndCheck()
        {
            if (nowEndTime <= 0) return false;

            nowEndTime -= Time.deltaTime;
            if (nowEndTime <= 0)
            {
                CustomEvent.Trigger(instance.gameObject, "inClimingJump");
                instance._verticalVelocity = Mathf.Sqrt(JUMP_HIGHT * -2f * instance.GRAVITY);
            }

            return true;
        }

        private void TimeManagement()
        {
            if(startAnimTime > 0)
            {
                startAnimTime -= Time.deltaTime;
            }
        }

        private Vector3 GetNowSplinePos()
        {
            return SplineUtility.EvaluatePosition(spline, splineRate).ChangeVector3();
        }

        private Vector3 GetMoveDir()
        {
            if (instance.playerMove.magnitude < 0.1f) return Vector3.zero;

            Vector3 moveDir = LibTransform.RotationDirOfObjectFront(instance._mainCamera.transform, instance.playerMove);
            return moveDir;
        }

        private Vector3 CalNextPos(Vector3 nowSplinePos, Vector3 moveDir)
        {
            if (moveDir.magnitude == 0) return nowSplinePos;

            float dir = LibTransform.HolizontalElementOfForwardToDir(instance.transform.forward, moveDir);
            splineRate += SPLINE_MOVE_SPEED * Time.deltaTime * dir / splineLength;
            splineRate = LibMath.CastLimit(splineRate, 0, 1);

            return spline.EvaluatePosition(splineRate).ChangeVector3();
        }

        private void MoveManagement(Vector3 movePos)
        {

            Vector2 dir2D = new Vector2(POSISION_OFFSET.x, POSISION_OFFSET.z);
            Vector3 offset = LibTransform.RotationDirOfObjectFront(instance.transform, dir2D) * dir2D.magnitude;
            offset.y += POSISION_OFFSET.y;
            instance.transform.MoveFocusSpeed(movePos - offset, START_MOVE_SPEED*Time.deltaTime);
        }

        private void Rot()
        {
            spline.Evaluate(splineRate, out float3 position, out float3 tangent, out float3 upVector);
            Player.instance.transform.RotFocusSpeed(Quaternion.LookRotation(tangent, Vector3.up) * Quaternion.Euler(0, ROT_OFFSET, 0), ROT_SPEED);
            Player.instance.transform.eulerAngles= Player.instance.transform.eulerAngles.Only_Y();
        }

        private void SetAnimatorIK(Vector3 movePos, Vector3 moveDir)
        {
            float dir = (moveDir.magnitude == 0) ? 0 : LibTransform.HolizontalElementOfForwardToDir(instance.transform.forward, moveDir);
            instance._animator.SetFloat(instance._animIDClimbing_x, dir);

            if (Mathf.Abs(dir )< 0.1f)
            {
                instance.rightHandIKPosition = IKRay(movePos, RIGHT_HAND_RAY_OFFSET_STOP) + LibTransform.RotationDirOfObjectFront(instance.transform, RIGHT_HAND_POS_OFFSET_STOP) * RIGHT_HAND_POS_OFFSET_STOP.magnitude;//右手
                instance.leftHandIKPosition = IKRay(movePos, LEFT_HAND_RAY_OFFSET_STOP) + LibTransform.RotationDirOfObjectFront(instance.transform, LEFT_HAND_POS_OFFSET_STOP) * LEFT_HAND_POS_OFFSET_STOP.magnitude; ;//左手
                instance.rightLegIKPosition = IKRay(movePos, RIGHT_LEG_RAY_OFFSET_STOP) + LibTransform.RotationDirOfObjectFront(instance.transform, RIGHT_LEG_POS_OFFSET_STOP) * RIGHT_LEG_POS_OFFSET_STOP.magnitude; ;//右足
                instance.leftLegIKPosition = IKRay(movePos, LEFT_LEG_RAY_OFFSET_STOP) + LibTransform.RotationDirOfObjectFront(instance.transform, LEFT_LEG_POSY_OFFSET_STOP) * LEFT_LEG_POSY_OFFSET_STOP.magnitude; ;//左足
            }
        }

        private Vector3 IKRay(Vector3 movePos, Vector3 offset)
        {
            Vector3 rayStartPos = movePos + LibTransform.RotationDirOfObjectFront(instance.transform, offset) * offset.magnitude;
            Vector3 rayEndPos = rayStartPos + instance.transform.forward * IK_CHECK_LENGTH;
            RaycastHit hit = LibPhysics.Raycast(rayStartPos, instance.transform.forward, IK_CHECK_LENGTH, LAYER_MASK);
            return (hit.IsHit() == true) ? hit.point: movePos;
        }



        #endregion

        #region OnExit

        public virtual void OnExit()
        {
            
            ResetStateData();
            ClearWallArea();
            ResetIKPosition();
        }

        private void ResetStateData()
        {
            isClimging = false;
            nowEndTime = 0;
            startAnimTime = 0;
        }

        private void ClearWallArea()
        {
            wallArea = null;
            splineRate = 0;
            splineLength = 0;
        }

        private void ResetIKPosition()
        {
            instance.rightHandIKPosition = Vector3.zero;
            instance.leftHandIKPosition = Vector3.zero;
        }

        #endregion

        #region OnTrigger

        public virtual void OnTrigger()
        {
            if (instance._verticalVelocity > CANT_CLIM_UPPOWER) return;
            if (instance.isGrounded == true && instance._jumpTimeoutDelta > 0.0f) return;

            if (isClimging == false)
            {
                StartClimbing();
            }
            else if(startAnimTime <= 0)
            {
                EndClimbing();
            }
        }

        private void StartClimbing()
        {
            //TODO:トリガーのやつパクる
            
            CustomEvent.Trigger(instance.gameObject, "ClimbingStart");
        }

        private void EndClimbing()
        {
            if (nowEndTime > 0) return;

            instance._animator.SetTrigger(instance._animIDClimbingDown);
            nowEndTime = END_JUMP_ANIM_TIME;
        }
        #endregion

        public virtual bool IsGuard()
        {
            if (canUse == false) return true;
            return false;
        }

        public virtual void AddArea(WallArea wallArea)
        {
            wallAreaList.Add(wallArea);
        }

        public virtual void DeleteArea(WallArea wallArea)
        {
            wallAreaList.Remove(wallArea);
            if(wallAreaList.Count == 0)
            {
                LibButtonUIInfoManager.RemoveIcon(ButtonType.Climbing);
                canUse = false;
            }
        }

        #region CanUseCheck

        public virtual void CanUseCheck()
        {
            if (isClimging == true) return;
            if (wallAreaList.Count == 0) return;


            if(canUse == false)
            {
                InAreaCheck();
            }
            else
            {
                OutAreaCheck();
            }

            
        }

        private void InAreaCheck()
        {
            foreach (WallArea wall in wallAreaList)
            {
                NativeSpline spline = new NativeSpline(wall._spline.Spline, wall._spline.transform.localToWorldMatrix);
                float distance = SplineUtility.GetNearestPoint(
                    spline,
                    instance.transform.position + WALL_CHECK_OFFSEST,
                    out float3 nearPos,
                    out float nearPosRate);

                if (distance < CAN_USE_CLIMING_RANGE_START)
                {
                    LibButtonUIInfoManager.PopIcon(ButtonType.Climbing);
                    canUse = true;
                    return;
                }
            }
        }

        private void OutAreaCheck()
        {
            foreach (WallArea wall in wallAreaList)
            {
                NativeSpline spline = new NativeSpline(wall._spline.Spline, wall._spline.transform.localToWorldMatrix);
                float distance = SplineUtility.GetNearestPoint(
                    spline,
                    instance.transform.position + WALL_CHECK_OFFSEST,
                    out float3 nearPos,
                    out float nearPosRate);

                if (distance < CAN_USE_CLIMING_RANGE_END)
                {
                    return;
                }
            }

            LibButtonUIInfoManager.RemoveIcon(ButtonType.Climbing);
            canUse = false;
        }

        #endregion
    }
}