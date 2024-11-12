using System.Collections.Generic;
using UnityEngine;
using static Player;

/// <summary>
/// ����Ă�Ƃ������ɂȂ鏰
/// </summary>
public class SlowGround : MonoBehaviour,I_GeneralColliderUser
{
    List<GeneralCollider3D> gc3d = new List<GeneralCollider3D>();
    [SerializeField] Slow_Add slowParameter;

    public void OnEnter_GeneralCollider(Collider other, GeneralColliderAttribute attribute) 
    {
        gc3d.Add(attribute.gc3d);
        if(gc3d.Count == 1)
        {
            StartArea(other);
        }
    }

    public void OnExit_GeneralCollider(Collider other, GeneralColliderAttribute attribute) 
    {
        gc3d.Remove(attribute.gc3d);
        if (gc3d.Count == 0)
        {
            EndArea(other);
        }
    }

    /// <summary>
    /// �X���[�̃G���A�ɓ���n�߂��ۂ̏���
    /// </summary>
    private void StartArea(Collider other)
    {
        if(other.TryGetComponent<I_SlowGround>(out var hit))
        {
            hit.SlowGroundInGround(slowParameter);
        }
    }

    /// <summary>
    /// �X���[�̃G���A�ɏo���ۂ̏���
    /// </summary>
    private void EndArea(Collider other)
    {
        if (other.TryGetComponent<I_SlowGround>(out var hit))
        {
            hit.SlowGroundOutGround(slowParameter);
        }
    }
}

/// <summary>
/// �X�̏�Ŋ���I�u�W�F�N�g�p�̃C���^�[�t�F�[�X
/// </summary>
public interface I_SlowGround
{
    /// <summary>
    /// �X�̏�ɏ��n�߂����̏���
    /// </summary>
    public void SlowGroundInGround(Slow_Add ice);

    /// <summary>
    /// �X�̏ォ��~�肽�ۂɏ���
    /// </summary>
    public void SlowGroundOutGround(Slow_Add ice);
}

public partial class Player : SingletonActionListener<Player>
{
    /// <summary>
    /// �X�̏�ɂ���ۂ̏����p�N���X
    /// </summary>
    [System.Serializable]
    public class Slow_Add : I_AdditionalState
    {
        Vector3 verocity = Vector2.zero;
        [SerializeField] float SLOW_SPEED = 0.9f;

        public virtual void OnEnter()
        {
            Vector3 targetDirection = Quaternion.Euler(0.0f, instance._targetRotation, 0.0f) * Vector3.forward;
            verocity = targetDirection.normalized * instance._speed;
        }

        public virtual void OnUpdate()
        {
            //Move�œ�������������
            instance._speed = instance.playerMove.magnitude * instance.MOVE_SPEED;
            Vector3 targetDirection = Quaternion.Euler(0.0f, instance._targetRotation, 0.0f) * Vector3.forward;
            instance._controller.Move(-targetDirection.normalized * instance._speed * Time.deltaTime * SLOW_SPEED);
        }

        public virtual void OnExit()
        {

        }
    }
}