using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class Player : SingletonActionListener<Player>
{
    #region �ʃR���|�[�l���g

    private Animator _animator;
    private CharacterController _controller;
    private StarterAssets.StarterAssetsInputs _input;
    private GameObject _mainCamera;

    #endregion

    #region �X�e�[�^�X

    [Header("�X�e�[�^�X")]
    [Tooltip("����̈ړ����x")]
    [SerializeField] private float SPRINT_SPEED = 5.335f;

    [Tooltip("�����̈ړ����x")]
    [SerializeField] private float MOVE_SPEED = 5.335f;

    [Tooltip("�ړ����x�̕ω���")]
    private float SPEED_CHANGE_RATE = 10.0f;

    [Tooltip("�ő��]���x")]
    [Range(0.0f, 0.3f)]
    private float ROTATION_SMOOTH_TIME = 0.12f;

    [Space(10)]
    [Tooltip("�W�����v�N�[���^�C��")]
    public float JUMP_TIMEOUT = 0.50f;

    [Tooltip("�����܂ł̎���")]
    public float FALL_TIMEOUT = 0.15f;

    //�ړ�
    private float _speed;
    private float _animationBlend;
    //��]
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    //����
    private float _verticalVelocity;
    private float TERMINAL_VELOCITY = 53.0f;
    //����
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    #endregion

    #region ���n����p�̕ϐ�

    [Header("���n����")]
    [Tooltip("�n�ʂɐڂ��Ă��邪")]
    [SerializeField,ReadOnly] private bool isGrounded = true;

    [Tooltip("Player��Transform����v�����钸�_�̋���")]
    [SerializeField] private float GROUNDED_OFFSET = -0.14f;

    [Tooltip("�n�ʂƗ��ꂽ��������鋅�̔��a")]
    [SerializeField] private float GROUNDED_RADIUS = 0.28f;

    [Tooltip("�n�ʂ̃��C���[")]
    [SerializeField] private LayerMask GroundLayers;

    #endregion

    #region ActionListener�̗p�ϐ�

    private Vector2 move = Vector2.zero;
    private bool isSlide = false;
    private bool isSlow = false;

    #endregion

    #region �T�E���h�֌W

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FOOTSTEP_AUDIO_VOLUME = 0.5f;

    #endregion

    #region �W�����v�p�̕ϐ�

    [SerializeField] private float JUMP_HEIGHT = 1.2f;
    [SerializeField] private float GRAVITY = -15.0f;

    #endregion

    #region �A�j���[�^�[�̐ݒ�

    //�A�j���[�^�[�̐ݒ�p
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    #endregion

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (isGrounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GROUNDED_OFFSET, transform.position.z),GROUNDED_RADIUS);
    }

    public override void OnInitialize()
    {
        base.OnInitialize();

        SettngAnimator();
        SettingCamera();
        SettingCharacterController();
        SettingPlayerParameter();
    }

    private void Update()
    {
        GroundCheck();
    }

    /// <summary>
    /// �L�����N�^�[�̈ړ��Ɋւ��鏈��
    /// ���Rot()
    /// </summary>
    private void Move()
    {
        float targetSpeed = SPRINT_SPEED;
        if (move == Vector2.zero) targetSpeed = 0.0f;
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        const float SPEED_OFFSET = 0.1f;
        float inputMagnitude = move.magnitude;

        //�ς��ƌ��ړ��n�v�Z
        if (currentHorizontalSpeed < targetSpeed - SPEED_OFFSET || currentHorizontalSpeed > targetSpeed + SPEED_OFFSET)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SPEED_CHANGE_RATE);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SPEED_CHANGE_RATE);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        _animator.SetFloat(_animIDSpeed, _animationBlend);
        _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);

    }

    /// <summary>
    /// �L�����N�^�[�̉�]�Ɋւ��鏈��
    /// </summary>
    private void Rot()
    {
        Vector3 inputDirection = new Vector3(move.x, 0.0f, move.y).normalized;
        if (move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, ROTATION_SMOOTH_TIME);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
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
    /// �L�����N�^�[�R���g���[���[�̐ݒ�
    /// </summary>
    private void SettingCharacterController()
    {
        _controller = GetComponent<CharacterController>();
    }

    /// <summary>
    /// �v���C���[�̃p�����[�^�̐ݒ�
    /// </summary>
    private void SettingPlayerParameter()
    {
        // reset our timeouts on start
        _jumpTimeoutDelta = JUMP_TIMEOUT;
        _fallTimeoutDelta = FALL_TIMEOUT;
    }

    /// <summary>
    /// �L�����N�^�[�����n���Ă��邩�m�F���鏈��
    /// </summary>
    private void GroundCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = LibVector.Add_Y(transform.position, -GROUNDED_OFFSET);
        isGrounded = Physics.CheckSphere(spherePosition, GROUNDED_RADIUS, GroundLayers,
            QueryTriggerInteraction.Ignore);

        _animator.SetBool(_animIDGrounded, isGrounded);

        if(isGrounded == false)
        {
            CustomEvent.Trigger(gameObject, "inAir");
        }
        else
        {
            CustomEvent.Trigger(gameObject, "onGrounded");
        }
    }

    /// <summary>
    /// �d�͐ݒ�𔽉f���鏈��
    /// </summary>
    private void CaluculateGravitySettings()
    {
        if (_verticalVelocity < TERMINAL_VELOCITY)
        {
            _verticalVelocity += GRAVITY * Time.deltaTime;
        }
    }

    /// <summary>
    /// �󒆂ɂ���ۂ̃W�����v�Ɋւ���v�Z�����鏈��
    /// </summary>
    private void CaluculateJumpSettingsInAir()
    {

        if (isGrounded == false)
        {
            _jumpTimeoutDelta = JUMP_TIMEOUT;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _animator.SetBool(_animIDFreeFall, true);
            }
        }
    }

    /// <summary>
    /// ����ɂ���ۂ̃W�����v�Ɋւ���v�Z�����鏈��
    /// </summary>
    private void CaluculateJumpSettingsOnGround()
    {
        if (isGrounded == true)
        {
            _fallTimeoutDelta = FALL_TIMEOUT;

            _animator.SetBool(_animIDJump, false);
            _animator.SetBool(_animIDFreeFall, false);

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }


            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
    }

    #region ActionListener�̃R�[���o�b�N�֐�

    public override void OnPlayerMove(InputAction.CallbackContext context)
    {
        GameData.G_AllCheck();

        base.OnPlayerMove(context);

        move = context.ReadValue<Vector2>();
    }

    public override void OnJump(InputAction.CallbackContext context)
    {
        GameData.G_AllCheck();

        base.OnJump(context);

        if (isGrounded == true)
        {
            if (context.phase == InputActionPhase.Started && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(JUMP_HEIGHT * -2f * GRAVITY);
                _animator.SetBool(_animIDJump, true);
                _jumpTimeoutDelta = JUMP_TIMEOUT;
            }
        }
    }

    public override void OnSlow(InputAction.CallbackContext context)
    {
        GameData.G_AllCheck();

        base.OnSlow(context);

        isSlow = context.ReadValue<bool>();
    }

    public override void OnSlide(InputAction.CallbackContext context)
    {
        GameData.G_AllCheck();

        base.OnSlide(context);

        isSlide = context.ReadValue<bool>();
    }

    #endregion

    #region �T�E���h�֌W

    /// <summary>
    /// �����̃T�E���h�Đ�����
    /// </summary>
    private void OnFootstep(AnimationEvent animationEvent)
    {
        //if (animationEvent.animatorClipInfo.weight > 0.5f)
        //{
        //    if (FootstepAudioClips.Length > 0)
        //    {
        //        var index = Random.Range(0, FootstepAudioClips.Length);
        //        AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FOOTSTEP_AUDIO_VOLUME);
        //    }
        //}
    }

    /// <summary>
    /// ���n�����ۂ̃T�E���h�Đ�����
    /// </summary>
    private void OnLand(AnimationEvent animationEvent)
    {
        //if (animationEvent.animatorClipInfo.weight > 0.5f)
        //{
        //    AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FOOTSTEP_AUDIO_VOLUME);
        //}
    }

    #endregion


    #region �X�e�[�g�Ăяo���p�֐�

    public void LoadState()
    {
        if (LibSceneManager.GetIsLoad() == false)
        {
            CustomEvent.Trigger(gameObject, "OnGrounded");
        }
    }

    public void DashState()
    {
        GameData.G_AllCheck();

        Rot();
        Move();
        CaluculateJumpSettingsOnGround();
    }

    public void InAirState()
    {
        GameData.G_AllCheck();

        Rot();
        Move();//�v���؁A�󒆓��͂��󂯕t����ׂ����ǂ���
        CaluculateJumpSettingsInAir();
        CaluculateGravitySettings();
    }
    #endregion
}

