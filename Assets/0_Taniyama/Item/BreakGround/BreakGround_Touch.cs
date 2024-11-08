using System.Collections;
using UnityEngine;

public class BreakGround_Touch: MonoBehaviour ,I_GeneralColliderUser
{
    [SerializeField] float BREAK_TIME = 5.0f;
    [SerializeField, ReadOnly] float nowBreakTime = 0;
    [SerializeField] float REPOP_TIME = 5.0f;

    bool isTouched = false;

    public virtual void OnEnter_GeneralCollider(Collider other, Transform generalCollider) {
        if (!other.TryGetComponent<I_BreakGround>(out _)) return;

        nowBreakTime = 0;

        LibFuncUtility.WhenFlaseDoAndReverse(ref isTouched, () => LibCoroutineRunner.StartCoroutine(StartBreakTimer()));
    }

    /// <summary>
    /// タイマー稼働処理
    /// </summary>
    /// <returns>コルーチン</returns>
    private IEnumerator StartBreakTimer()
    {
        nowBreakTime = 0;

        while(nowBreakTime < BREAK_TIME)
        {
            yield return null;
            nowBreakTime += Time.deltaTime;
        }

        Break();
    }

    /// <summary>
    /// 壊れる時の処理
    /// 追加演出のために追記する可能性が高いので関数化
    /// </summary>
    private void Break()
    {
        isTouched = false;
        LibCoroutineRunner.StartCoroutine(Repop(REPOP_TIME));
    }
    
    /// <summary>
    /// 再出現処理
    /// </summary>
    /// <returns>コルーチン</returns>
    private IEnumerator Repop(float repopTime)
    {
        gameObject.SetActive(false);

        while(repopTime > 0)
        {
            yield return null;
            repopTime -= Time.deltaTime;
        }

        gameObject.SetActive(true);
    }


}
