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
        // ���苗����ݒ�
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

        // �ŏ��͔�\���ɂ���
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
    /// �o�ꉉ�o
    /// </summary>
    /// <returns>�R���[�`��</returns>
    private IEnumerator StartDissolve(float animTime)
    {
        col.enabled = true;
        SetDissolve(0);

        // �����ʒu�̒���
        Vector3 randomPos = CreateRandomPos();
        //transform.position = randomPos;

        float nowTimeRate = 0;

        while (animTime > 0)
        {
            yield return null;

            // ���Ԍv��
            animTime -= Time.deltaTime;
            nowTimeRate = animTime / DISSOLVE_TIME;

            // �f�B�]���u����
            SetDissolve(nowTimeRate);

            // �|�W�V��������
            SetPosition(randomPos,nowTimeRate);
        }
    }

    /// <summary>
    /// �ޏꉉ�o
    /// </summary>
    /// <returns>�R���[�`��</returns>
    private IEnumerator EndDissolve(float animTime)
    {
        // �����ʒu�̒���
        Vector3 randomPos = CreateRandomPos();
        //transform.position = randomPos;
        SetDissolve(1);

        float nowTimeRate = 0;

        while (animTime > 0)
        {
            yield return null;

            // ���Ԍv��
            animTime -= Time.deltaTime;
            nowTimeRate = LibMath.OneMinus(animTime / DISSOLVE_TIME);

            // �f�B�]���u����
            SetDissolve(nowTimeRate);

            // �|�W�V��������
            SetPosition(randomPos,nowTimeRate);
        }

        transform.position = START_POS;
        col.enabled = false;
    }

    /// <summary>
    /// �����_���Ȉړ��n�_���쐬
    /// </summary>
    /// <returns>�쐬���ꂽ�ړ��n�_</returns>
    private Vector3 CreateRandomPos()
    {
        float theta = Random.Range(-90f, 90f);
        Vector3 rondomPos = START_POS + Quaternion.AngleAxis(theta, transform.forward) * (Vector3.up * ANIM_MOVE_LENGHT);
        return rondomPos;
    }

    /// <summary>
    /// �V�F�[�_�[�Ƀf�B�]���u�̒l��n������
    /// </summary>
    /// <param name="value">�f�B�]���u�̊����@0�F�����@1�F��������Ȃ�</param>
    private void SetDissolve(float value)
    {
        mat.SetFloat(MATERIAL_PASS, value);
    }

    /// <summary>
    /// �|�W�V������ݒ肷�鏈��
    /// </summary>
    /// <param name="goalPos">�����_���n�_</param>
    /// <param name="value">�n�_�̊����@0�F�����ʒu�@1�F�����_���ʒu</param>
    private void SetPosition(Vector3 randomPos,float value)
    {
        // TODO:���o�Ɉړ��A�j���[�V�������쐬����ꍇ�����𕜊�������B
        // �������A�ړ��A�j���[�V�����ɍۂɃL�����N�^�[���͈͂���O���ƃo�O�̌����ɂȂ邽�߂�������������
        // �쐬�����͉^�p�����m�ł͂Ȃ����ߓo�ꎞ�Ɉړ������Ȃ������őΉ�

        //Vector3 targetPos = Vector3.Lerp(START_POS, randomPos, value);
        //transform.position = targetPos;
    }
}
