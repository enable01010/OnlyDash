using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBlock : MonoBehaviour ,I_GeneralColliderUser
{
    #region Fields

    private MeshRenderer switchMeshRenderer;
    private GameObject blocks;

    [SerializeField] private float LIMIT_TIME = 3f;

    [SerializeField, ReadOnly] private bool isNotPlayerOn = false;
    [SerializeField, ReadOnly] private float countTime = 0f;
    private bool isUse { get { return (isNotPlayerOn && countTime > 0); } }

    #endregion


    #region MonoBehaviourMethod

    private void Awake()
    {
        GameObject switchObj = transform.GetChild(0).gameObject;

        switchMeshRenderer = switchObj.GetComponent<MeshRenderer>();
        ChangeColor(Color.green);

        blocks = transform.GetChild(1).gameObject;
        blocks.SetActive(false);
    }

    private void Update()
    {
        if (isUse == false) return;

        countTime -= Time.deltaTime;

        if (countTime <= 0)
        {
            ChangeColor(Color.green);
            blocks.SetActive(false);
        }

    }

    #endregion


    #region CustomMethod

    /// <summary>
    /// 色の変更
    /// </summary>
    /// <param name="color"></param>
    private void ChangeColor(Color color)
    {
        switchMeshRenderer.material.color = color;
    }

    #endregion

    public virtual void OnEnter_GeneralCollider(Collider collision, Transform generalCollider)
    {
        if (collision.TryGetComponent(out I_SwitchHit i_SwitchHit))
        {
            ChangeColor(Color.yellow);
            blocks.SetActive(true);

            isNotPlayerOn = false;
            countTime = LIMIT_TIME;
        }
    }

    public virtual void OnExit_GeneralCollider(Collider collision, Transform generalCollider)
    {
        if (collision.TryGetComponent<I_SwitchHit>(out _))
        {
            ChangeColor(Color.red);

            isNotPlayerOn = true;
        }
    }
}


/// <summary>
/// スイッチを押せるオブジェクト用のインターフェース
/// </summary>
public interface I_SwitchHit
{

}
