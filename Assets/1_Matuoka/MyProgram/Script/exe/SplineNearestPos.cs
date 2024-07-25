using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class SplineNearestPos : MonoBehaviour
{
    #region Fields

    // �X�v���C��
    [SerializeField] private SplineContainer spline;

    // ���͈ʒu�̃Q�[���I�u�W�F�N�g
    [SerializeField, ReadOnly] private Transform inputObject;
    private Vector3 offsetPos = new Vector3(0f, 1.2f, 0f);

    // �o�͈ʒu�i���߈ʒu�j�𔽉f����Q�[���I�u�W�F�N�g
    [SerializeField] private Transform outputObject;

    // �𑜓x
    // �����I��PickResolutionMin�`PickResolutionMax�͈̔͂Ɋۂ߂���
    [SerializeField]
    [Range(SplineUtility.PickResolutionMin, SplineUtility.PickResolutionMax)]
    private int _resolution = 4;

    // �v�Z��
    // �����I��10��ȉ��Ɋۂ߂���
    [SerializeField]
    [Range(1, 10)]
    private int _iterations = 2;

    // �ʒu�̊���
    public float rate;

    //�ŒZ�̋���
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

    // �����v�Z
    private void DistanceUpdate()
    {
        if (spline == null || inputObject == null) return;

        // ���[���h��Ԃɂ�����X�v���C�����擾
        // �X�v���C���̓��[�J����ԂȂ̂ŁA���[�J�������[���h�ϊ��s����|����
        // Update�𔲂���^�C�~���O��Dispose�����
        NativeSpline tempSpline = new(spline.Spline, spline.transform.localToWorldMatrix);

        // �X�v���C���ɂ����钼�߈ʒu�����߂�
        float dis = SplineUtility.GetNearestPoint(
            tempSpline,
            inputObject.position + offsetPos,
            out float3 nearest,
            out float t,
            _resolution,
            _iterations
        );

        // ���ʂ𔽉f
        rate = Mathf.Clamp01(t);
        distance = dis;

        // �o�͈ʒu�i���߈ʒu�j�𔽉f����Q�[���I�u�W�F�N�g
        if (outputObject != null) outputObject.position = nearest;
    }

    #endregion
}