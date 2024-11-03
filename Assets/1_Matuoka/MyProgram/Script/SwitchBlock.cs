using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBlock : MonoBehaviour
{
    #region Fields

    private MeshRenderer switchMeshRenderer;
    private GeneralCollider3D switchCollider;
    private GameObject blocks;
    enum ColorMaterial
    {
        Green = 0,
        Yellow,
        Red,
    }

    [SerializeField] private float LIMIT_TIME = 3f;

    [SerializeField, ReadOnly] private bool isCountDown = false;
    [SerializeField, ReadOnly] private float countTime = 0f;

    #endregion


    #region MonoBehaviourMethod

    private void Awake()
    {
        GameObject switchObj = transform.GetChild(0).gameObject;

        switchMeshRenderer = switchObj.GetComponent<MeshRenderer>();
        ChangeColor(Color.green);

        switchCollider = switchObj.GetComponent<GeneralCollider3D>();
        switchCollider.onEnter += OnSwitchTriggerEnter;
        switchCollider.onStay  += OnSwitchTriggerStay;
        switchCollider.onExit  += OnSwitchTriggerExit;

        blocks = transform.GetChild(1).gameObject;
        blocks.SetActive(false);
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (isCountDown == true)
        {
            countTime -= Time.deltaTime;

            if (countTime <= 0)
            {
                isCountDown = false;
                ChangeColor(Color.green);
                blocks.SetActive(false);
            }
        }
    }

    private void FixedUpdate()
    {

    }

    #endregion


    #region CustomMethod

    /// <summary>
    /// êFÇÃïœçX
    /// </summary>
    /// <param name="color"></param>
    private void ChangeColor(Color color)
    {
        switchMeshRenderer.material.color = color;
    }

    private void OnSwitchTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            ChangeColor(Color.yellow);
            blocks.SetActive(true);

            isCountDown = false;
            countTime = LIMIT_TIME;
        }
    }

    private void OnSwitchTriggerStay(Collider collision)
    {
        if (collision.TryGetComponent(out Player player))
        {

        }
    }

    private void OnSwitchTriggerExit(Collider collision)
    {
        if (collision.TryGetComponent<Player>(out _))
        {
            ChangeColor(Color.red);

            isCountDown = true;
        }
    }

    #endregion
}
