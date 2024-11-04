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
/// 風の影響を受けるオブジェクト用のインターフェース
/// </summary>
public interface I_WindMover
{
    /// <summary>
    /// 風のエリアに入った処理
    /// </summary>
    public void WindEnter(Player.Wind_Add wind_Add);

    /// <summary>
    /// 風のエリアから出た処理
    /// </summary>
    public void WindExit(Player.Wind_Add wind_Add);
}
