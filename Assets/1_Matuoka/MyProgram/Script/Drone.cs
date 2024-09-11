using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class Drone : MonoBehaviour
{
    #region Fields

    // スプライン
    private SplineContainer spline;

    // 位置の割合
    //public float rate;

    //最短の距離
    public float distance;

    // ドローン
    private GameObject droneObject;

    // 乗ってるか
    private bool isPlayerRide = false;

    // 乗れるか
    private bool canPlayerRide = false;

    [Header("Spline")]
    private float splineLength;

    [field:Header("乗り始めるとき")]
    [SerializeField, Tooltip("進む向き")]
    public bool isDirectionPlus { get; private set; } = false;

    [field: Header("移動時")]
    [SerializeField, Tooltip("現在位置割合")]
    public float nowRate { get; private set; }

    [Header("移動時")]
    [SerializeField, Tooltip("移動スピード")]
    private float speed = 5f;

    [SerializeField, Tooltip("傾けるか")]
    private bool isFreezeRotation = false;

    #endregion


    #region MonoBehaviourMethod

    private void Start()
    {
        spline = GetComponent<SplineContainer>();
        splineLength = spline.CalculateLength();

        droneObject = transform.GetChild(0).gameObject;
        nowRate = isDirectionPlus ? 0f : 1f;
        droneObject.transform.position = spline.EvaluatePosition(nowRate);

        GameObject startObj = transform.GetChild(1).gameObject;
        startObj.transform.position = spline.EvaluatePosition(nowRate);

        GameObject endObj = transform.GetChild(2).gameObject;
        endObj.transform.position = spline.EvaluatePosition(((int)nowRate + 1) % 2);
    }

    private void Update()
    {
        if (IsGuardUpdate() == true) return;

        NowRateUpdate(!isDirectionPlus);
        MoveRotation(!isDirectionPlus);
        MovePos();

        DroneReturnCheck();
    }

    private void FixedUpdate()
    {

    }

    #endregion


    #region CustomMethod

    /// <summary>
    /// 乗り場との距離計算
    /// </summary>
    /// <param name="pos"> PlayerのPos </param>
    public void FixedRateDistanceUpdate(Vector3 pos)
    {
        if (spline == null) return;

        Vector3 splinPos = spline.EvaluatePosition(nowRate);

        distance = Vector3.Distance(pos, splinPos);
    }

    /// <summary>
    /// Playerが乗れるか trueならガード
    /// </summary>
    /// <returns></returns>
    public bool IsGuardPlayerRide()
    {
        return !canPlayerRide;
    }

    /// <summary>
    /// Playerが乗っているときの移動
    /// </summary>
    /// <param name="rate"> Splineの割合 </param>
    public void PlayerRideMove(float rate)
    {
        isPlayerRide = true;

        nowRate = rate;

        // 回転
        MoveRotation(isDirectionPlus);

        // 移動
        droneObject.transform.position = spline.EvaluatePosition(nowRate);
    }

    /// <summary>
    /// Playerが降りる
    /// </summary>
    /// <param name="rate"> 今のsplineの割合 </param>
    public void PlayerRideEnd(float rate)
    {
        isPlayerRide = false;

        canPlayerRide = false;

        nowRate = rate;
    }

    /// <summary>
    /// NowRateの更新
    /// </summary>
    private void NowRateUpdate(bool isDirectionPlus)
    {
        if (isDirectionPlus == true)
        {
            nowRate += (speed / splineLength) * Time.deltaTime;
        }
        else
        {
            nowRate -= (speed / splineLength) * Time.deltaTime;
        }

        nowRate = Mathf.Clamp01(nowRate);
    }

    /// <summary>
    /// Droneの回転
    /// </summary>
    private void MoveRotation(bool isDirectionPlus)
    {
        droneObject.transform.rotation = MoveQuaternion(isDirectionPlus);

        // Playerを左右に回転しない(傾けない)
        if (isFreezeRotation == true)
        {
            droneObject.transform.rotation = Quaternion.Euler(0, droneObject.transform.localEulerAngles.y, 0);
        }
    }

    /// <summary>
    /// Droneの回転のためのクォータニオン
    /// </summary>
    /// <returns></returns>
    private Quaternion MoveQuaternion(bool isDirectionPlus)
    {
        // スプラインの上(法線)ベクトル
        Vector3 up = spline.EvaluateUpVector(nowRate);

        // スプラインの接線ベクトル
        Vector3 forward = spline.EvaluateTangent(nowRate);
        if (isDirectionPlus != true) forward *= -1f;

        // クォータニオン計算
        Quaternion quaternion = Quaternion.LookRotation(forward, up);
        return quaternion;
    }

    /// <summary>
    /// 移動
    /// </summary>
    private void MovePos()
    {
        droneObject.transform.position = spline.EvaluatePosition(nowRate);
    }

    /// <summary>
    /// DroneのUpdate trueならガード
    /// </summary>
    /// <returns></returns>
    private bool IsGuardUpdate()
    {
        return isPlayerRide || canPlayerRide;
    }

    /// <summary>
    /// Droneが戻ったかチェックと処理
    /// </summary>
    private void DroneReturnCheck()
    {
        if (nowRate != 0f && nowRate != 1f) return;

        canPlayerRide = true;

        // 向きを戻す
        Vector3 up = spline.EvaluateUpVector(nowRate);
        Vector3 forward = droneObject.transform.forward * -1f;

        droneObject.transform.rotation = Quaternion.LookRotation(forward, up);
    }

    #endregion
}