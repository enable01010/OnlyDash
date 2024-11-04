using System.Collections;
using UnityEngine;

public class BreakGround_Stay : MonoBehaviour, I_GeneralColliderUser
{
    [SerializeField] float BREAK_TIME = 5.0f;
    [SerializeField, ReadOnly] float nowBreakTime = 0;
    [SerializeField] float REPOP_TIME = 5.0f;


    public virtual void OnEnter_GeneralCollider(Collider other)
    {
        if (!other.TryGetComponent<I_BreakGround>(out _)) return;

        nowBreakTime = 0;
    }

    public virtual void OnStay_GeneralCollider(Collider other)
    {
        nowBreakTime += Time.deltaTime;
        if (nowBreakTime > BREAK_TIME)
        {
            Break();
        }
    }

    /// <summary>
    /// 壊れる時の処理
    /// 追加演出のために追記する可能性が高いので関数化
    /// </summary>
    private void Break()
    {
        LibCoroutineRunner.StartCoroutine(Repop(REPOP_TIME));
    }

    /// <summary>
    /// 再出現処理
    /// </summary>
    /// <returns>コルーチン</returns>
    private IEnumerator Repop(float repopTime)
    {
        gameObject.SetActive(false);

        while (repopTime > 0)
        {
            yield return null;
            repopTime -= Time.deltaTime;
        }

        gameObject.SetActive(true);
    }
}

public interface I_BreakGround
{

}