using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]

public class Player : SingletonActionListener<Player>
{
    #region 別コンポーネント

    private Animator _animator;
    private CharacterController _controller;
    private GameObject _mainCamera;

    #endregion

    #region ステータス

    [Header("ステータス")]
    [Tooltip("走りの移動速度")]
    [SerializeField] private float SPRINT_SPEED = 5.335f;

    [Tooltip("歩きの移動速度")]
    [SerializeField] private float MOVE_SPEED = 5.335f;

    [Tooltip("移動速度の変化率")]
    private float SPEED_CHANGE_RATE = 10.0f;

    [Tooltip("最大回転速度")]
    [Range(0.0f, 0.3f)]
    private float ROTATION_SMOOTH_TIME = 0.12f;

    [Space(10)]
    [Tooltip("ジャンプクールタイム")]
    public float JUMP_TIMEOUT = 0.50f;

    [Tooltip("落下までの時間")]
    public float FALL_TIMEOUT = 0.15f;

    //移動
    private float _speed;
    private float _animationBlend;
    //回転
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    //落下
    private float _verticalVelocity;
    private float TERMINAL_VELOCITY = 53.0f;
    //時間
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    #endregion

    #region 着地判定用の変数

    [Header("着地判定")]
    [Tooltip("地面に接しているが")]
    [SerializeField,ReadOnly] private bool isGrounded = true;

    [Tooltip("PlayerのTransformから計測する頂点の距離")]
    [SerializeField] private float GROUNDED_OFFSET = -0.14f;

    [Tooltip("地面と離れた判定をする球の半径")]
    [SerializeField] private float GROUNDED_RADIUS = 0.28f;

    [Tooltip("地面のレイヤー")]
    [SerializeField] private LayerMask GroundLayers;

    #endregion

    #region ActionListenerの用変数

    private Vector2 playerMove = Vector2.zero;
    private Vector2 camMove = Vector2.zero;
    private bool isSlide = false;
    private bool isSlow = false;

    #endregion

    #region カメラ用の変数
    [Header("カメラ")]
    public GameObject CinemachineCameraTarget;
    [Tooltip("上限")]
    public float TopClamp = 70.0f;
    [Tooltip("下限")]
    public float BottomClamp = -30.0f;
    public bool lockCameraPosition = false;
    public float CameraAngleOverride = 0.0f;
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

    #region サウンド関係

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FOOTSTEP_AUDIO_VOLUME = 0.5f;

    #endregion

    #region ジャンプ用の変数

    [SerializeField] private float JUMP_HEIGHT = 1.2f;
    [SerializeField] private float GRAVITY = -15.0f;

    #endregion

    #region アニメーターの設定

    //アニメーターの設定用
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    #endregion

    #region Monobehaviourのコールバック関数

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

    #region 初期設定用関数

    /// <summary>
    /// アニメーションの設定
    /// </summary>
    private void SettngAnimator()
    {
        //アニメーション設定
        _animator = GetComponent<Animator>();
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    /// <summary>
    /// カメラの設定
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
    /// キャラクターコントローラーの設定
    /// </summary>
    private void SettingCharacterController()
    {
        _controller = GetComponent<CharacterController>();
    }

    /// <summary>
    /// プレイヤーのパラメータの設定
    /// </summary>
    private void SettingPlayerParameter()
    {
        // reset our timeouts on start
        _jumpTimeoutDelta = JUMP_TIMEOUT;
        _fallTimeoutDelta = FALL_TIMEOUT;
    }

#endregion

    #region キャラクターの挙動制御用関数

    /// <summary>
    /// キャラクターが着地しているか確認する処理
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
    /// 重力設定を反映する処理
    /// </summary>
    private void CaluculateGravitySettings()
    {
        if (_verticalVelocity < TERMINAL_VELOCITY)
        {
            _verticalVelocity += GRAVITY * Time.deltaTime;
        }
    }

    /// <summary>
    /// 空中にいる際のジャンプに関する計算をする処理
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
    /// 陸上にいる際のジャンプに関する計算をする処理
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
    /// キャラクターの移動に関する処理
    /// 先にRot()
    /// </summary>
    private void Move()
    {
        float targetSpeed = SPRINT_SPEED;
        if (playerMove == Vector2.zero) targetSpeed = 0.0f;
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        const float SPEED_OFFSET = 0.1f;
        float inputMagnitude = playerMove.magnitude;

        //ぱっと見移動系計算
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
    /// キャラクターの回転に関する処理
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

    #region ActionListenerのコールバック関数

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

    #region カメラの挙動制御用関数

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

    #region サウンド関係

    /// <summary>
    /// 足音のサウンド再生処理
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
    /// 着地した際のサウンド再生処理
    /// </summary>
    private void OnLand(AnimationEvent animationEvent)
    {
        //if (animationEvent.animatorClipInfo.weight > 0.5f)
        //{
        //    AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FOOTSTEP_AUDIO_VOLUME);
        //}
    }

    #endregion

    #region ステート呼び出し用関数

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
        Move();//要検証、空中入力を受け付けるべきかどうか
        CaluculateJumpSettingsInAir();
        CaluculateGravitySettings();
    }
    #endregion
}

