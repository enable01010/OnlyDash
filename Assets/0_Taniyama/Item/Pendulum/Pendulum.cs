using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] float HIT_STOP_TIME = 0.3f;
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
        if (isMove == false) return;

        nowPitch += Time.deltaTime;
        float rate = (Mathf.Sin(nowPitch * Mathf.PI * 2 / MOVE_PITCH) + 1) / 2.0f; //�s�b�`�̕b���ň������������쐬
        float angle = LibMath.GetValueToRange(rate, -MAX_ROTANGLE, MAX_ROTANGLE);
        transform.localEulerAngles = LibVector.Set_Z(transform.localEulerAngles, angle);
    }

    public void OnEnter_GeneralCollider(Collider other, GeneralColliderAttribute attribute) 
    {
        if (isMove == false) return;
        if (!other.TryGetComponent<I_PendulumHit>(out var hit)) return;

        // ���������̋������Z�o
        // (normalized�̏���������ۂɍ���������Ɛ�����тɂ����ӏ����������̂ō��������j
        Vector3 targetPos = LibVector.Set_Y(other.transform.position,0);
        Vector3 collisionPos = LibVector.Set_Y(_generalCollider.position, 0);
        Vector3 dir = targetPos - collisionPos;
        Vector3 impact = dir.normalized * MOVE_POWER;

        // �c�����̋������Z�o
        // ������normalized����ۂɐ��������̒l���~�������ߐ����������ēx�v�Z
        Vector3 @Dir = other.transform.position - _generalCollider.position;
        @Dir.y += ConstData.CHARACTER_HIGHT; // �L�����N�^�[�̌��_�������̂��ߍ����C��
        Vector3 @Impact = @Dir.normalized * MOVE_POWER;

        // ���������̒l�ɏc�����̒l�̂ݓK��
        impact.y = Mathf.Max(MIN_POWER_Y, @Impact.y);

        hit.HitPendulum(impact);

        //��莞�Ԓ�~
        LibCoroutineRunner.StartCoroutine(HitStop(HIT_STOP_TIME));
    }

    /// <summary>
    /// �ꎞ��~����
    /// </summary>
    /// <param name="stopTime">��~����</param>
    /// <returns>�R���[�`��</returns>
    private IEnumerator HitStop(float stopTime)
    {
        isMove = false;

        while(stopTime > 0)
        {
            stopTime -= Time.deltaTime;
            yield return null;
        }
        isMove = true;
    }
}

public interface I_PendulumHit
{
    public void HitPendulum(Vector3 power);
}