using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class SplineNearestPos : MonoBehaviour
{
    #region Fields

    // スプライン
    [SerializeField] private SplineContainer spline;

    // 入力位置のゲームオブジェクト
    [SerializeField, ReadOnly] private Transform inputObject;
    private Vector3 offsetPos = new Vector3(0f, 1.2f, 0f);

    // 出力位置（直近位置）を反映するゲームオブジェクト
    [SerializeField] private Transform outputObject;

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

    //最短の距離
    public float distance;

    #endregion


    #region MonoBehaviourMethod

    private void Start()
    {
        inputObject = GameObject.FindGameObjectWithTag("Player").transform;

        DistanceUpdate();
    }

    private void Update()
    {
        DistanceUpdate();
    }

    private void FixedUpdate()
    {

    }

    #endregion


    #region CustomMethod

    // 距離計算
    private void DistanceUpdate()
    {
        if (spline == null || inputObject == null) return;

        // ワールド空間におけるスプラインを取得
        // スプラインはローカル空間なので、ローカル→ワールド変換行列を掛ける
        // Updateを抜けるタイミングでDisposeされる
        NativeSpline tempSpline = new(spline.Spline, spline.transform.localToWorldMatrix);

        // スプラインにおける直近位置を求める
        float dis = SplineUtility.GetNearestPoint(
            tempSpline,
            inputObject.position + offsetPos,
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