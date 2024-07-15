using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public partial class Player : SingletonActionListener<Player>
{
    public interface I_Sliding
    {
        /// <summary>
        /// キャラクターのスライディングに関する処理
        /// </summary>
        public void OnExit();
        public void Sliding();
        public void OnEnter();
    }  

    public class DefaultSliding:I_Sliding
    {
        public virtual void OnEnter()
        {
            Player instance = Player.instance;

            instance._animator.SetTrigger(instance._animIDSliding);
            instance._slidingTimeoutDelta = instance.SLIDING_TIMEOUT;
        }

        public virtual void OnExit()
        {
            Player instance = Player.instance;
            instance._slidingTimeoutDelta = 0;
        }

        public virtual void Sliding()
        {
            RotManagement();
            MoveManagerment();
            TimeManagement();
        }

        /// <summary>
        /// スライディングの時間管理
        /// </summary>
        private void TimeManagement()
        {
            Player instance = Player.instance;
            instance._slidingTimeoutDelta -= Time.deltaTime;
            if (instance._slidingTimeoutDelta < 0)
            {
                CustomEvent.Trigger(instance.gameObject, "endSliding");
            }
        }

        private void RotManagement()
        {
            Player instance = Player.instance;
            float rotation = Mathf.SmoothDampAngle(instance.transform.eulerAngles.y, instance._targetRotation, ref instance._rotationVelocity, instance.ROTATION_SMOOTH_TIME);

            // rotate to face input direction relative to camera position
            instance.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        /// <summary>
        /// 移動の管理
        /// </summary>
        private void MoveManagerment()
        {
            Player instance = Player.instance;
            // 進行方向
            Vector3 targetDirection = Quaternion.Euler(0.0f, instance._targetRotation, 0.0f) * Vector3.forward;

            instance._controller.Move(targetDirection.normalized * (instance.SLIDING_SPEED * Time.deltaTime) + new Vector3(0.0f, instance._verticalVelocity, 0.0f) * Time.deltaTime);
        }
    }
}
