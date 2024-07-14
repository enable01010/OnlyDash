using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : SingletonActionListener<Player>
{
    public interface I_AutoMove
    {
        public Player GetThis();

        /// <summary>
        /// �L�����N�^�[�̈ړ��Ɋւ��鏈��
        /// ���Rot()
        /// </summary>
        public void Move()
        {
            // ����ő呬�x
            float targetSpeed = GetThis().SPRINT_SPEED;

            // �R���g���[���[����
            //if (GetThis().playerMove == Vector2.zero) targetSpeed = 0.0f;

            // ���݂̑��x
            float currentHorizontalSpeed = new Vector3(GetThis()._controller.velocity.x, 0.0f, GetThis()._controller.velocity.z).magnitude;

            // �H�H�H
            const float SPEED_OFFSET = 0.1f;

            // �R���g���[���[���͂�Magnitude
            //float inputMagnitude = GetThis().playerMove.magnitude;
            float inputMagnitude = 1;



            // ���x�v�Z

            if (currentHorizontalSpeed < targetSpeed - SPEED_OFFSET || currentHorizontalSpeed > targetSpeed + SPEED_OFFSET)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                // ���X�ɑ��x��ς���H
                GetThis()._speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * GetThis().SPEED_CHANGE_RATE);

                // ���x�������_�ȉ�3���Ɋۂ߂�
                // Mathf.Round �ł��߂�������Ԃ�(�[����.5�̏ꍇ�͍ł��߂�������Ԃ�)
                GetThis()._speed = Mathf.Round(GetThis()._speed * 1000f) / 1000f;
            }
            else
            {
                GetThis()._speed = targetSpeed;
            }

            GetThis()._animationBlend = Mathf.Lerp(GetThis()._animationBlend, targetSpeed, Time.deltaTime * GetThis().SPEED_CHANGE_RATE);
            if (GetThis()._animationBlend < 0.01f) GetThis()._animationBlend = 0f;

            // �i�s����
            //Vector3 targetDirection = Quaternion.Euler(0.0f, GetThis()._targetRotation, 0.0f) * Vector3.forward;
            Vector3 targetDirection = GetThis().transform.forward;

            GetThis()._controller.Move(targetDirection.normalized * (GetThis()._speed * Time.deltaTime) + new Vector3(0.0f, GetThis()._verticalVelocity, 0.0f) * Time.deltaTime);


            // �A�j���[�^�[�֘A
            GetThis()._animator.SetFloat(GetThis()._animIDSpeed, GetThis()._animationBlend);
            GetThis()._animator.SetFloat(GetThis()._animIDMotionSpeed, inputMagnitude);

        }
    }
}
