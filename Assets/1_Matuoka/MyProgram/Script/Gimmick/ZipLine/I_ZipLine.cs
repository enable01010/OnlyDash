using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public partial class Player : SingletonActionListener<Player>
{
    public interface I_ZipLine
    {
        /// <summary>
        /// PlayerのStartで呼ばれる
        /// </summary>
        public void PlayerStart();

        /// <summary>
        /// PlayerのUpdateで呼ばれる
        /// </summary>
        public void PlayerUpdate();


        /// <summary>
        /// Stateを変えるときの処理
        /// </summary>
        public void OnTrigger();

        /// <summary>
        /// OnTriggerのためのガード節
        /// </summary>
        /// <returns></returns>
        public bool IsGuardOnTrigger();


        /// <summary>
        /// StateのOnEnter
        /// </summary>
        public void OnEnter();

        /// <summary>
        /// StateのOnUpdate
        /// </summary>
        public void OnUpdate();

        /// <summary>
        /// StateのOnExit
        /// </summary>
        public void OnExit();


        /// <summary>
        /// Areaの追加
        /// </summary>
        /// <param name="zipLineArea"></param>
        public void AddArea(ZipLineArea zipLineArea);

        /// <summary>
        /// Areaの削除
        /// </summary>
        /// <param name="zipLineArea"></param>
        public void DeleteArea(ZipLineArea zipLineArea);
    }

    [System.Serializable]
    public class DefaultZipLine : I_ZipLine
    {
        #region 変数

        //[Header("Spline")]

        //[SerializeField, ReadOnly]
        private List<ZipLineArea> zipLineAreaList = new List<ZipLineArea>();
        private int nearSplineNumber;
        private SplineNearestPos nearSplinePos;
        private SplineContainer nearSplineContainer;
        private float nearSplineLength;



        // チェック用
        private bool canPushButton = false;
        private bool isRide = false;



        [Header("乗り始めるとき")]

        [SerializeField, Tooltip("一番近いSplineとの距離"), ReadOnly]
        private float nearDistance = 0f;

        [SerializeField, Tooltip("乗れる範囲")]
        private float rideRange = 3f;

        //[SerializeField, Tooltip("乗れる範囲のPlayerの中心のoffset")]
        private Vector3 offsetRideCenterPos = new Vector3(0f, 1.2f, 0f);

        private Vector3 offsetRideCenterPosScale;// Scale対応(途中で大きさ変わらないならStartでOK)

        [SerializeField, Tooltip("向きを固定する長さ(端から)")]
        private float dirFreezeLength = 10f;

        private bool isDirectionPlus = true;



        [Header("硬直時間")]

        [SerializeField, Tooltip("乗るまでにかかる時間")]
        private float moveWaitTime = 0.2f;

        [SerializeField, Tooltip("乗るまでの時間に沿った回転速度")]
        private float moveWaitRotSpeed = 10f;

        [SerializeField, Tooltip("乗るまでにかかる残りの時間"), ReadOnly]
        private float moveWaitElapsedTime = 0f;

        [SerializeField, Tooltip("降り始めれるまでの時間")]
        private float rideEndIntervalTime = 1f;

        [SerializeField, Tooltip("再度乗り始めれるまでの時間")]
        private float reRideIntervalTime = 1f;

        [SerializeField, Tooltip("ボタンを押せるまでの残りの時間"), ReadOnly]
        private float nowPushWaitTime = 0f;



        [Header("移動時")]

        [SerializeField, Tooltip("現在位置割合"), ReadOnly]
        private float nowRate;

        [SerializeField, Tooltip("移動スピード")]
        private float speed = 5f;
        
        //[SerializeField, Tooltip("掴む位置")]
        private Vector3 offsetHandPos = new Vector3(0f, 1.62f, 0.15f);

        private Vector3 offsetHandPosScale;// Scale対応(途中で大きさ変わらないならStartでOK)
        
        [SerializeField, Tooltip("傾けないか")]
        private bool isFreezeRotation = false;



        [Header("降りるとき")]

        [SerializeField, Tooltip("自動で降りる長さ(端から)")]
        private float edgeLength = 1f;

        [SerializeField, Tooltip("ジャンプの高さ")]
        private float JUMP_HIGHT = 3;



        //[Header("IK用変数")]// offsetHandPos変えるならこちらも調整

        //[SerializeField, Tooltip("IK右手")]
        Vector3 RIGHT_HAND = new Vector3(-1.3f, 6.62f, 4.15f);

        //[SerializeField, Tooltip("IK左手")]
        Vector3 LEFT_HAND = new Vector3(1f, 6.62f, 4.77f);

        //[SerializeField, Tooltip("IK右足")]
        Vector3 RIGHT_LEG = new Vector3(0.06f, 0.47f, 0.15f);

        //[SerializeField, Tooltip("IK左足")]
        Vector3 LEF_LEGD = new Vector3(-0.06f, 0.42f, 0.15f);

        #endregion


        #region PlayerStart

        public virtual void PlayerStart()
        {
            // 途中で大きさ変わらないならStartでOK
            offsetRideCenterPosScale.x = offsetRideCenterPos.x * instance.transform.lossyScale.x;
            offsetRideCenterPosScale.y = offsetRideCenterPos.y * instance.transform.lossyScale.y;
            offsetRideCenterPosScale.z = offsetRideCenterPos.z * instance.transform.lossyScale.z;

            // 途中で大きさ変わらないならStartでOK
            offsetHandPosScale.x = offsetHandPos.x * instance.transform.lossyScale.x;
            offsetHandPosScale.y = offsetHandPos.y * instance.transform.lossyScale.y;
            offsetHandPosScale.z = offsetHandPos.z * instance.transform.lossyScale.z;
        }

        #endregion


        #region PlayerUpdate

        public virtual void PlayerUpdate()
        {
            DistanceZipLineUpdate();
            NearZipLineUpdate();

            // boolを保持
            bool temp = canPushButton;
            canPushButton = CanPushButtonUpdate();

            // 前回フレームのcanRideから変更があったら
            if (temp == false && canPushButton == true)
            {
                LibButtonUIInfoManager.PopIcon(ButtonType.ZipLine);
            }
            else if (temp == true && canPushButton == false)
            {
                LibButtonUIInfoManager.RemoveIcon(ButtonType.ZipLine);
            }
        }

        // 距離更新
        private void DistanceZipLineUpdate()
        {
            for (int i = 0; i < zipLineAreaList.Count; i++)
            {
                zipLineAreaList[i].splinePos.NearestDistanceUpdate(instance.transform.position + offsetRideCenterPosScale);
            }
        }

        // near変数に格納
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
                nearSplineLength = zipLineAreaList[nearSplineNumber].splineLength;
            }
        }

        // ボタンが押せるか
        private bool CanPushButtonUpdate()
        {
            if (CanPushWaitTimeUpdate() && IsRideRangeCheckUpdate()) return true;

            return false;
        }

        // 乗れる範囲にいるか
        private bool IsRideRangeCheckUpdate()
        {
            if (isRide == true) return true;

            return nearDistance < rideRange;
        }

        // ボタンの待ち時間の更新
        private bool CanPushWaitTimeUpdate()
        {
            if (nowPushWaitTime < 0) return true;

            nowPushWaitTime -= Time.deltaTime;
            return false;
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
            return !canPushButton;
        }

        #endregion


        #region OnEnter

        public virtual void OnEnter()
        {
            isRide = true;
            LibButtonUIInfoManager.RemoveIcon(ButtonType.ZipLine);
            instance._animator.SetBool(instance._animIDZipLine, true);
            nowPushWaitTime = rideEndIntervalTime;
            moveWaitElapsedTime = moveWaitTime; 

            nowRate = Mathf.Clamp(nearSplinePos.rate,
                SplineLengthToRate(edgeLength),
                1f - SplineLengthToRate(edgeLength));

            StartZipLineDirection();
        }

        // 進行方向決める
        private void StartZipLineDirection()
        {
            // 端にいたら向き固定
            if (nowRate < SplineLengthToRate(dirFreezeLength))
            {
                isDirectionPlus = true;
                return;
            }
            else if (nowRate > 1f - SplineLengthToRate(dirFreezeLength))
            {
                isDirectionPlus = false;
                return;
            }

            // 向いている方向に近い向きにする
            Vector3 dir = nearSplineContainer.EvaluateTangent(nowRate);// スプラインの接線ベクトル
            float angle = Vector3.Angle(instance.transform.forward.normalized, dir.normalized);
            isDirectionPlus = angle <= 90;
        }

        #endregion


        #region OnUpdate

        public virtual void OnUpdate()
        {
            // 乗る前
            if (RideBeforeCheck() == true)
            {
                RideBeforeUpdate();
                return;
            }

            // 乗った後
            if (EdgeEndLengthCheck() == true) return;

            NowRateUpdate();
            MoveRotation();
            MovePos();
            MoveIKPos();
        }

        // 端まで来たら降りる
        private bool EdgeEndLengthCheck()
        {
            float tempNowRate = nowRate;
            if (isDirectionPlus == false)
            {
                tempNowRate = 1f - tempNowRate;
            }

            if (tempNowRate >= 1f - SplineLengthToRate(edgeLength))
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

        // ZipLineに乗っているか
        private bool RideBeforeCheck()
        {
            return moveWaitElapsedTime > 0;
        }

        // NowRateの更新
        private void NowRateUpdate()
        {
            if (isDirectionPlus == true)
            {
                nowRate += (speed / nearSplineLength) * Time.deltaTime;
            }
            else
            {
                nowRate -= (speed / nearSplineLength) * Time.deltaTime;
            }

            nowRate = Mathf.Clamp01(nowRate);
        }

        // ZipLineに乗る前のUpdate
        private void RideBeforeUpdate()
        {
            instance.transform.RotFocusSpeed(MoveQuaternion(), moveWaitRotSpeed);

            Vector3 targetPos = (Vector3)nearSplineContainer.EvaluatePosition(nowRate) - OffsetPlayerHandPos();
            instance.transform.MoveFocusTime(targetPos, ref moveWaitElapsedTime);

            MoveIKPos();
        }

        // Playerの回転
        private void MoveRotation()
        {
            instance.transform.rotation = MoveQuaternion();

            // Playerを左右に回転しない(傾けない)
            if (isFreezeRotation == true)
            {
                instance.transform.rotation = Quaternion.Euler(0, instance.transform.localEulerAngles.y, 0);
            }
        }

        // Playerの回転のためのクォータニオン
        private Quaternion MoveQuaternion()
        {
            // スプラインの上(法線)ベクトル
            Vector3 up = nearSplineContainer.EvaluateUpVector(nowRate);

            // スプラインの接線ベクトル
            Vector3 forward = nearSplineContainer.EvaluateTangent(nowRate);
            if (isDirectionPlus != true) forward *= -1f;

            // クォータニオン計算
            Quaternion quaternion = Quaternion.LookRotation(forward, up);
            return quaternion;
        }

        // Playerの移動
        private void MovePos()
        {
            // 移動
            instance.transform.position = nearSplineContainer.EvaluatePosition(nowRate);

            // 掴む位置に合わせる
            instance.transform.position -= OffsetPlayerHandPos();
        }

        // Playerの中心から掴む位置までのベクトル
        private Vector3 OffsetPlayerHandPos()
        {
            Quaternion rot = Quaternion.Euler(instance.transform.localEulerAngles);

            return rot * offsetHandPosScale;
        }

        // IKの反映
        private void MoveIKPos()
        {
            // Playerの現在の回転
            Quaternion rot = Quaternion.Euler(instance.transform.localEulerAngles);

            // == Vector3.zero なら元のアニメーションのまま
            // != Vector3.zero なら四肢の先のワールド座標指定のIK
            instance.rightHandIKPosition = (RIGHT_HAND == Vector3.zero) ? Vector3.zero : instance.transform.position + rot * RIGHT_HAND;
            instance.leftHandIKPosition =  (LEFT_HAND  == Vector3.zero) ? Vector3.zero : instance.transform.position + rot * LEFT_HAND;
            instance.rightLegIKPosition =  (RIGHT_LEG  == Vector3.zero) ? Vector3.zero : instance.transform.position + rot * RIGHT_LEG;
            instance.leftLegIKPosition =   (LEF_LEGD   == Vector3.zero) ? Vector3.zero : instance.transform.position + rot * LEF_LEGD;
        }

        #endregion


        #region OnExit

        public virtual void OnExit()
        {
            isRide = false;
            LibButtonUIInfoManager.RemoveIcon(ButtonType.ZipLine);
            instance._animator.SetBool(instance._animIDZipLine, false);
            nowPushWaitTime = reRideIntervalTime;

            // Playerの調整
            instance.transform.rotation = Quaternion.Euler(0, instance.transform.localEulerAngles.y, 0);
            //IKPosReset();
            EndZipLineJump();
        }

        // IKのリセット
        private void IKPosReset()
        {
            instance.rightHandIKPosition = Vector3.zero;
            instance.leftHandIKPosition = Vector3.zero;
            instance.rightLegIKPosition = Vector3.zero;
            instance.leftLegIKPosition = Vector3.zero;
        }

        // 降りるときのジャンプ
        private void EndZipLineJump()
        {
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

        private float SplineLengthToRate(float length)
        {
            return Mathf.Clamp01(length / nearSplineLength);
        }

        #endregion
    }
}