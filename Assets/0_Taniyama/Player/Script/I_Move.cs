using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : SingletonActionListener<Player>
{
    public interface I_Move
    {
        public Player GetThis();

        /// <summary>
        /// キャラクターの移動に関する処理
        /// 先にRot()
        /// </summary>
        public void Move()
        {
            float targetSpeed = GetThis().SPRINT_SPEED;
            if (GetThis().playerMove == Vector2.zero) targetSpeed = 0.0f;
            float currentHorizontalSpeed = new Vector3(GetThis()._controller.velocity.x, 0.0f, GetThis()._controller.velocity.z).magnitude;
            const float SPEED_OFFSET = 0.1f;
            float inputMagnitude = GetThis().playerMove.magnitude;

            //ぱっと見移動系計算
            if (currentHorizontalSpeed < targetSpeed - SPEED_OFFSET || currentHorizontalSpeed > targetSpeed + SPEED_OFFSET)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                GetThis()._speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * GetThis().SPEED_CHANGE_RATE);

                // round speed to 3 decimal places
                GetThis()._speed = Mathf.Round(GetThis()._speed * 1000f) / 1000f;
            }
            else
            {
                GetThis()._speed = targetSpeed;
            }

            GetThis()._animationBlend = Mathf.Lerp(GetThis()._animationBlend, targetSpeed, Time.deltaTime * GetThis().SPEED_CHANGE_RATE);
            if (GetThis()._animationBlend < 0.01f) GetThis()._animationBlend = 0f;

            Vector3 targetDirection = Quaternion.Euler(0.0f, GetThis()._targetRotation, 0.0f) * Vector3.forward;

            GetThis()._controller.Move(targetDirection.normalized * (GetThis()._speed * Time.deltaTime) + new Vector3(0.0f, GetThis()._verticalVelocity, 0.0f) * Time.deltaTime);

            GetThis()._animator.SetFloat(GetThis()._animIDSpeed, GetThis()._animationBlend);
            GetThis()._animator.SetFloat(GetThis()._animIDMotionSpeed, inputMagnitude);

        }
    }
}
