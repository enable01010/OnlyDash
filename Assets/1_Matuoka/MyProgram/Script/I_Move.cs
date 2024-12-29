using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : SingletonActionListener<Player>
{
    public interface I_Move
    {
        /// <summary>
        /// キャラクターの移動に関する処理
        /// 先にRot()
        /// </summary>
        public void Move();
    }

    [System.Serializable]
    public class ControlledMove : I_Move
    {
        public virtual void Move()
        {
            instance._speed = instance.playerMove.magnitude * instance.MOVE_SPEED;
            instance._animationBlend = (instance._speed < 0.1f) ? 0 : instance._speed;
            Vector3 targetDirection = Quaternion.Euler(0.0f, instance._targetRotation, 0.0f) * Vector3.forward;
            instance._controller.Move(targetDirection.normalized * (instance._speed * Time.deltaTime) + new Vector3(0.0f, instance._verticalVelocity, 0.0f) * Time.deltaTime);
            instance._animator.SetFloat(instance._animIDSpeed, instance._animationBlend);
        }
    }

    [System.Serializable]
    public class AutoMove : I_Move
    {
        public virtual void Move()
        {
            Player instance = Player.instance;

            // 走る最大速度
            float targetSpeed = instance.SPRINT_SPEED;

            // コントローラー入力
            //if (instance.playerMove == Vector2.zero) targetSpeed = 0.0f;

            // 現在の速度
            float currentHorizontalSpeed = new Vector3(instance._controller.velocity.x, 0.0f, instance._controller.velocity.z).magnitude;

            // ？？？
            const float SPEED_OFFSET = 0.1f;

            // コントローラー入力のMagnitude
            //float inputMagnitude = instance.playerMove.magnitude;
            float inputMagnitude = 1;



            // 速度計算

            if (currentHorizontalSpeed < targetSpeed - SPEED_OFFSET || currentHorizontalSpeed > targetSpeed + SPEED_OFFSET)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                // 徐々に速度を変える？
                instance._speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * instance.SPEED_CHANGE_RATE);

                // 速度を小数点以下3桁に丸める
                // Mathf.Round 最も近い整数を返す(端数が.5の場合は最も近い偶数を返す)
                instance._speed = Mathf.Round(instance._speed * 1000f) / 1000f;
            }
            else
            {
                instance._speed = targetSpeed;
            }

            instance._animationBlend = Mathf.Lerp(instance._animationBlend, targetSpeed, Time.deltaTime * instance.SPEED_CHANGE_RATE);
            if (instance._animationBlend < 0.01f) instance._animationBlend = 0f;

            // 進行方向
            //Vector3 targetDirection = Quaternion.Euler(0.0f, instance._targetRotation, 0.0f) * Vector3.forward;
            Vector3 targetDirection = instance.transform.forward;

            instance._controller.Move(targetDirection.normalized * (instance._speed * Time.deltaTime) + new Vector3(0.0f, instance._verticalVelocity, 0.0f) * Time.deltaTime);


            // アニメーター関連
            instance._animator.SetFloat(instance._animIDSpeed, instance._animationBlend);
            instance._animator.SetFloat(instance._animIDMotionSpeed, inputMagnitude);
        }
    }

#if UNITY_EDITOR

    [System.Serializable]
    public class DroneMove : I_Move
    {
        public virtual void Move()
        {
            float MOVE_SPEED = 5f;
            float getkeySpeed = MOVE_SPEED * Time.deltaTime;
            Transform playerTrans = instance.transform;
            Quaternion rot = Quaternion.Euler(0.0f, instance.DebugCameraAngleGet().y, 0.0f);

            // Keyに応じて移動
            if (Input.GetKey(KeyCode.E))
            {
                Vector3 up = Vector3.up * getkeySpeed;
                playerTrans.position += up;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                Vector3 down = Vector3.down * getkeySpeed;
                playerTrans.position += down;
            }
            if (Input.GetKey(KeyCode.W))
            {
                Vector3 forward = Vector3.forward * getkeySpeed;
                playerTrans.position += rot * forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                Vector3 back = Vector3.back * getkeySpeed;
                playerTrans.position += rot * back;
            }
            if (Input.GetKey(KeyCode.D))
            {
                Vector3 right = Vector3.right * getkeySpeed;
                playerTrans.position += rot * right;
            }
            if (Input.GetKey(KeyCode.A))
            {
                Vector3 left = Vector3.left * getkeySpeed;
                playerTrans.position += rot * left;
            }
        }
    }

#endif

}

