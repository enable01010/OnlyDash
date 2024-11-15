using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GeneralCollider3D : MonoBehaviour
{
    private Action<Collider, GeneralColliderAttribute> onEnter;
    private Action<Collider, GeneralColliderAttribute> onStay;
    private Action<Collider, GeneralColliderAttribute> onExit;
    private GeneralColliderAttribute attribute;

    private void Awake()
    {
        // �����p�ϐ��̍쐬
        if(attribute == null)
        {
            SetAttribute(new GeneralColliderAttribute());
        }

        // �e�I�u�W�F�N�g���玩������
        if (onEnter == null && onStay == null && onExit == null)
        {
            Init(transform.GetComponentInParent<I_GeneralColliderUser>());
        }

        // �g���K�[���Y��p
        GetComponent<Collider>().isTrigger = true;
    }

    /// <summary>
    /// ������
    /// </summary>
    /// <param name="owner">�Ăяo����</param>
    public void Init(I_GeneralColliderUser owner)
    {
        if (owner == null) return;
        
        onEnter = owner.OnEnter_GeneralCollider;
        onStay = owner.OnStay_GeneralCollider;
        onExit = owner.OnExit_GeneralCollider;
    }

    public void SetAttribute(GeneralColliderAttribute attribute)
    {
        this.attribute = attribute;
        this.attribute.AddGeneralCollider3D(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(onEnter != null)
            onEnter(other, attribute);
    }

    private void OnTriggerStay(Collider other)
    {
        if (onStay != null)
            onStay(other, attribute);
    }

    private void OnTriggerExit(Collider other)
    {
        if (onExit != null)
            onExit(other, attribute);
    }
}

/// <summary>
/// �W�F�l�����R���C�_�[���g���悤�̃C���^�[�t�F�[�X
/// </summary>
public interface I_GeneralColliderUser
{
    /// <summary>
    /// �R���C�_�[�ɐG��n�߂��ۂɌĂ΂��
    /// </summary>
    /// <param name="other">���������I�u�W�F�N�g</param>
    /// <param name="attribute">�ǉ����</param>
    public void OnEnter_GeneralCollider(Collider other, GeneralColliderAttribute attribute) { }

    /// <summary>
    /// �R���C�_�[�ɑ����Ă���ۂɌĂ΂��
    /// </summary>
    /// <param name="other">���������I�u�W�F�N�g</param>
    /// <param name="attribute">�ǉ����</param>
    public void OnStay_GeneralCollider(Collider other, GeneralColliderAttribute attribute) { }

    /// <summary>
    /// �R���C�_�[���痣�ꂽ�ۂɌĂ΂��
    /// </summary>
    /// <param name="other">���������I�u�W�F�N�g</param>
    /// <param name="attribute">�ǉ����</param>
    public void OnExit_GeneralCollider(Collider other, GeneralColliderAttribute attribute) { }
}


/// <summary>
/// GeneralCollider3D�p�̈����p�N���X
/// </summary>
public class GeneralColliderAttribute
{
    public GeneralCollider3D gc3d;
    public Transform transform;
    public GameObject gameObject;

    public void AddGeneralCollider3D(GeneralCollider3D gc3d)
    {
        this.gc3d = gc3d;
        transform = gc3d.transform;
        gameObject = gc3d.gameObject;
    }
}