using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]

public class Player : SingletonActionListener<Player>, I_Move
{
    #region �ʃR���|�[�l���g

    public Animator _animator { private set; get; }
    public CharacterController _controller { private set; get; }
    public GameObject _mainCamera { private set; get; }

    #endregion

    #region �X�e�[�^�X

    [field: Header("�X�e�[�^�X")]
    [field: Tooltip("����̈ړ����x")]
    [field: SerializeField] public float SPRINT_SPEED { private set; get; } = 5.335f;

    [field: Tooltip("�����̈ړ����x")]
    [field: SerializeField] public float MOVE_SPEED { private set; get; } = 5.335f;

    [field: Tooltip("�ړ����x�̕ω���")]
    public float SPEED_CHANGE_RATE { private set; get; } = 10.0f;

    [field: Tooltip("�ő��]���x")]
    [field: Range(0.0f, 0.3f)]
    public float ROTATION_SMOOTH_TIME { private set; get; } = 0.12f;

    [field: Space(10)]
    [field: Tooltip("�W�����v�N�[���^�C��")]
    [field: SerializeField] public float JUMP_TIMEOUT { private set; get; } = 0.50f;

    [field: Tooltip("�����܂ł̎���")]
    [field: SerializeField] public float FALL_TIMEOUT { private set; get; } = 0.15f;

    //�ړ�
    [HideInInspector] public float _speed;
    [HideInInspector] public float _animationBlend;
    //��]
    public float _targetRotation { private set; get; } = 0.0f;
    [HideInInspector] public float _rotationVelocity;
    //����
    public float _verticalVelocity{private set; get;}
    public float TERMINAL_VELOCITY { private set; get; } = 53.0f;
    //����
    public float _jumpTimeoutDelta{private set; get;}
    public float _fallTimeoutDelta{private set; get;}
    #endregion

    #region ���n����p�̕ϐ�

    [field: Header("���n����")]
    [field: Tooltip("�n�ʂɐڂ��Ă��邪")]
    [field: SerializeField, ReadOnly] public bool isGrounded { private set; get; } = true;

    [field: Tooltip("Player��Transform����v�����钸�_�̋���")]
    [field: SerializeField] public float GROUNDED_OFFSET { private set; get; } = -0.14f;

    [field: Tooltip("�n�ʂƗ��ꂽ��������鋅�̔��a")]
    [field: SerializeField] public float GROUNDED_RADIUS { private set; get; } = 0.28f;

    [field: Tooltip("�n�ʂ̃��C���[")]
    [field: SerializeField] public LayerMask GroundLayers{private set; get;}

    #endregion

    #region ActionListener�̗p�ϐ�

    public Vector2 playerMove { private set; get; } = Vector2.zero;
    public Vector2 camMove { private set; get; } = Vector2.zero;
    public bool isSlide { private set; get; } = false;
    public bool isSlow { private set; get; } = false;

    #endregion

    #region �J�����p�̕ϐ�
    [field: Header("�J����")]
    [field: SerializeField] public GameObject CinemachineCameraTarget{private set; get;}
    [field: Tooltip("���")]
    [field: SerializeField] public float TopClamp { private set; get; } = 70.0f;
    [field: Tooltip("����")]
    [field: SerializeField] public float BottomClamp { private set; get; } = -30.0f;
    [field: SerializeField] public bool lockCameraPosition { private set; get; } = false;
    [field: SerializeField] public float CameraAngleOverride { private set; get; } = 0.0f;
    public const float THRESHOLD = 0.01f;
    public float _cinemachineTargetYaw{private set; get;}
    public float _cinemachineTargetPitch{private set; get;}

    public bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM

            string a = MainSceneManager.GetCurrentControlScheme();
            bool answer = MainSceneManager.GetCurrentControlScheme() == "KeyboardMouse";
            return MainSceneManager.GetCurrentControlScheme() == "KeyboardMouse";
#else
			return false;
#endif
        }
    }

    #endregion

    #region �T�E���h�֌W

    [field: SerializeField] public AudioClip LandingAudioClip{private set; get;}
    [field: SerializeField] public AudioClip[] FootstepAudioClips{private set; get;}
    [field: Range(0, 1), SerializeField] public float FOOTSTEP_AUDIO_VOLUME { private set; get; } = 0.5f;

    #endregion

    #region �W�����v�p�̕ϐ�

    [field: SerializeField] public float JUMP_HEIGHT { private set; get; } = 1.2f;
    [field: SerializeField] public float GRAVITY { private set; get; } = -15.0f;

    #endregion

    #region �A�j���[�^�[�̐ݒ�

    //�A�j���[�^�[�̐ݒ�p
    public int _animIDSpeed{private set; get;}
    public int _animIDGrounded{private set; get;}
    public int _animIDJump{private set; get;}
    public int _animIDFreeFall{private set; get;}
    public int _animIDMotionSpeed{private set; get;}

    #endregion

    #region Monobehaviour�̃R�[���o�b�N�֐�

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
        CameraRotation();
    }

    #endregion

    #region �����ݒ�p�֐�

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

        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
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

#endregion

    #region �L�����N�^�[�̋�������p�֐�

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
                _animator.SetBool(_animIDJump, false);
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


    /// <summary>
    /// �L�����N�^�[�̉�]�Ɋւ��鏈��
    /// </summary>
    private void Rot()
    {
        Vector3 inputDirection = new Vector3(playerMove.x, 0.0f, playerMove.y).normalized;
        if (playerMove != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, ROTATION_SMOOTH_TIME);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
    }

    #endregion

    #region ActionListener�̃R�[���o�b�N�֐�

    public override void OnPlayerMove(InputAction.CallbackContext context)
    {
        GameData.G_AllCheck();

        base.OnPlayerMove(context);

        playerMove = context.ReadValue<Vector2>();
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

    public override void OnCamMove(InputAction.CallbackContext context)
    {
        base.OnCamMove(context);

        camMove = context.ReadValue<Vector2>();
    }

    #endregion

    #region �J�����̋�������p�֐�

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (camMove.sqrMagnitude >= THRESHOLD && !lockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += camMove.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += camMove.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = LibMath.ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = LibMath.ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
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
        ((I_Move)this).Move();
        CaluculateJumpSettingsOnGround();
    }

    public void InAirState()
    {
        GameData.G_AllCheck();

        Rot();
        ((I_Move)this).Move();//�v���؁A�󒆓��͂��󂯕t����ׂ����ǂ���
        CaluculateJumpSettingsInAir();
        CaluculateGravitySettings();
    }
    #endregion

    public virtual Player GetThis()
    {
        return this;
    }
}

public interface I_Move
{
    public Player GetThis();

    /// <summary>
    /// �L�����N�^�[�̈ړ��Ɋւ��鏈��
    /// ���Rot()
    /// </summary>
    public void Move()
    {
        float targetSpeed = GetThis().SPRINT_SPEED;
        if (GetThis().playerMove == Vector2.zero) targetSpeed = 0.0f;
        float currentHorizontalSpeed = new Vector3(GetThis()._controller.velocity.x, 0.0f, GetThis()._controller.velocity.z).magnitude;
        const float SPEED_OFFSET = 0.1f;
        float inputMagnitude = GetThis().playerMove.magnitude;

        //�ς��ƌ��ړ��n�v�Z
        if (currentHorizontalSpeed < targetSpeed - SPEED_OFFSET || currentHorizontalSpeed > targetSpeed + SPEED_OFFSET)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            GetThis()._speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * GetThis().SPEED_CHANGE_RATE);

            // round speed to 3 decimal places
            GetThis()._speed = Mathf.Round(GetThis()._speed * 1000f) / 1000f;
        }
        else
        {
            GetThis()._speed = targetSpeed;
        }

        GetThis()._animationBlend = Mathf.Lerp(GetThis()._animationBlend, targetSpeed, Time.deltaTime * GetThis().SPEED_CHANGE_RATE);
        if (GetThis()._animationBlend < 0.01f) GetThis()._animationBlend = 0f;

        Vector3 targetDirection = Quaternion.Euler(0.0f, GetThis()._targetRotation, 0.0f) * Vector3.forward;

        GetThis()._controller.Move(targetDirection.normalized * (GetThis()._speed * Time.deltaTime) + new Vector3(0.0f, GetThis()._verticalVelocity, 0.0f) * Time.deltaTime);

        GetThis()._animator.SetFloat(GetThis()._animIDSpeed, GetThis()._animationBlend);
        GetThis()._animator.SetFloat(GetThis()._animIDMotionSpeed, inputMagnitude);
    }
}