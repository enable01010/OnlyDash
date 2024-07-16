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
        private List<WallArea> wallAreaList;
        private WallArea wallArea;

        [SerializeField] float START_MOVE_SPEED = 5.0f;
        [SerializeField] Vector3 POSISION_OFFSET = new Vector3(0, 0.15f, 0.15f);
        [SerializeField] float MAX_DISTANCE = 0.1f;

        //スプライン用
        private float splineRate = 0;

        //移動用
        [SerializeField] float holizontalMoveSpeed;
        [SerializeField] Vector3 RIGHT_HAND_OFFSET;
        [SerializeField] Vector3 LEFT_HAND_OFFSET;
        [SerializeField] float ROT_OFFSET;


        public virtual bool IsGuard()
        {
            if (wallArea == null) return true;
            return false;
        }
        public virtual void OnEnter()
        {
            SelectedWallArea();
            SetAnimator();
        }
        public virtual void Climbing()
        {
            RotManagement();
            MoveManagement();

            Move();
            Rot();
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
                    wallAreaList[i]._spline,
                    instance.transform.position.ChangeFloat3(),
                    out float3 nearPos,
                    out float nearPosRate);
                //TODO: 消せるか実験

                if (minsDistance < distance)
                {
                    minsDistance = distance;
                    minsNumber = i;
                    splineRate = nearPosRate;
                }

            }
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

        private void RotManagement()
        {
            Player instance = Player.instance;
            float rotation = Mathf.SmoothDampAngle(instance.transform.eulerAngles.y, wallArea.rot.y, ref instance._rotationVelocity, instance.ROTATION_SMOOTH_TIME);
            instance.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        private void MoveManagement(Vector3 goalPos)
        {
            Player instance = Player.instance;
            Vector3 moveDir = goalPos - instance.transform.position - POSISION_OFFSET;
            if (MAX_DISTANCE > moveDir.magnitude) return;
            moveDir = moveDir.normalized * START_MOVE_SPEED * Time.deltaTime;
            instance._controller.Move(moveDir);
        }

        private void Move()
        {
            Player instance = Player.instance;
            Spline spline = wallArea._spline;

            //入力を変数に置き換える
            Vector2 inputDir = instance.playerMove;
            float xDir = inputDir.x;

            //現状のsplineのポジションを求める
            Vector3 nowPos = spline.EvaluatePosition(splineRate);

            //TODO: カメラの向きに合わせ
            Vector3 inputDirection = new Vector3(instance.playerMove.x, 0.0f, instance.playerMove.y).normalized;
            float _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + instance._mainCamera.transform.eulerAngles.y;
            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            //TODO: 移動先のポイントを求める
            Vector3 movePos = nowPos + targetDirection.normalized * (instance._speed * Time.deltaTime);

            //移動先のポイントから一番近いスプラインのポジションを求める
            SplineUtility.GetNearestPoint(
                spline,
                movePos.ChangeFloat3(),
                out float3 nearPos,
                out splineRate);

            //TODO: splineRateに合わせてキャラクターの位置を調整する
            MoveManagement(nearPos.ChengeVector3());

            //アニメーターに数値を入れる
            instance._animator.SetFloat(instance._animIDClimbing_x, xDir);

            //IK用の情報を入れる
            instance.rightHandIKPosition = nearPos.ChengeVector3() + RIGHT_HAND_OFFSET;
            instance.leftHandIKPosition = nearPos.ChengeVector3() + LEFT_HAND_OFFSET;
        }

        private void Rot()
        {
            Player instance = Player.instance;
            Spline spline = wallArea._spline;

            //ベクトルを角度に変更する
            Vector3 spinePos = spline.EvaluatePosition(splineRate);
            Vector3 playerPos = instance.transform.position;
            Vector3 dir = spinePos - playerPos;
            float bestAngle = Mathf.Atan2(dir.y, dir.x) + ROT_OFFSET;

            //TODO:目標値に向けて回転させる
            float rotation = Mathf.SmoothDampAngle(instance.transform.eulerAngles.y, bestAngle, ref instance._rotationVelocity, instance.ROTATION_SMOOTH_TIME);
            instance.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

    }
}