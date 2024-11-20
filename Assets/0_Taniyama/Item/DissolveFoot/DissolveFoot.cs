using System.Collections;
using UnityEngine;

public class DissolveFoot : MonoBehaviour,I_GeneralColliderUser
{
    [SerializeField] float DISSOLVE_TIME = 0.5f;
    [SerializeField] float ANIM_MOVE_LENGHT = 0.3f;
    [SerializeField] float SERACH_AREA_LENGTH = 5.0f;
    Vector3 START_POS;

    private Collider col;
    private Material mat;
    private const string MATERIAL_PASS = "_clipTimer";

    bool isStartDissolve = false;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 判定距離を設定
        var collider = GetComponentInChildren<SphereCollider>();
        if(collider != null)
        {
            collider.radius = SERACH_AREA_LENGTH;
        }
    }

#endif

    private void Awake()
    {
        START_POS = transform.position;
        col = GetComponent<Collider>();
        mat = GetComponent<MeshRenderer>().material;

        // 最初は非表示にする
        col.enabled = false;
        SetDissolve(1);
    }

    public void OnEnter_GeneralCollider(Collider other, GeneralColliderAttribute attribute) 
    {
        if (isStartDissolve == true) return;

        isStartDissolve = true;
        LibCoroutineRunner.StartCoroutine(StartDissolve(DISSOLVE_TIME));
    }


    public void OnExit_GeneralCollider(Collider other, GeneralColliderAttribute attribute) 
    {
        if (isStartDissolve == false) return;

        isStartDissolve = false;
        LibCoroutineRunner.StartCoroutine(EndDissolve(DISSOLVE_TIME));
    }

    /// <summary>
    /// 登場演出
    /// </summary>
    /// <returns>コルーチン</returns>
    private IEnumerator StartDissolve(float animTime)
    {
        col.enabled = true;
        SetDissolve(0);

        // 初期位置の調整
        Vector3 randomPos = CreateRandomPos();
        //transform.position = randomPos;

        float nowTimeRate = 0;

        while (animTime > 0)
        {
            yield return null;

            // 時間計測
            animTime -= Time.deltaTime;
            nowTimeRate = animTime / DISSOLVE_TIME;

            // ディゾルブ処理
            SetDissolve(nowTimeRate);

            // ポジション処理
            SetPosition(randomPos,nowTimeRate);
        }
    }

    /// <summary>
    /// 退場演出
    /// </summary>
    /// <returns>コルーチン</returns>
    private IEnumerator EndDissolve(float animTime)
    {
        // 初期位置の調整
        Vector3 randomPos = CreateRandomPos();
        //transform.position = randomPos;
        SetDissolve(1);

        float nowTimeRate = 0;

        while (animTime > 0)
        {
            yield return null;

            // 時間計測
            animTime -= Time.deltaTime;
            nowTimeRate = LibMath.OneMinus(animTime / DISSOLVE_TIME);

            // ディゾルブ処理
            SetDissolve(nowTimeRate);

            // ポジション処理
            SetPosition(randomPos,nowTimeRate);
        }

        transform.position = START_POS;
        col.enabled = false;
    }

    /// <summary>
    /// ランダムな移動地点を作成
    /// </summary>
    /// <returns>作成された移動地点</returns>
    private Vector3 CreateRandomPos()
    {
        float theta = Random.Range(-90f, 90f);
        Vector3 rondomPos = START_POS + Quaternion.AngleAxis(theta, transform.forward) * (Vector3.up * ANIM_MOVE_LENGHT);
        return rondomPos;
    }

    /// <summary>
    /// シェーダーにディゾルブの値を渡す処理
    /// </summary>
    /// <param name="value">ディゾルブの割合　0：透明　1：透明じゃない</param>
    private void SetDissolve(float value)
    {
        mat.SetFloat(MATERIAL_PASS, value);
    }

    /// <summary>
    /// ポジションを設定する処理
    /// </summary>
    /// <param name="goalPos">ランダム地点</param>
    /// <param name="value">地点の割合　0：初期位置　1：ランダム位置</param>
    private void SetPosition(Vector3 randomPos,float value)
    {
        // TODO:演出に移動アニメーションを作成する場合ここを復活させる。
        // ただし、移動アニメーションに際にキャラクターが範囲から外れるとバグの原因になるためそこを解消する
        // 作成当初は運用が明確ではないため登場時に移動させない方向で対応

        //Vector3 targetPos = Vector3.Lerp(START_POS, randomPos, value);
        //transform.position = targetPos;
    }
}
