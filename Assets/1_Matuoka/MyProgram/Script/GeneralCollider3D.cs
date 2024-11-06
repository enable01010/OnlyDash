using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GeneralCollider3D : MonoBehaviour
{
    private Action<Collider, Transform> onEnter;
    private Action<Collider, Transform> onStay;
    private Action<Collider, Transform> onExit;

    private void Awake()
    {
        // 親オブジェクトから自動検索
        if(onEnter == null && onStay == null && onExit == null)
            Init(transform.GetComponentInParent<I_GeneralColliderUser>());

        // トリガーつけ忘れ用
        GetComponent<Collider>().isTrigger = true;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="owner">呼び出し元</param>
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
            onEnter(other, this.transform);
    }

    private void OnTriggerStay(Collider other)
    {
        if (onStay != null)
            onStay(other, this.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (onExit != null)
            onExit(other, this.transform);
    }
}

/// <summary>
/// ジェネラルコライダーを使うようのインターフェース
/// </summary>
public interface I_GeneralColliderUser
{
    public void OnEnter_GeneralCollider(Collider other, Transform generalCollider) { }
    public void OnStay_GeneralCollider(Collider other, Transform generalCollider) { }
    public void OnExit_GeneralCollider(Collider other, Transform generalCollider) { }
    //public void OnEnter_GeneralCollider(Collider other) { }
    //public void OnStay_GeneralCollider(Collider other) { }
    //public void OnExit_GeneralCollider(Collider other) { }
}
