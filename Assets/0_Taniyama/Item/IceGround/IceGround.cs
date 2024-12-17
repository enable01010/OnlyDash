using System.Collections.Generic;
using UnityEngine;
using static Player;

public class IceGround : MonoBehaviour, I_GeneralColliderUser
{
    List<GeneralCollider3D> gc3d = new List<GeneralCollider3D>();
    [SerializeField] Ice_Add iceParameter;

    public void OnEnter_GeneralCollider(Collider other, GeneralColliderAttribute attribute)
    {
        gc3d.Add(attribute.gc3d);
        if (gc3d.Count == 1)
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
        if (other.TryGetComponent<I_IceGroundMover>(out var hit))
        {
            hit.IceAreaInGround(iceParameter);
        }
    }

    /// <summary>
    /// �X���[�̃G���A�ɏo���ۂ̏���
    /// </summary>
    private void EndArea(Collider other)
    {
        if (other.TryGetComponent<I_IceGroundMover>(out var hit))
        {
            hit.IceAreaOutGround(iceParameter);
        }
    }
}

/// <summary>
/// �X�̏�Ŋ���I�u�W�F�N�g�p�̃C���^�[�t�F�[�X
/// </summary>
public interface I_IceGroundMover
{
    /// <summary>
    /// �X�̏�ɏ��n�߂����̏���
    /// </summary>
    public void IceAreaInGround(Ice_Add ice);

    /// <summary>
    /// �X�̏ォ��~�肽�ۂɏ���
    /// </summary>
    public void IceAreaOutGround(Ice_Add ice);
}


public partial class Player : SingletonActionListener<Player>
{
    /// <summary>
    /// �X�̏�ɂ���ۂ̏����p�N���X
    /// </summary>
    [System.Serializable]
    public class Ice_Add : I_AdditionalState
    {
        Vector3 verocity = Vector2.zero;
        [SerializeField] float LERP_SPEED = 0.9f;

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
            instance._controller.Move(-targetDirection.normalized * instance._speed * Time.deltaTime);

            //�ړ������쐬
            Vector3 inputMove = targetDirection.normalized * instance._speed;
            verocity = Vector3.Lerp(verocity, inputMove, LERP_SPEED);

            //�ړ����{
            instance._controller.Move(verocity * Time.deltaTime);
        }

        public virtual void OnExit()
        {

        }
    }
}
