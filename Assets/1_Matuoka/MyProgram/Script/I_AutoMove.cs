using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : SingletonActionListener<Player>
{
    public interface I_AutoMove
    {
        public Player GetThis();

        /// <summary>
        /// キャラクターの移動に関する処理
        /// 先にRot()
        /// </summary>
        public void Move()
        {
            // 走る最大速度
            float targetSpeed = GetThis().SPRINT_SPEED;

            // コントローラー入力
            //if (GetThis().playerMove == Vector2.zero) targetSpeed = 0.0f;

            // 現在の速度
            float currentHorizontalSpeed = new Vector3(GetThis()._controller.velocity.x, 0.0f, GetThis()._controller.velocity.z).magnitude;

            // ？？？
            const float SPEED_OFFSET = 0.1f;

            // コントローラー入力のMagnitude
            //float inputMagnitude = GetThis().playerMove.magnitude;
            float inputMagnitude = 1;



            // 速度計算

            if (currentHorizontalSpeed < targetSpeed - SPEED_OFFSET || currentHorizontalSpeed > targetSpeed + SPEED_OFFSET)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                // 徐々に速度を変える？
                GetThis()._speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * GetThis().SPEED_CHANGE_RATE);

                // 速度を小数点以下3桁に丸める
                // Mathf.Round 最も近い整数を返す(端数が.5の場合は最も近い偶数を返す)
                GetThis()._speed = Mathf.Round(GetThis()._speed * 1000f) / 1000f;
            }
            else
            {
                GetThis()._speed = targetSpeed;
            }

            GetThis()._animationBlend = Mathf.Lerp(GetThis()._animationBlend, targetSpeed, Time.deltaTime * GetThis().SPEED_CHANGE_RATE);
            if (GetThis()._animationBlend < 0.01f) GetThis()._animationBlend = 0f;

            // 進行方向
            //Vector3 targetDirection = Quaternion.Euler(0.0f, GetThis()._targetRotation, 0.0f) * Vector3.forward;
            Vector3 targetDirection = GetThis().transform.forward;

            GetThis()._controller.Move(targetDirection.normalized * (GetThis()._speed * Time.deltaTime) + new Vector3(0.0f, GetThis()._verticalVelocity, 0.0f) * Time.deltaTime);


            // アニメーター関連
            GetThis()._animator.SetFloat(GetThis()._animIDSpeed, GetThis()._animationBlend);
            GetThis()._animator.SetFloat(GetThis()._animIDMotionSpeed, inputMagnitude);

        }
    }
}
