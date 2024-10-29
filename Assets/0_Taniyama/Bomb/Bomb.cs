using UnityEngine;
using System.Collections;

/// <summary>
/// 爆弾オブジェクト
/// </summary>
public class Bomb : MonoBehaviour
{
    const float PREFAB_DESTROY_TIME = 5.0f;
    [SerializeField] float POWER = 10;
    [SerializeField] float REPOP_TIME = 5.0f;
    public const float BOMB_SPEED_SLOW = 0.99f;

    private void OnTriggerEnter(Collider other)
    {
        // ボムが当たる対象の場合のみ処理
        if (!other.TryGetComponent<I_BombHit>(out var hit)) return;

        // 爆発の移動方向と強さを算出
        Vector3 dir = other.transform.position - transform.position;
        dir.y += ConstData.CHARACTER_HIGHT / 2; //キャラクターの原点が足元のため高さ修正
        Vector3 impact = dir.normalized * POWER;
        hit.BombHit(impact);

        // 当たった際のエフェクト作成
        GameObject instance = Instantiate(LibResourceLoader._bombFxPref);
        instance.transform.position = transform.position;

        //廃棄処理
        Destroy(instance, PREFAB_DESTROY_TIME);
        LibCoroutineRunner.StartCoroutine(Repop(REPOP_TIME));
    }

    /// <summary>
    /// リポップ処理
    /// </summary>
    /// <param name="repopTime"></param>
    /// <returns></returns>
    private IEnumerator Repop(float repopTime)
    {
        gameObject.SetActive(false);

        while(repopTime > 0)
        {
            repopTime -= Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(true);
    }
}


/// <summary>
/// 爆弾オブジェクトの影響を受ける
/// </summary>
public interface I_BombHit
{

    /// <summary>
    /// 爆発に当たった時の処理
    /// </summary>
    public void BombHit(Vector3 power);
}
