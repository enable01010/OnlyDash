using UnityEngine;
using System.Collections;

/// <summary>
/// ���e�I�u�W�F�N�g
/// </summary>
public class Bomb : MonoBehaviour
{
    const float PREFAB_DESTROY_TIME = 5.0f;
    [SerializeField] float POWER = 10;
    [SerializeField] float REPOP_TIME = 5.0f;
    public const float BOMB_SPEED_SLOW = 0.99f;

    private void OnTriggerEnter(Collider other)
    {
        // �{����������Ώۂ̏ꍇ�̂ݏ���
        if (!other.TryGetComponent<I_BombHit>(out var hit)) return;

        // �����̈ړ������Ƌ������Z�o
        Vector3 dir = other.transform.position - transform.position;
        dir.y += ConstData.CHARACTER_HIGHT / 2; //�L�����N�^�[�̌��_�������̂��ߍ����C��
        Vector3 impact = dir.normalized * POWER;
        hit.BombHit(impact);

        // ���������ۂ̃G�t�F�N�g�쐬
        GameObject instance = Instantiate(LibResourceLoader._bombFxPref);
        instance.transform.position = transform.position;

        //�p������
        Destroy(instance, PREFAB_DESTROY_TIME);
        LibCoroutineRunner.StartCoroutine(Repop(REPOP_TIME));
    }

    /// <summary>
    /// ���|�b�v����
    /// </summary>
    /// <param name="repopTime"></param>
    /// <returns></returns>
    private IEnumerator Repop(float repopTime)
    {
        gameObject.SetActive(false);

        while(repopTime > 0)
        {
            repopTime -= Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(true);
    }
}


/// <summary>
/// ���e�I�u�W�F�N�g�̉e�����󂯂�
/// </summary>
public interface I_BombHit
{

    /// <summary>
    /// �����ɓ����������̏���
    /// </summary>
    public void BombHit(Vector3 power);
}
