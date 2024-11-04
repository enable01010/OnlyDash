using System.Collections;
using UnityEngine;

public class BreakGround_Stay : MonoBehaviour, I_GeneralColliderUser
{
    [SerializeField] float BREAK_TIME = 5.0f;
    [SerializeField, ReadOnly] float nowBreakTime = 0;
    [SerializeField] float REPOP_TIME = 5.0f;


    public virtual void OnEnter_GeneralCollider(Collider other)
    {
        if (!other.TryGetComponent<I_BreakGround>(out _)) return;

        nowBreakTime = 0;
    }

    public virtual void OnStay_GeneralCollider(Collider other)
    {
        nowBreakTime += Time.deltaTime;
        if (nowBreakTime > BREAK_TIME)
        {
            Break();
        }
    }

    /// <summary>
    /// ���鎞�̏���
    /// �ǉ����o�̂��߂ɒǋL����\���������̂Ŋ֐���
    /// </summary>
    private void Break()
    {
        LibCoroutineRunner.StartCoroutine(Repop(REPOP_TIME));
    }

    /// <summary>
    /// �ďo������
    /// </summary>
    /// <returns>�R���[�`��</returns>
    private IEnumerator Repop(float repopTime)
    {
        gameObject.SetActive(false);

        while (repopTime > 0)
        {
            yield return null;
            repopTime -= Time.deltaTime;
        }

        gameObject.SetActive(true);
    }
}

public interface I_BreakGround
{

}