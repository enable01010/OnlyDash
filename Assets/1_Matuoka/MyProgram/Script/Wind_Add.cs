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

        [SerializeField, Tooltip("�����󂯂�ő勗��")]
        private float maxDistance = 10f;

        [SerializeField, Tooltip("�ŏ��p���[")]
        private float minPower = 3f;

        [SerializeField, Tooltip("�ő�p���[")]
        private float maxPower = 5f;

        [SerializeField, Tooltip("���݃p���["), ReadOnly]
        private float nowPower = 0f;

        [SerializeField, Tooltip("�c�����̑��x"), ReadOnly]
        private float verticalPower;

        //[SerializeField, Tooltip("�����x")] 
        private float acceleration = 1f;

        [SerializeField, Tooltip("�f�o�b�O�p"), ReadOnly]
        private float nowVelocity = 1f;

        #endregion


        #region I_AdditionalState

        public virtual void OnEnter()
        {
            nowPower = 0f;
        }

        public virtual void OnUpdate()
        {
            MoveUpdate();

            //PlayerMoveUpdate();// ���s��
        }

        public virtual void OnExit()
        {
            nowPower = 0f;
        }

        #endregion


        #region CustomMethod

        public void Init(Transform transform)
        {
            windAreaTransform = transform;
        }

        private void PlayerMoveUpdate()
        {
            nowPower += acceleration * Time.deltaTime;
            nowPower = Mathf.Min(nowPower, maxPower);
            Vector3 forward = windAreaTransform.forward;
            instance._controller.Move(nowPower * Time.deltaTime * forward);
        }

        private void MoveUpdate()
        {
            Vector3 forward = windAreaTransform.forward;

            float distance = Vector3.Distance(windAreaTransform.position, instance.transform.position);
            float rate = Mathf.InverseLerp(0f, maxDistance, distance);
            nowPower = Mathf.Lerp(minPower, maxPower, 1f - rate);

            Vector3 power = nowPower * forward;

            //Vector3 power = maxPower * forward;

            // �c�����̑��x�ύX
            verticalPower = power.y;

            if (verticalPower > 0 && instance.isGrounded == true)
            {
                instance.isGrounded = false;
                instance._verticalVelocity = 0;
            }

            instance._verticalVelocity += verticalPower * Time.deltaTime;

            instance._verticalVelocity = Mathf.Clamp(instance._verticalVelocity, -4f, 4f);
            nowVelocity = instance._verticalVelocity;

            // xz���ʂ�position�ړ�
            power.y = 0;
            instance._controller.Move(power * Time.deltaTime);

            
        }


        #endregion
    }
}
