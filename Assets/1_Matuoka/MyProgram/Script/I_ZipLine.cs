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
        /// Player��Start�ŌĂ΂��
        /// </summary>
        public void PlayerStart();

        /// <summary>
        /// Player��Update�ŌĂ΂��
        /// </summary>
        public void PlayerUpdate();


        /// <summary>
        /// State��ς���Ƃ��̏���
        /// </summary>
        public void OnTrigger();

        /// <summary>
        /// OnTrigger�̂��߂̃K�[�h��
        /// </summary>
        /// <returns></returns>
        public bool IsGuardOnTrigger();


        /// <summary>
        /// State��OnEnter
        /// </summary>
        public void OnEnter();

        /// <summary>
        /// State��OnUpdate
        /// </summary>
        public void OnUpdate();

        /// <summary>
        /// State��OnExit
        /// </summary>
        public void OnExit();


        /// <summary>
        /// Area�̒ǉ�
        /// </summary>
        /// <param name="zipLineArea"></param>
        public void AddArea(ZipLineArea zipLineArea);

        /// <summary>
        /// Area�̍폜
        /// </summary>
        /// <param name="zipLineArea"></param>
        public void DeleteArea(ZipLineArea zipLineArea);
    }

    [System.Serializable]
    public class DefaultZipLine : I_ZipLine
    {
        #region �ϐ�

        //[Header("Spline")]

        //[SerializeField, ReadOnly]
        private List<ZipLineArea> zipLineAreaList = new List<ZipLineArea>();
        private int nearSplineNumber;
        private SplineNearestPos nearSplinePos;
        private SplineContainer nearSplineContainer;
        private float nearSplineLength;



        // �`�F�b�N�p
        private bool canPushButton = false;
        private bool isRide = false;



        [Header("���n�߂�Ƃ�")]

        [SerializeField, Tooltip("��ԋ߂�Spline�Ƃ̋���"), ReadOnly]
        private float nearDistance = 0f;

        [SerializeField, Tooltip("����͈�")]
        private float rideRange = 3f;

        //[SerializeField, Tooltip("����͈͂�Player�̒��S��offset")]
        private Vector3 offsetRideCenterPos = new Vector3(0f, 1.2f, 0f);

        private Vector3 offsetRideCenterPosScale;// Scale�Ή�(�r���ő傫���ς��Ȃ��Ȃ�Start��OK)

        [SerializeField, Tooltip("�������Œ肷�钷��(�[����)")]
        private float dirFreezeLength = 10f;

        private bool isDirectionPlus = true;



        [Header("�d������")]

        [SerializeField, Tooltip("���܂łɂ����鎞��")]
        private float moveWaitTime = 0.2f;

        [SerializeField, Tooltip("���܂ł̎��Ԃɉ�������]���x")]
        private float moveWaitRotSpeed = 10f;

        [SerializeField, Tooltip("���܂łɂ�����c��̎���"), ReadOnly]
        private float moveWaitElapsedTime = 0f;

        [SerializeField, Tooltip("�~��n�߂��܂ł̎���")]
        private float rideEndIntervalTime = 1f;

        [SerializeField, Tooltip("�ēx���n�߂��܂ł̎���")]
        private float reRideIntervalTime = 1f;

        [SerializeField, Tooltip("�{�^����������܂ł̎c��̎���"), ReadOnly]
        private float nowPushWaitTime = 0f;



        [Header("�ړ���")]

        [SerializeField, Tooltip("���݈ʒu����"), ReadOnly]
        private float nowRate;

        [SerializeField, Tooltip("�ړ��X�s�[�h")]
        private float speed = 5f;
        
        //[SerializeField, Tooltip("�͂ވʒu")]
        private Vector3 offsetHandPos = new Vector3(0f, 1.62f, 0.15f);

        private Vector3 offsetHandPosScale;// Scale�Ή�(�r���ő傫���ς��Ȃ��Ȃ�Start��OK)
        
        [SerializeField, Tooltip("�X���Ȃ���")]
        private bool isFreezeRotation = false;



        [Header("�~���Ƃ�")]

        [SerializeField, Tooltip("�����ō~��钷��(�[����)")]
        private float edgeLength = 1f;

        [SerializeField, Tooltip("�W�����v�̍���")]
        private float JUMP_HIGHT = 3;



        //[Header("IK�p�ϐ�")]// offsetHandPos�ς���Ȃ炱���������

        //[SerializeField, Tooltip("IK�E��")]
        Vector3 RIGHT_HAND = new Vector3(-1.3f, 6.62f, 4.15f);

        //[SerializeField, Tooltip("IK����")]
        Vector3 LEFT_HAND = new Vector3(1f, 6.62f, 4.77f);

        //[SerializeField, Tooltip("IK�E��")]
        Vector3 RIGHT_LEG = new Vector3(0.06f, 0.47f, 0.15f);

        //[SerializeField, Tooltip("IK����")]
        Vector3 LEF_LEGD = new Vector3(-0.06f, 0.42f, 0.15f);

        #endregion


        #region PlayerStart

        public virtual void PlayerStart()
        {
            // �r���ő傫���ς��Ȃ��Ȃ�Start��OK
            offsetRideCenterPosScale.x = offsetRideCenterPos.x * instance.transform.lossyScale.x;
            offsetRideCenterPosScale.y = offsetRideCenterPos.y * instance.transform.lossyScale.y;
            offsetRideCenterPosScale.z = offsetRideCenterPos.z * instance.transform.lossyScale.z;

            // �r���ő傫���ς��Ȃ��Ȃ�Start��OK
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

            // bool��ێ�
            bool temp = canPushButton;
            canPushButton = CanPushButtonUpdate();

            // �O��t���[����canRide����ύX����������
            if (temp == false && canPushButton == true)
            {
                LibButtonUIInfoManager.PopIcon(ButtonType.ZipLine);
            }
            else if (temp == true && canPushButton == false)
            {
                LibButtonUIInfoManager.RemoveIcon(ButtonType.ZipLine);
            }
        }

        // �����X�V
        private void DistanceZipLineUpdate()
        {
            for (int i = 0; i < zipLineAreaList.Count; i++)
            {
                zipLineAreaList[i].splinePos.NearestDistanceUpdate(instance.transform.position + offsetRideCenterPosScale);
            }
        }

        // near�ϐ��Ɋi�[
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

        // �{�^���������邩
        private bool CanPushButtonUpdate()
        {
            if (CanPushWaitTimeUpdate() && IsRideRangeCheckUpdate()) return true;

            return false;
        }

        // ����͈͂ɂ��邩
        private bool IsRideRangeCheckUpdate()
        {
            if (isRide == true) return true;

            return nearDistance < rideRange;
        }

        // �{�^���̑҂����Ԃ̍X�V
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

        // �i�s�������߂�
        private void StartZipLineDirection()
        {
            // �[�ɂ���������Œ�
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

            // �����Ă�������ɋ߂������ɂ���
            Vector3 dir = nearSplineContainer.EvaluateTangent(nowRate);// �X�v���C���̐ڐ��x�N�g��
            float angle = Vector3.Angle(instance.transform.forward.normalized, dir.normalized);
            isDirectionPlus = angle <= 90;
        }

        #endregion


        #region OnUpdate

        public virtual void OnUpdate()
        {
            // ���O
            if (RideBeforeCheck() == true)
            {
                RideBeforeUpdate();
                return;
            }

            // �������
            if (EdgeEndLengthCheck() == true) return;

            NowRateUpdate();
            MoveRotation();
            MovePos();
            MoveIKPos();
        }

        // �[�܂ŗ�����~���
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

        // ZipLine�ɏ���Ă��邩
        private bool RideBeforeCheck()
        {
            return moveWaitElapsedTime > 0;
        }

        // NowRate�̍X�V
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

        // ZipLine�ɏ��O��Update
        private void RideBeforeUpdate()
        {
            instance.transform.RotFocusSpeed(MoveQuaternion(), moveWaitRotSpeed);

            Vector3 targetPos = (Vector3)nearSplineContainer.EvaluatePosition(nowRate) - OffsetPlayerHandPos();
            instance.transform.MoveFocusTime(targetPos, ref moveWaitElapsedTime);

            MoveIKPos();
        }

        // Player�̉�]
        private void MoveRotation()
        {
            instance.transform.rotation = MoveQuaternion();

            // Player�����E�ɉ�]���Ȃ�(�X���Ȃ�)
            if (isFreezeRotation == true)
            {
                instance.transform.rotation = Quaternion.Euler(0, instance.transform.localEulerAngles.y, 0);
            }
        }

        // Player�̉�]�̂��߂̃N�H�[�^�j�I��
        private Quaternion MoveQuaternion()
        {
            // �X�v���C���̏�(�@��)�x�N�g��
            Vector3 up = nearSplineContainer.EvaluateUpVector(nowRate);

            // �X�v���C���̐ڐ��x�N�g��
            Vector3 forward = nearSplineContainer.EvaluateTangent(nowRate);
            if (isDirectionPlus != true) forward *= -1f;

            // �N�H�[�^�j�I���v�Z
            Quaternion quaternion = Quaternion.LookRotation(forward, up);
            return quaternion;
        }

        // Player�̈ړ�
        private void MovePos()
        {
            // �ړ�
            instance.transform.position = nearSplineContainer.EvaluatePosition(nowRate);

            // �͂ވʒu�ɍ��킹��
            instance.transform.position -= OffsetPlayerHandPos();
        }

        // Player�̒��S����͂ވʒu�܂ł̃x�N�g��
        private Vector3 OffsetPlayerHandPos()
        {
            Quaternion rot = Quaternion.Euler(instance.transform.localEulerAngles);

            return rot * offsetHandPosScale;
        }

        // IK�̔��f
        private void MoveIKPos()
        {
            // Player�̌��݂̉�]
            Quaternion rot = Quaternion.Euler(instance.transform.localEulerAngles);

            // == Vector3.zero �Ȃ猳�̃A�j���[�V�����̂܂�
            // != Vector3.zero �Ȃ�l���̐�̃��[���h���W�w���IK
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

            // Player�̒���
            instance.transform.rotation = Quaternion.Euler(0, instance.transform.localEulerAngles.y, 0);
            //IKPosReset();
            EndZipLineJump();
        }

        // IK�̃��Z�b�g
        private void IKPosReset()
        {
            instance.rightHandIKPosition = Vector3.zero;
            instance.leftHandIKPosition = Vector3.zero;
            instance.rightLegIKPosition = Vector3.zero;
            instance.leftLegIKPosition = Vector3.zero;
        }

        // �~���Ƃ��̃W�����v
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