using UnityEngine;
using static Player;

public class IceGround : MonoBehaviour
{
    [SerializeField] Ice_Add iceParameter;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<I_IceGroundMover>(out var hit))
        {
            hit.InGround(iceParameter);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<I_IceGroundMover>(out var hit))
        {
            hit.OutGround(iceParameter);
        }
    }
}

/// <summary>
/// 氷の上で滑るオブジェクト用のインターフェース
/// </summary>
public interface I_IceGroundMover
{
    /// <summary>
    /// 氷の上に乗り始めた時の処理
    /// </summary>
    public void InGround(Ice_Add ice);

    /// <summary>
    /// 氷の上から降りた際に処理
    /// </summary>
    public void OutGround(Ice_Add ice);
}


public partial class Player : SingletonActionListener<Player>
{
    /// <summary>
    /// 氷の上にいる際の処理用クラス
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
            //Moveで動いた分を消す
            instance._speed = instance.playerMove.magnitude * instance.MOVE_SPEED;
            Vector3 targetDirection = Quaternion.Euler(0.0f, instance._targetRotation, 0.0f) * Vector3.forward;
            instance._controller.Move(-targetDirection.normalized * instance._speed * Time.deltaTime);

            //移動方向作成
            Vector3 inputMove = targetDirection.normalized * instance._speed;
            verocity = Vector3.Lerp(verocity, inputMove, LERP_SPEED);

            //移動実施
            instance._controller.Move(verocity * Time.deltaTime);
        }

        public virtual void OnExit()
        {

        }
    }
}
