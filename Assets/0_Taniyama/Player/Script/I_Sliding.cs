using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public partial class Player : SingletonActionListener<Player>
{
    public interface I_Sliding
    {
        public Player GetThis();

        /// <summary>
        /// キャラクターの移動に関する処理
        /// </summary>
        public void Sliding()
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
            GetThis()._slidingTimeoutDelta -= Time.deltaTime;
            if (GetThis()._slidingTimeoutDelta < 0)
            {
                CustomEvent.Trigger(GetThis().gameObject, "endSliding");
            }
        }

        private void RotManagement()
        {
            float rotation = Mathf.SmoothDampAngle(GetThis().transform.eulerAngles.y, GetThis()._targetRotation, ref GetThis()._rotationVelocity, GetThis().ROTATION_SMOOTH_TIME);

            // rotate to face input direction relative to camera position
            GetThis().transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        
        /// <summary>
        /// 移動の管理
        /// </summary>
        private void MoveManagerment()
        {
            // 進行方向
            Vector3 targetDirection = Quaternion.Euler(0.0f, GetThis()._targetRotation, 0.0f) * Vector3.forward;

            GetThis()._controller.Move(targetDirection.normalized * (GetThis().SLIDING_SPEED * Time.deltaTime) + new Vector3(0.0f, GetThis()._verticalVelocity, 0.0f) * Time.deltaTime);
        }
    }  
}
