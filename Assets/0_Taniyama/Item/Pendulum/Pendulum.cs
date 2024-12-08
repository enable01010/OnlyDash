using System.Collections;
using UnityEngine;

public class Pendulum : MonoBehaviour, I_GeneralColliderUser
{
    [SerializeField,Range(-1,1)] float START_PITCH = 0;
    [SerializeField] float MOVE_PITCH = 3.0f;
    [SerializeField] float MAX_ROTANGLE = 60f;
    [SerializeField, ReadOnly] float nowPitch;
    bool isMove = true;
    [SerializeField] float MOVE_POWER = 5.0f;
    [SerializeField] float MIN_POWER_Y = 1.0f;
    [SerializeField] float HIT_STOP_TIME = 0.15f;
    [SerializeField] float HIT_STOP_REMOVE_TIME = 0.1f;
    public const float PENDULUM_SPEED_SLOW = 0.99f;

    Transform _generalCollider;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        float rate = (START_PITCH + 1) / 2.0f; ; 
        float angle = LibMath.GetValueToRange(rate, -MAX_ROTANGLE, MAX_ROTANGLE);
        transform.localEulerAngles = LibVector.Set_Z(transform.localEulerAngles, angle);
    }
#endif

    void Awake()
    {
        _generalCollider = GetComponentInChildren<GeneralCollider3D>().transform;
        nowPitch = MOVE_PITCH / 4 * START_PITCH;
    }

    void Update()
    {
        nowPitch += Time.deltaTime;

        if (isMove == false) return;
        float rate = (Mathf.Sin(nowPitch * Mathf.PI * 2 / MOVE_PITCH) + 1) / 2.0f; //ピッチの秒数で一周する周期を作成
        float angle = LibMath.GetValueToRange(rate, -MAX_ROTANGLE, MAX_ROTANGLE);
        transform.localEulerAngles = LibVector.Set_Z(transform.localEulerAngles, angle);
    }

    public void OnEnter_GeneralCollider(Collider other, GeneralColliderAttribute attribute) 
    {
        if (isMove == false) return;
        if (!other.TryGetComponent<I_PendulumHit>(out var hit)) return;

        // 水平方向の強さを算出
        // (normalizedの処理をする際に高さがあると吹っ飛びにくい箇所があったので高さ無視）
        Vector3 targetPos = LibVector.Set_Y(other.transform.position,0);
        Vector3 collisionPos = LibVector.Set_Y(_generalCollider.position, 0);
        Vector3 dir = targetPos - collisionPos;
        Vector3 impact = dir.normalized * MOVE_POWER;

        // 縦方向の強さを算出
        // 高さはnormalizedする際に水平方向の値も欲しいため水平方向も再度計算
        Vector3 @Dir = other.transform.position - _generalCollider.position;
        @Dir.y += ConstData.CHARACTER_HIGHT; // キャラクターの原点が足元のため高さ修正
        Vector3 @Impact = @Dir.normalized * MOVE_POWER;

        // 水平方向の値に縦方向の値のみ適応
        impact.y = Mathf.Max(MIN_POWER_Y, @Impact.y);

        hit.HitPendulum(impact);

        // 一定時間停止
        LibCoroutineRunner.StartCoroutine(HitStop(HIT_STOP_TIME, HIT_STOP_REMOVE_TIME));

        // カメラの揺れ
        LibCameraAction.Shake();
    }

    /// <summary>
    /// 一時停止処理
    /// </summary>
    /// <param name="stopTime">停止時間</param>
    /// <returns>コルーチン</returns>
    private IEnumerator HitStop(float stopTime,float removeTime)
    {
        isMove = false;

        while(stopTime > 0)
        {
            stopTime -= Time.deltaTime;
            yield return null;
        }

        while (removeTime > 0)
        {
            removeTime -= Time.deltaTime;

            // ヒットストップで遅れた位置を規定時間かけてもとに戻す
            float removeRate = LibMath.OneMinus(removeTime / HIT_STOP_REMOVE_TIME);
            float nowAngle = transform.localEulerAngles.z;

            float rate = (Mathf.Sin(nowPitch * Mathf.PI * 2 / MOVE_PITCH) + 1) / 2.0f; //ピッチの秒数で一周する周期を作成
            float nonStopedAngle = LibMath.GetValueToRange(rate, -MAX_ROTANGLE, MAX_ROTANGLE);

            float nextAngle = Mathf.Lerp(LibMath.ClampAngle180(nowAngle), LibMath.ClampAngle180(nonStopedAngle), removeRate);
            transform.localEulerAngles = LibVector.Set_Z(transform.localEulerAngles, nextAngle);

            yield return null;
        }

        isMove = true;
    }
}

public interface I_PendulumHit
{
    public void HitPendulum(Vector3 power);
}