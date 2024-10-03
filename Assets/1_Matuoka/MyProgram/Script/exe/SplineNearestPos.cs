using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class SplineNearestPos : MonoBehaviour
{
    #region Fields

    // スプライン
    private SplineContainer spline;

    // 出力位置（直近位置）を反映するゲームオブジェクト
    private Transform outputObject;

    // 解像度
    // 内部的にPickResolutionMin～PickResolutionMaxの範囲に丸められる
    [SerializeField]
    [Range(SplineUtility.PickResolutionMin, SplineUtility.PickResolutionMax)]
    private int _resolution = 4;

    // 計算回数
    // 内部的に10回以下に丸められる
    [SerializeField]
    [Range(1, 10)]
    private int _iterations = 2;

    // 位置の割合
    public float rate;

    // 最短の距離
    public float distance;

    #endregion


    #region MonoBehaviourMethod

    private void Start()
    {
        spline = GetComponent<SplineContainer>();
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {

    }

    #endregion


    #region CustomMethod

    // 最短距離計算
    public void NearestDistanceUpdate(Vector3 pos)
    {
        if (spline == null) return;

        // ワールド空間におけるスプラインを取得
        // スプラインはローカル空間なので、ローカル→ワールド変換行列を掛ける
        // Updateを抜けるタイミングでDisposeされる
        NativeSpline tempSpline = new(spline.Spline, spline.transform.localToWorldMatrix);

        // スプラインにおける直近位置を求める
        float dis = SplineUtility.GetNearestPoint(
            tempSpline,
            pos,
            out float3 nearest,
            out float t,
            _resolution,
            _iterations
        );

        // 結果を反映
        rate = Mathf.Clamp01(t);
        distance = dis;

        // 出力位置（直近位置）を反映するゲームオブジェクト
        if (outputObject != null) outputObject.position = nearest;
    }

    #endregion
}