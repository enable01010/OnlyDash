using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using static Player;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]

public partial class Player : SingletonActionListener<Player>, I_Trampolined
{
    #region �ʃR���|�[�l���g

    private Animator _animator;
    private CharacterController _controller;
    private GameObject _mainCamera;

    #endregion

    #region �X�e�[�^�X

    [Header("�X�e�[�^�X")]
    [Tooltip("����̈ړ����x"),SerializeField] private float SPRINT_SPEED = 5.335f;
    [Tooltip("�����̈ړ����x"),SerializeField] private float MOVE_SPEED = 5.335f;

    [Tooltip("�ړ����x�̕ω���")]private float SPEED_CHANGE_RATE = 10.0f;
    [Tooltip("�ő��]���x"),Range(0.0f, 0.3f)] private float ROTATION_SMOOTH_TIME = 0.12f;

    [Space(10)]
    [Tooltip("�W�����v�N�[���^�C��"),SerializeField] private float JUMP_TIMEOUT = 0.50f;
    [Tooltip("�����܂ł̎���"),SerializeField] private float FALL_TIMEOUT = 0.15f;

    [Space(10)]
    [Header("�X���C�f�B���O�֌W")]
    [Tooltip("�X���C�f�B���O�̎���"),SerializeField] private float SLIDING_TIMEOUT = 1.0f;
    [Tooltip("�X���C�f�B���O�̑��x"),SerializeField] private float SLIDING_SPEED = 6.0f;

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
    private float _slidingTimeoutDelta;

    #endregion

    #region �C���^�[�t�F�[�X

    [Space(10)]
    [Header("�C���^�[�t�F�[�X")]
    [Tooltip("�ړ�"), SerializeReference, SubclassSelector] I_Move move = new ControlledMove();
    [Tooltip("�X���C�f�B���O"), SerializeReference, SubclassSelector] I_Sliding sliding = new DefaultSliding();
    [Tooltip("�W�b�v���C��"), SerializeReference, SubclassSelector] I_ZipLine zipLine = new DefaultZipLine();
    [Tooltip("�N���C�~���O"), SerializeReference, SubclassSelector] I_Climbing climbing = new DefaultClimbing();

    #endregion

    #region ���n����p�̕ϐ�

    [Space(10)]
    [Header("���n����")]
    [Tooltip("�n�ʂɐڂ��Ă��邪"),SerializeField,ReadOnly] private bool isGrounded = true;
    [Tooltip("Player��Transform����v�����钸�_�̋���"),SerializeField] private float GROUNDED_OFFSET = -0.14f;
    [Tooltip("�n�ʂƗ��ꂽ��������鋅�̔��a"),SerializeField] private float GROUNDED_RADIUS = 0.28f;
    [Tooltip("�n�ʂ̃��C���["),SerializeField] private LayerMask GroundLayers;

    #endregion

    #region ActionListener�̗p�ϐ�

    private Vector2 playerMove = Vector2.zero;
    private Vector2 camMove = Vector2.zero;
    private bool isSlide = false;
    private bool isSlow = false;

    #endregion

    #region �J�����p�̕ϐ�
    [Header("�J����")]
    public GameObject CinemachineCameraTarget;
    [Tooltip("���"),SerializeField] private float TopClamp = 70.0f;
    [Tooltip("����"), SerializeField] private float BottomClamp = -30.0f;
    private bool lockCameraPosition = false;
    private float CameraAngleOverride = 0.0f;
    private const float THRESHOLD = 0.01f;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private bool IsCurrentDeviceMouse
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

    [SerializeField] private AudioClip LandingAudioClip;
    [SerializeField] private AudioClip[] FootstepAudioClips;
    [Range(0, 1),SerializeField] private float FOOTSTEP_AUDIO_VOLUME = 0.5f;

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
    private int _animIDSliding;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;
    private int _animIDClimbing_x;
    private int _animIDClimbing_y;
    private int _animIDClimbingStart;
    private int _animIDClimbingUp;
    private int _animIDClimbingDown;
    private int _animIDZipLine;

    //IK�A�j���[�V�����̐ݒ�
    private Vector3 rightHandIKPosition;
    private Vector3 leftHandIKPosition;
    private Vector3 rightLegIKPosition;
    private Vector3 leftLegIKPosition;

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


        zipLine.PlayerStart();
    }

    private void Update()
    {
        GroundCheck();
        CameraRotation();

        zipLine.PlayerUpdate();
        climbing.CanUseCheck();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(move.GetType() == typeof(ControlledMove))
            {
                move = new AutoMove();
            }
            else if(move.GetType() == typeof(AutoMove))
            {
                move = new ControlledMove();
            }

        }
#endif
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
        _animIDSliding = Animator.StringToHash("Sliding");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        _animIDClimbing_x = Animator.StringToHash("Climbing_X");
        _animIDClimbing_y = Animator.StringToHash("Climbing_Y");
        _animIDClimbingStart = Animator.StringToHash("ClimbingStart");
        _animIDClimbingUp = Animator.StringToHash("ClimbingEndUp");
        _animIDClimbingDown = Animator.StringToHash("ClimbingEndDown");
        _animIDZipLine = Animator.StringToHash("ZipLine");
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

    #region ZipLine�ݒ�p�֐�



    #endregion

    #region ActionListener�̃R�[���o�b�N�֐�

    public override void OnPlayerMove(InputAction.CallbackContext context)
    {
        if (GameData.G_AllCheck() == true) return;

        base.OnPlayerMove(context);

        playerMove = context.ReadValue<Vector2>();
    }

    public override void OnJump(InputAction.CallbackContext context)
    {
        if (GameData.G_AllCheck() == true) return;
        if (isGrounded == false) return;

        base.OnJump(context);

        if (context.phase == InputActionPhase.Started && _jumpTimeoutDelta <= 0.0f)
        {
            _verticalVelocity = Mathf.Sqrt(JUMP_HEIGHT * -2f * GRAVITY);
            _animator.SetBool(_animIDJump, true);
            _jumpTimeoutDelta = JUMP_TIMEOUT;
        }
        
    }

    public override void OnSlow(InputAction.CallbackContext context)
    {
        if (GameData.G_AllCheck() == true) return;

        base.OnSlow(context);

        isSlow = context.ReadValue<bool>();
    }

    public override void OnSlide(InputAction.CallbackContext context)
    {
        if (GameData.G_AllCheck() == true) return;
        if (isGrounded == false) return; //���n���ĂȂ��ꍇ
        if (sliding.IsGuard() == true) return;//�X�e�[�g���L�̃K�[�h��

        base.OnSlide(context);

        if (context.phase == InputActionPhase.Started)
        {
            CustomEvent.Trigger(gameObject, "useSliding");
        }
    }

    public override void OnZipLine(InputAction.CallbackContext context)
    {
        if (GameData.G_AllCheck() == true) return;
        if (zipLine.IsGuardOnTrigger() == true) return;//�X�e�[�g���L�̃K�[�h��

        base.OnZipLine(context);

        if (context.phase == InputActionPhase.Started)
        {
            zipLine.OnTrigger();
        }
    }

    public override void OnClimbing(InputAction.CallbackContext context)
    {
        if (GameData.G_AllCheck()) return;
        if (climbing.IsGuard() == true) return;
        base.OnClimbing(context);

        if (context.phase == InputActionPhase.Started)
        {
            climbing.OnTrigger();
        }

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
        move.Move();
        CaluculateJumpSettingsOnGround();
    }

    public void InAirState()
    {
        GameData.G_AllCheck();

        Rot();
        move.Move();//�v���؁A�󒆓��͂��󂯕t����ׂ����ǂ���
        CaluculateJumpSettingsInAir();
        CaluculateGravitySettings();
    }

    public void SlidingEnter()
    {
        sliding.OnEnter();
    }

    public void SlidingState()
    {
        GameData.G_AllCheck();

        sliding.Sliding();
        CaluculateJumpSettingsOnGround();
    }

    public void SlidingExit()
    {
        sliding.OnExit();
    }

    public void ZipLineEnter()
    {
        zipLine.OnEnter();
    }

    public void ZipLineState()
    {
        zipLine.OnUpdate();
    }

    public void ZipLineExit()
    {
        zipLine.OnExit();
    }

    public void ClimbingEnter()
    {
        climbing.OnEnter();
    }

    public void ClimbingState()
    {
        climbing.Climbing();
    }

    public void ClimbingExit()
    {
        climbing.OnExit();
    }

    #endregion

    #region �C���^�[�t�F�[�X�Ăяo���p�֐�

    public void SetZipLineArea(ZipLineArea zipLineArea)
    {
        zipLine.AddArea(zipLineArea);
    }

    public void DeleteZipLineArea(ZipLineArea zipLineArea)
    {
        zipLine.DeleteArea(zipLineArea);
    }

    public void SetWallArea(WallArea wallArea)
    {
        climbing.AddArea(wallArea);
    }

    public void DeleteWallArea(WallArea wallArea)
    {
        climbing.DeleteArea(wallArea);
    }

    #endregion

    #region �A�j���[�^�[IK�p
    
    private void OnAnimatorIK()
    {
        RightHandIK();
        LeftHandIK();
        RightLegIK();
        LeftLefIK();
    }

    private void RightHandIK()
    {

        float rightHandWeight = (rightHandIKPosition == Vector3.zero)?0:_animator.GetFloat("RightHandWeight");
        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
        _animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIKPosition);
    }

    private void LeftHandIK()
    {
        float leftHandWeight = (leftHandIKPosition == Vector3.zero) ? 0 : _animator.GetFloat("LeftHandWeight");
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
        _animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIKPosition);
    }

    private void RightLegIK()
    {
        float leftLegWeight = (rightLegIKPosition == Vector3.zero) ? 0 : _animator.GetFloat("RightLegWeight");
        _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, leftLegWeight);
        _animator.SetIKPosition(AvatarIKGoal.RightFoot, rightLegIKPosition);
    }

    private void LeftLefIK()
    {
        float leftLegWeight = (leftLegIKPosition == Vector3.zero) ? 0 : _animator.GetFloat("LeftLegWeight");
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftLegWeight);
        _animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftLegIKPosition);
    }

    #endregion

    #region �O���C���^�[�t�F�[�X

    public void TrampolineJump(float trampolineJumpPower)
    {
        _verticalVelocity = trampolineJumpPower;
        _animator.SetBool(_animIDJump, true);
    }

    #endregion
}

