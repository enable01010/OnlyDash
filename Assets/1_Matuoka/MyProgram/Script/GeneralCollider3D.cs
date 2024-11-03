using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GeneralCollider3D : MonoBehaviour
{
    private Action<Collider> onEnter;
    private Action<Collider> onStay;
    private Action<Collider> onExit;

    private void Awake()
    {
        // �e�I�u�W�F�N�g���玩������
        if(onEnter == null && onStay == null && onExit == null)
            Init(transform.root.GetComponentInChildren<I_GeneralColliderUser>());

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

    private void OnTriggerEnter(Collider other)
    {
        if(onEnter != null)
            onEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (onStay != null)
            onStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (onExit != null)
            onExit(other);
    }
}

/// <summary>
/// �W�F�l�����R���C�_�[���g���悤�̃C���^�[�t�F�[�X
/// </summary>
public interface I_GeneralColliderUser
{
    public void OnEnter_GeneralCollider(Collider other) { }
    public void OnStay_GeneralCollider(Collider other) { }
    public void OnExit_GeneralCollider(Collider other) { }
}
