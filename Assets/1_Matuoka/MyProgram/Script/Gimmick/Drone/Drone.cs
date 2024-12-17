using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System;

public class Drone : MonoBehaviour
{
    #region Fields

    // �X�v���C��
    private SplineContainer spline;

    // Spline�̒���
    private float splineLength;

    // ����
    public float distance { get; private set; }

    // �h���[��
    private GameObject droneObject;

    // ����Ă邩
    private bool isPlayerRide = false;

    // ���邩
    private bool canPlayerRide = true;

    // �X���Ȃ���
    private bool isFreezeRotation = true;

    [field: SerializeField, Tooltip("�i�ތ���")]
    public bool isDirectionPlus { get; private set; } = false;

    [SerializeField, Tooltip("�ړ��X�s�[�h")]
    private float speed = 5f;

    [field: SerializeField, Tooltip("���݈ʒu����"), ReadOnly]
    public float nowRate { get; private set; }

    [Header("�}�e���A��")]
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
    /// ����Ƃ̋����v�Z
    /// </summary>
    /// <param name="pos"> Player��Pos </param>
    public void FixedRateDistanceUpdate(Vector3 pos)
    {
        if (spline == null) return;

        Vector3 splinPos = spline.EvaluatePosition(nowRate);

        distance = Vector3.Distance(pos, splinPos);
    }

    /// <summary>
    /// Player�����邩 true�Ȃ�K�[�h
    /// </summary>
    /// <returns></returns>
    public bool IsGuardPlayerRide()
    {
        return !canPlayerRide;
    }

    /// <summary>
    /// Player�����n�߂�
    /// </summary>
    /// <param name="isFreezeRotation"> �X���Ȃ��� </param>
    public void PlayerRideStart(bool isFreezeRotation)
    {
        ChangeColor(ColorMaterial.Yellow);
        isPlayerRide = true;

        this.isFreezeRotation = isFreezeRotation;
    }

    /// <summary>
    /// Player������Ă���Ƃ��̈ړ�
    /// </summary>
    /// <param name="rate"> Spline�̊��� </param>
    public void PlayerRideMove(float rate)
    {
        nowRate = rate;

        // ��]
        MoveRotation(isDirectionPlus);

        // �ړ�
        droneObject.transform.position = spline.EvaluatePosition(nowRate);
    }

    /// <summary>
    /// Player���~���
    /// </summary>
    /// <param name="rate"> ����spline�̊��� </param>
    public void PlayerRideEnd(float rate)
    {
        isPlayerRide = false;

        canPlayerRide = false;
        ChangeColor(ColorMaterial.Red);

        nowRate = rate;
    }

    /// <summary>
    /// NowRate�̍X�V
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
    /// Drone�̉�]
    /// </summary>
    private void MoveRotation(bool isDirectionPlus)
    {
        // ���E�ɉ�]���Ȃ�(�X���Ȃ�)
        if (isFreezeRotation == true)
        {
            // �e�̉�]�𔽉f
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
    /// Drone�̉�]�̂��߂̃N�H�[�^�j�I��
    /// </summary>
    /// <returns></returns>
    private Quaternion MoveQuaternion(bool isDirectionPlus)
    {
        // �X�v���C���̏�(�@��)�x�N�g��
        Vector3 up = spline.EvaluateUpVector(nowRate);

        // �X�v���C���̐ڐ��x�N�g��
        Vector3 forward = spline.EvaluateTangent(nowRate);
        if (isDirectionPlus != true) forward *= -1f;

        // �N�H�[�^�j�I���v�Z
        Quaternion quaternion = Quaternion.LookRotation(forward, up);
        return quaternion;
    }

    /// <summary>
    /// �ړ�
    /// </summary>
    private void MovePos()
    {
        droneObject.transform.position = spline.EvaluatePosition(nowRate);
    }

    /// <summary>
    /// Drone��Update true�Ȃ�K�[�h
    /// </summary>
    /// <returns></returns>
    private bool IsGuardUpdate()
    {
        return isPlayerRide || canPlayerRide;
    }

    /// <summary>
    /// Drone���߂������`�F�b�N�Ə���
    /// </summary>
    private void DroneReturnCheck()
    {
        if (nowRate != 0f && nowRate != 1f) return;

        canPlayerRide = true;
        ChangeColor(ColorMaterial.Green);

        // ������߂�
        MoveRotation(isDirectionPlus);
    }

    /// <summary>
    /// �F�̕ύX
    /// </summary>
    /// <param name="colorMaterial"></param>
    private void ChangeColor(ColorMaterial colorMaterial)
    {
        droneMeshRenderer.material = material[(int)colorMaterial];
    }

    #endregion
}