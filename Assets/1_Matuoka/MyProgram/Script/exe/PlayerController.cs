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

    // ����
    private void InputUpdate()
    {
        if (isGetAxisRaw == true)
        {
            // �L�[�{�[�h�@   : -1 or 0 or 1 
            // �R���g���[���[ : -1 �` 1
            inputX = Input.GetAxisRaw("Horizontal");
            inputZ = Input.GetAxisRaw("Vertical");
        }
        else
        {
            // �L�[�{�[�h�@   : -1 �` 1
            // �R���g���[���[ : -1 �` 1
            inputX = Input.GetAxis("Horizontal");
            inputZ = Input.GetAxis("Vertical");
        }
    }

    // �ړ���
    private void MoveDir()
    {
        moveDir = new Vector3(inputX, 0.0f, inputZ);

        if (Mathf.Abs(moveDir.x) < inputMagnitude) moveDir.x = 0.0f;
        if (Mathf.Abs(moveDir.z) < inputMagnitude) moveDir.z = 0.0f;
    }

    // ��]
    private void Rotation()
    {
        rbody.angularVelocity = Vector3.zero;

        // �����Ă���ΐi�s����������
        if (moveDir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(moveDir);

            // ���炩�ɉ�]
            rot = Quaternion.Slerp(this.transform.rotation, rot, rotationSpeed);

            this.transform.rotation = rot;
        }
    }

    // ���x
    private void Velocity()
    {
        Vector3 tempDir = moveDir.normalized;

        // ���x��肶��Ȃ�
        if (isConstSpeed == false)
        {
            tempDir *= Mathf.Max(Mathf.Abs(moveDir.x), Mathf.Abs(moveDir.z));
        }

        Vector3 tempVelocity = tempDir * moveSpeed;

        rbody.velocity = new Vector3(tempVelocity.x, rbody.velocity.y, tempVelocity.z);
    }

    #endregion
}
