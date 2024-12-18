using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public partial class Player : SingletonActionListener<Player>
{
    [System.Serializable]
    public class Wind_Add : I_AdditionalState
    {
        #region Fields

        private Transform windAreaTransform;

        [SerializeField, Tooltip("風を受ける最大距離")]
        private float maxDistance = 10f;

        [Header("XZ方向の力")]

        [SerializeField, Tooltip("最小パワー")]
        private float minPower = 3f;

        [SerializeField, Tooltip("最大パワー")]
        private float maxPower = 5f;

        [SerializeField, Tooltip("現在パワー"), ReadOnly]
        private float nowPower = 0f;


        [Header("Y方向の力")]

        [SerializeField, Tooltip("振幅")]
        private float amplitude = 3f;

        [SerializeField, Tooltip("周期")]
        private float period = 2f;

        [SerializeField, Tooltip("基準高さ")]
        private float hight = 5f;

        [SerializeField, Tooltip("上昇速度")]
        private float risingVelocity = 2f;

        [SerializeField, Tooltip("振動してるか"), ReadOnly]
        private bool isOscillation = false;

        private float distance;

        private float radian = 0;

        private float nearLength = 1f;


        #endregion


        #region I_AdditionalState

        public virtual void OnEnter()
        {
            nowPower = 0f;

            isOscillation = false;
            radian = 0;
        }

        public virtual void OnUpdate()
        {
            Vector3 power = PowerUpdate();
            MoveUpdateXZ(power);
            MoveUpdateY(power);
        }

        public virtual void OnExit()
        {
            nowPower = 0f;

            instance._animator.SetBool(instance._animIDWind, false);
        }

        #endregion


        #region CustomMethod

        /// <summary>
        /// transformを渡す
        /// </summary>
        /// <param name="transform"></param>
        public void Init(Transform transform)
        {
            windAreaTransform = transform;
        }

        /// <summary>
        /// 距離を反映した力計算
        /// </summary>
        /// <returns></returns>
        private Vector3 PowerUpdate()
        {
            distance = Vector3.Distance(windAreaTransform.position, instance.transform.position);

            float rate = Mathf.InverseLerp(0f, maxDistance, distance);
            nowPower = Mathf.Lerp(minPower, maxPower, 1f - rate);

            return windAreaTransform.forward * nowPower;
        }


        /// <summary>
        /// xz平面のposition移動 _controller.Move
        /// </summary>
        /// <param name="power"></param>
        private void MoveUpdateXZ(Vector3 power)
        {
            power.y = 0;
            instance._controller.Move(power * Time.deltaTime);
        }

        /// <summary>
        /// y方向の速度反映 _verticalVelocity
        /// </summary>
        /// <param name="power"></param>
        private void MoveUpdateY(Vector3 power)
        {
            // 力が下向きのとき
            if (power.y <= 0)
            {
                instance._verticalVelocity += power.y * Time.deltaTime;
                return;
            }


            // 力が上向きのとき
            instance._verticalVelocity = 0;

            // 着地判定を離す
            if (instance._verticalVelocity <= 0 && instance.isGrounded == true)
            {
                instance.isGrounded = false;

                // InAirに遷移
                instance._animator.SetBool(instance._animIDWind, true);
            }

            // 振動前
            if (isOscillation == false)
            {
                if (Mathf.Abs(hight - distance) < nearLength)
                {
                    isOscillation = true;
                }

                if (hight > distance)
                {
                    instance._verticalVelocity = risingVelocity;
                }
            }

            // 振動
            if (isOscillation == true)
            {
                float deltaRadian = 2f * Mathf.PI * Time.deltaTime / period;
                Vector3 tempUp = amplitude * (Mathf.Sin(radian + deltaRadian) - Mathf.Sin(radian)) * Vector3.up;
                instance._controller.Move(tempUp);
                radian += deltaRadian;
            }
        }

        #endregion
    }
}
