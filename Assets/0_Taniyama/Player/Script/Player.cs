using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class Player : SingletonActionListener<Player>
{
    private Animator _animator;
    private PlayerInput _playerInput;
    private StarterAssets.StarterAssetsInputs _input;
    private GameObject _mainCamera;

    #region ���n����p�̕ϐ�

    [Header("���n����")]
    [Tooltip("�n�ʂɐڂ��Ă��邪")]
    [SerializeField] private bool isGrounded = true;

    [Tooltip("Player��Transform����v�����钸�_�̋���")]
    [SerializeField] private float groundedOffset = -0.14f;

    [Tooltip("�n�ʂƗ��ꂽ��������鋅�̔��a")]
    [SerializeField] private float groundedRadius = 0.28f;

    [Tooltip("�n�ʂ̃��C���[")]
    [SerializeField] private LayerMask GroundLayers;

    #endregion

    #region ActionListener�̗p�ϐ�

    private Vector2 move = Vector2.zero;
    private bool isJump = false;
    private bool isSlide = false;
    private bool isSlow = false;

    #endregion

    #region �W�����v�p�̕ϐ�

    [SerializeField] private float JumpHeight = 1.2f;
    [SerializeField] private float Gravity = -15.0f;
    private float _jumpTimeoutDelta;
    private float _verticalVelocity;

    #endregion
    //�A�j���[�^�[�̐ݒ�p
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    public override void OnInitialize()
    {
        base.OnInitialize();

        SettngAnimator();
        SettingCamera();
    }

    public void Move()
    {
        
    }

    //public void Jump()
    //{

    //}

    public void Fall()
    {

    }

    /// <summary>
    /// �A�j���[�V�����̐ݒ�
    /// </summary>
    private void SettngAnimator()
    {
        //�A�j���[�V�����ݒ�
        _animator = GetComponent<Animator>();
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    /// <summary>
    /// �J�����̐ݒ�
    /// </summary>
    private void SettingCamera()
    {
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    /// <summary>
    /// �L�����N�^�[�����n���Ă��邩�m�F���鏈��
    /// </summary>
    private void GroundCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = LibVector.Add_Y(transform.position, -groundedOffset);
        isGrounded = Physics.CheckSphere(spherePosition, groundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        _animator.SetBool(_animIDGrounded, isGrounded); 
    }

    #region ActionListener�̃R�[���o�b�N�֐�

    public override void OnPlayerMove(InputAction.CallbackContext context)
    {
        base.OnPlayerMove(context);

        move = context.ReadValue<Vector2>();
    }

    public override void OnJump(InputAction.CallbackContext context)
    {
        base.OnJump(context);

        isJump = context.ReadValue<bool>();

        if(isGrounded == true)
        {
            // Jump
            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                // update animator if using character
                
                _animator.SetBool(_animIDJump, true);
                
            }
        }
    }

    public override void OnSlow(InputAction.CallbackContext context)
    {
        base.OnSlow(context);

        isSlow = context.ReadValue<bool>();
    }

    public override void OnSlide(InputAction.CallbackContext context)
    {
        base.OnSlide(context);

        isSlide = context.ReadValue<bool>();
    }

    #endregion

}
