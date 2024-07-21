using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Fields

    [Header("Player")]
    [SerializeField] private float moveSpeed;
    private Rigidbody rbody;
    private Vector3 moveDir;
    [SerializeField] private bool isGetAxisRaw = true;
    [SerializeField] private bool isConstSpeed = true;
    [SerializeField] private float inputX;
    [SerializeField] private float inputZ;
    [SerializeField] private float inputMagnitude = 0.1f;
    [SerializeField, Range(0f, 1f)] private float rotationSpeed = 1.0f;

    #endregion


    #region MonoBehaviourMethod

    private void Start()
    {
        rbody = this.gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        InputUpdate();

        MoveDir();
    }

    private void FixedUpdate()
    {
        Rotation();

        Velocity();
    }

    #endregion


    #region CustomMethod

    // 入力
    private void InputUpdate()
    {
        if (isGetAxisRaw == true)
        {
            // キーボード　   : -1 or 0 or 1 
            // コントローラー : -1 ～ 1
            inputX = Input.GetAxisRaw("Horizontal");
            inputZ = Input.GetAxisRaw("Vertical");
        }
        else
        {
            // キーボード　   : -1 ～ 1
            // コントローラー : -1 ～ 1
            inputX = Input.GetAxis("Horizontal");
            inputZ = Input.GetAxis("Vertical");
        }
    }

    // 移動量
    private void MoveDir()
    {
        moveDir = new Vector3(inputX, 0.0f, inputZ);

        if (Mathf.Abs(moveDir.x) < inputMagnitude) moveDir.x = 0.0f;
        if (Mathf.Abs(moveDir.z) < inputMagnitude) moveDir.z = 0.0f;
    }

    // 回転
    private void Rotation()
    {
        rbody.angularVelocity = Vector3.zero;

        // 動いていれば進行方向を向く
        if (moveDir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(moveDir);

            // 滑らかに回転
            rot = Quaternion.Slerp(this.transform.rotation, rot, rotationSpeed);

            this.transform.rotation = rot;
        }
    }

    // 速度
    private void Velocity()
    {
        Vector3 tempDir = moveDir.normalized;

        // 速度一定じゃない
        if (isConstSpeed == false)
        {
            tempDir *= Mathf.Max(Mathf.Abs(moveDir.x), Mathf.Abs(moveDir.z));
        }

        Vector3 tempVelocity = tempDir * moveSpeed;

        rbody.velocity = new Vector3(tempVelocity.x, rbody.velocity.y, tempVelocity.z);
    }

    #endregion
}
