using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindArea : MonoBehaviour
{
    #region Fields

    [SerializeField] Player.Wind_Add wind_Add;

    #endregion


    #region MonoBehaviourMethod

    private void Awake()
    {
        wind_Add.Init(this.transform);
    }

    #endregion


    #region CustomMethod

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<I_WindMover>(out I_WindMover i_WindMover))
        {
            i_WindMover.WindEnter(wind_Add);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out I_WindMover i_WindMover))
        {
            i_WindMover.WindExit(wind_Add);
        }
    }

    #endregion
}


/// <summary>
/// ���̉e�����󂯂�I�u�W�F�N�g�p�̃C���^�[�t�F�[�X
/// </summary>
public interface I_WindMover
{
    /// <summary>
    /// ���̃G���A�ɓ���������
    /// </summary>
    public void WindEnter(Player.Wind_Add wind_Add);

    /// <summary>
    /// ���̃G���A����o������
    /// </summary>
    public void WindExit(Player.Wind_Add wind_Add);
}
