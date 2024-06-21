using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]

public class Player : SingletonActionListener<Player>, I_Move
{
    #region 別コンポーネント

    public Animator _animator { private set; get; }
    public CharacterController _controller { private set; get; }
    public GameObject _mainCamera { private set; get; }

    #endregion

    #region ステータス

    [field: Header("ステータス")]
    [field: Tooltip("走りの移動速度")]
    [field: SerializeField] public float SPRINT_SPEED { private set; get; } = 5.335f;

    [field: Tooltip("歩きの移動速度")]
    [field: SerializeField] public float MOVE_SPEED { private set; get; } = 5.335f;

    [field: Tooltip("移動速度の変化率")]
    public float SPEED_CHANGE_RATE { private set; get; } = 10.0f;

    [field: Tooltip("最大回転速度")]
    [field: Range(0.0f, 0.3f)]
    public float ROTATION_SMOOTH_TIME { private set; get; } = 0.12f;

    [field: Space(10)]
    [field: Tooltip("ジャンプクールタイム")]
    [field: SerializeField] public float JUMP_TIMEOUT { private set; get; } = 0.50f;

    [field: Tooltip("落下までの時間")]
    [field: SerializeField] public float FALL_TIMEOUT { private set; get; } = 0.15f;

    //移動
    [HideInInspector] public float _speed;
    [HideInInspector] public float _animationBlend;
    //回転
    public float _targetRotation { private set; get; } = 0.0f;
    [HideInInspector] public float _rotationVelocity;
    //落下
    public float _verticalVelocity{private set; get;}
    public float TERMINAL_VELOCITY { private set; get; } = 53.0f;
    //時間
    public float _jumpTimeoutDelta{private set; get;}
    public float _fallTimeoutDelta{private set; get;}
    #endregion

    #region 着地判定用の変数

    [field: Header("着地判定")]
    [field: Tooltip("地面に接しているが")]
    [field: SerializeField, ReadOnly] public bool isGrounded { private set; get; } = true;

    [field: Tooltip("PlayerのTransformから計測する頂点の距離")]
    [field: SerializeField] public float GROUNDED_OFFSET { private set; get; } = -0.14f;

    [field: Tooltip("地面と離れた判定をする球の半径")]
    [field: SerializeField] public float GROUNDED_RADIUS { private set; get; } = 0.28f;

    [field: Tooltip("地面のレイヤー")]
    [field: SerializeField] public LayerMask GroundLayers{private set; get;}

    #endregion

    #region ActionListenerの用変数

    public Vector2 playerMove { private set; get; } = Vector2.zero;
    public Vector2 camMove { private set; get; } = Vector2.zero;
    public bool isSlide { private set; get; } = false;
    public bool isSlow { private set; get; } = false;

    #endregion

    #region カメラ用の変数
    [field: Header("カメラ")]
    [field: SerializeField] public GameObject CinemachineCameraTarget{private set; get;}
    [field: Tooltip("上限")]
    [field: SerializeField] public float TopClamp { private set; get; } = 70.0f;
    [field: Tooltip("下限")]
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

    #region サウンド関係

    [field: SerializeField] public AudioClip LandingAudioClip{private set; get;}
    [field: SerializeField] public AudioClip[] FootstepAudioClips{private set; get;}
    [field: Range(0, 1), SerializeField] public float FOOTSTEP_AUDIO_VOLUME { private set; get; } = 0.5f;

    #endregion

    #region ジャンプ用の変数

    [field: SerializeField] public float JUMP_HEIGHT { private set; get; } = 1.2f;
    [field: SerializeField] public float GRAVITY { private set; get; } = -15.0f;

    #endregion

    #region アニメーターの設定

    //アニメーターの設定用
    public int _animIDSpeed{private set; get;}
    public int _animIDGrounded{private set; get;}
    public int _animIDJump{private set; get;}
    public int _animIDFreeFall{private set; get;}
    public int _animIDMotionSpeed{private set; get;}

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
        ((I_Move)this).Move();
        CaluculateJumpSettingsOnGround();
    }

    public void InAirState()
    {
        GameData.G_AllCheck();

        Rot();
        ((I_Move)this).Move();//要検証、空中入力を受け付けるべきかどうか
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
    /// キャラクターの移動に関する処理
    /// 先にRot()
    /// </summary>
    public void Move()
    {
        float targetSpeed = GetThis().SPRINT_SPEED;
        if (GetThis().playerMove == Vector2.zero) targetSpeed = 0.0f;
        float currentHorizontalSpeed = new Vector3(GetThis()._controller.velocity.x, 0.0f, GetThis()._controller.velocity.z).magnitude;
        const float SPEED_OFFSET = 0.1f;
        float inputMagnitude = GetThis().playerMove.magnitude;

        //ぱっと見移動系計算
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