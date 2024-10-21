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

        [SerializeField, Tooltip("•—‚ğó‚¯‚éÅ‘å‹——£")]
        private float maxDistance = 10f;

        [Header("XZ•ûŒü‚Ì—Í")]

        [SerializeField, Tooltip("Å¬ƒpƒ[")]
        private float minPower = 3f;

        [SerializeField, Tooltip("Å‘åƒpƒ[")]
        private float maxPower = 5f;

        [SerializeField, Tooltip("Œ»İƒpƒ["), ReadOnly]
        private float nowPower = 0f;


        [Header("Y•ûŒü‚Ì—Í")]

        [SerializeField, Tooltip("U•")]
        private float amplitude = 3f;

        [SerializeField, Tooltip("üŠú")]
        private float period = 2f;

        [SerializeField, Tooltip("Šî€‚‚³")]
        private float hight = 5f;

        [SerializeField, Tooltip("ã¸‘¬“x")]
        private float risingVelocity = 2f;

        [SerializeField, Tooltip("U“®‚µ‚Ä‚é‚©"), ReadOnly]
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
        /// transform‚ğ“n‚·
        /// </summary>
        /// <param name="transform"></param>
        public void Init(Transform transform)
        {
            windAreaTransform = transform;
        }

        /// <summary>
        /// ‹——£‚ğ”½‰f‚µ‚½—ÍŒvZ
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
        /// xz•½–Ê‚ÌpositionˆÚ“® _controller.Move
        /// </summary>
        /// <param name="power"></param>
        private void MoveUpdateXZ(Vector3 power)
        {
            power.y = 0;
            instance._controller.Move(power * Time.deltaTime);
        }

        /// <summary>
        /// y•ûŒü‚Ì‘¬“x”½‰f _verticalVelocity
        /// </summary>
        /// <param name="power"></param>
        private void MoveUpdateY(Vector3 power)
        {
            // —Í‚ª‰ºŒü‚«‚Ì‚Æ‚«
            if (power.y <= 0)
            {
                instance._verticalVelocity += power.y * Time.deltaTime;
                return;
            }


            // —Í‚ªãŒü‚«‚Ì‚Æ‚«
            instance._verticalVelocity = 0;

            // ’…’n”»’è‚ğ—£‚·
            if (instance._verticalVelocity <= 0 && instance.isGrounded == true)
            {
                instance.isGrounded = false;

                // InAir‚É‘JˆÚ
                instance._animator.SetBool(instance._animIDWind, true);
            }

            // U“®‘O
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

            // U“®
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
