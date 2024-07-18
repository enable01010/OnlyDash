using UnityEngine;
using UnityEngine.Splines;

public class NearestPointExample : MonoBehaviour
{
    // �X�v���C��
    [SerializeField] private SplineContainer _spline;

    // ���͈ʒu�̃Q�[���I�u�W�F�N�g
    [SerializeField] private Transform _inputPoint;

    // �o�͈ʒu�i���߈ʒu�j�𔽉f����Q�[���I�u�W�F�N�g
    [SerializeField] private Transform _outputPoint;

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


    public float rate;
    public float distance;

    private void Start()
    {
        _inputPoint = GameObject.Find("Player").transform;

        DistanceUpdate();
    }


    private void Update()
    {
        DistanceUpdate();
    }

    private void DistanceUpdate()
    {
        // Null�`�F�b�N
        if (_spline == null || _inputPoint == null/* || _outputPoint == null*/)
            return;

        // ���[���h��Ԃɂ�����X�v���C�����擾
        // �X�v���C���̓��[�J����ԂȂ̂ŁA���[�J�������[���h�ϊ��s����|����
        // Update�𔲂���^�C�~���O��Dispose�����
        using var spline = new NativeSpline(_spline.Spline, _spline.transform.localToWorldMatrix);

        // �X�v���C���ɂ����钼�߈ʒu�����߂�
        var dis = SplineUtility.GetNearestPoint(
            spline,
            _inputPoint.position,
            out var nearest,
            out var t,
            _resolution,
            _iterations
        );

        // ���ʂ𔽉f
        if (_outputPoint != null) _outputPoint.position = nearest;



        rate = Mathf.Clamp01(t);
        Vector3 temp = nearest;
        distance = Vector3.Distance(temp, _inputPoint.position);
    }

}