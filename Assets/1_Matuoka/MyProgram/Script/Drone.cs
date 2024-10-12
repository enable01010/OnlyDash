using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class Drone : MonoBehaviour
{
    #region Fields

    // スプライン
    private SplineContainer spline;

    // Splineの長さ
    private float splineLength;

    // 距離
    public float distance { get; private set; }

    // ドローン
    private GameObject droneObject;

    // 乗ってるか
    private bool isPlayerRide = false;

    // 乗れるか
    private bool canPlayerRide = true;

    // 傾けないか
    private bool isFreezeRotation = true;

    [field: SerializeField, Tooltip("進む向き")]
    public bool isDirectionPlus { get; private set; } = false;

    [SerializeField, Tooltip("移動スピード")]
    private float speed = 5f;

    [field: SerializeField, Tooltip("現在位置割合"), ReadOnly]
    public float nowRate { get; private set; }

    [Header("マテリアル")]
    [SerializeField]
    private Material[] material;

    enum ColorMaterial
    {
        Green = 0,
        Yellow,
        Red,
    }

    [SerializeField]
    private MeshRenderer droneMeshRenderer;
          

    #endregion


    #region MonoBehaviourMethod

    private void Start()
    {
        spline = GetComponent<SplineContainer>();
        splineLength = spline.CalculateLength();

        droneObject = transform.GetChild(0).gameObject;
        nowRate = isDirectionPlus ? 0f : 1f;
        droneObject.transform.position = spline.EvaluatePosition(nowRate);
        MoveRotation(isDirectionPlus);

        GameObject startObj = transform.GetChild(1).gameObject;
        startObj.transform.position = spline.EvaluatePosition(nowRate);

        GameObject endObj = transform.GetChild(2).gameObject;
        endObj.transform.position = spline.EvaluatePosition(((int)nowRate + 1) % 2);

        ChangeColor(ColorMaterial.Green);
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
    /// Playerが乗り始める
    /// </summary>
    /// <param name="isFreezeRotation"> 傾けないか </param>
    public void PlayerRideStart(bool isFreezeRotation)
    {
        ChangeColor(ColorMaterial.Yellow);
        isPlayerRide = true;

        this.isFreezeRotation = isFreezeRotation;
    }

    /// <summary>
    /// Playerが乗っているときの移動
    /// </summary>
    /// <param name="rate"> Splineの割合 </param>
    public void PlayerRideMove(float rate)
    {
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
        ChangeColor(ColorMaterial.Red);

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
        // 左右に回転しない(傾けない)
        if (isFreezeRotation == true)
        {
            // 親の回転を反映
            droneObject.transform.rotation = this.transform.rotation;

            droneObject.transform.rotation *= MoveQuaternion(isDirectionPlus);

            droneObject.transform.rotation = Quaternion.Euler(0, droneObject.transform.localEulerAngles.y, 0);
        }
        else
        {
            droneObject.transform.rotation = MoveQuaternion(isDirectionPlus);
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
        ChangeColor(ColorMaterial.Green);

        // 向きを戻す
        MoveRotation(isDirectionPlus);
    }

    /// <summary>
    /// 色の変更
    /// </summary>
    /// <param name="colorMaterial"></param>
    private void ChangeColor(ColorMaterial colorMaterial)
    {
        droneMeshRenderer.material = material[(int)colorMaterial];
    }

    #endregion
}