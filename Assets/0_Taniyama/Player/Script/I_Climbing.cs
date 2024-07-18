using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public partial class Player : SingletonActionListener<Player>
{
    public interface I_Climbing
    {
        public bool IsGuard();
        public void OnEnter();
        public void Climbing();
        public void OnExit();
        public void AddArea(WallArea wallArea);
        public void DeleteArea(WallArea wallArea);
    }

    [System.Serializable]
    public class DefaultClimbing : I_Climbing
    {
        //壁用
        private List<WallArea> wallAreaList = new List<WallArea>();
        private WallArea wallArea;

        [SerializeField] float START_MOVE_SPEED = 5.0f;
        [SerializeField] Vector3 POSISION_OFFSET = new Vector3(0, 0.15f, 0.15f);
        
        //スプライン用
        private float splineRate;
        private  NativeSpline spline;
        private float splineLength;

        //移動用
        [SerializeField] float SPLINE_MOVE_SPEED;
        [SerializeField] Vector3 RIGHT_HAND_OFFSET;
        [SerializeField] Vector3 LEFT_HAND_OFFSET;
        [SerializeField] float ROT_OFFSET;
        [SerializeField] float ROT_SPEED;

        public virtual bool IsGuard()
        {
            if (wallAreaList.Count == 0) return true;
            return false;
        }
        public virtual void OnEnter()
        {
            SelectedWallArea();
            SetAnimator();
        }
        public virtual void Climbing()
        {
            spline = new NativeSpline(wallArea._spline.Spline, wallArea._spline.transform.localToWorldMatrix);
            Vector3 nowSplinePos = GetNowSplinePos();
            Vector3 moveDir = GetMoveDir();
            Vector3 movePos = CalNextPos(nowSplinePos, moveDir);
            MoveManagement(movePos);
            Rot();
            SetAnimatorIK(movePos, moveDir);
        }
        public virtual void OnExit()
        {
            ClearWallArea();
            ResetIKPosition();
            splineRate = 0;
        }

        public virtual void AddArea(WallArea wallArea)
        {
            wallAreaList.Add(wallArea);
        }

        public virtual void DeleteArea(WallArea wallArea)
        {
            wallAreaList.Remove(wallArea);
        }

        /// <summary>
        /// WallAreaListの中から一番近いWallAreaを判定する
        /// </summary>
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
                    (instance.transform.position - wallAreaList[i]._spline.transform.position).ChangeFloat3(),
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
        private void ClearWallArea()
        {
            wallArea = null;
        }

        private void SetAnimator()
        {
            Player instance = Player.instance;
            instance._animator.SetTrigger(instance._animIDClimbingStart);
        }

        private void ResetIKPosition()
        {
            Player instance = Player.instance;
            instance.rightHandIKPosition = Vector3.zero;
            instance.leftHandIKPosition = Vector3.zero;
        }

        private Vector3 GetNowSplinePos()
        {
            return SplineUtility.EvaluatePosition(spline, splineRate).ChengeVector3();
        }

        private Vector3 GetMoveDir()
        {
            Player instance = Player.instance;
            if (instance.playerMove.magnitude < 0.1f) return Vector3.zero;

            Vector3 moveDir = LibVector.RotationDirOfObjectFront(instance._mainCamera.transform, instance.playerMove);
            Debug.Log(moveDir);
            return moveDir;
        }

        private Vector3 CalNextPos(Vector3 nowSplinePos, Vector3 moveDir)
        {
            //if (inputDir.magnitude == 0) return nowSplinePos;
            //Vector3 movedWoldPos = nowSplinePos + moveDir;
            //SplineUtility.GetNearestPoint(
            //  spline,
            //  movedWoldPos,
            //  out float3 movedSplinePos,
            //  out splineRate);
            //return movedSplinePos.ChengeVector3();

            if (moveDir.magnitude == 0) return nowSplinePos;

            float dir = LibVector.HolizontalElementOfForwardToDir(instance.transform.forward, moveDir);
            splineRate += SPLINE_MOVE_SPEED * Time.deltaTime * dir / splineLength;
            splineRate = LibMath.CastLimit(splineRate, 0, 1);

            return spline.EvaluatePosition(splineRate).ChengeVector3();
        }

        private void MoveManagement(Vector3 movePos)
        {
            Player instance = Player.instance;
            Vector2 dir2D = new Vector2(POSISION_OFFSET.x, POSISION_OFFSET.z);
            Vector3 offset = LibVector.RotationDirOfObjectFront(instance.transform, dir2D) * dir2D.magnitude;
            offset.y += POSISION_OFFSET.y;
            instance.transform.MoveFocusSpeed(movePos - offset, START_MOVE_SPEED);
        }



        private void SetAnimatorIK(Vector3 movePos, Vector3 moveDir)
        {
            Player instance = Player.instance;

            //アニメーターに数値を入れる
            float dir = (moveDir.magnitude == 0) ? 0 : LibVector.HolizontalElementOfForwardToDir(instance.transform.forward, moveDir);
            instance._animator.SetFloat(instance._animIDClimbing_x, dir);

            //IK用の情報を入れる
            instance.rightHandIKPosition = movePos + RIGHT_HAND_OFFSET;
            instance.leftHandIKPosition = movePos + LEFT_HAND_OFFSET;
        }

        private void Rot()
        {
            spline.Evaluate(splineRate, out float3 position, out float3 tangent, out float3 upVector);
            Player.instance.transform.RotFocusSpeed(Quaternion.LookRotation(tangent, Vector3.up) * Quaternion.Euler(0, ROT_OFFSET, 0), ROT_SPEED);
        }

    }
}