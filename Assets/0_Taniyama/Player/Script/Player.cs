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
    #region 別コンポーネント

    private Animator _animator;
    private CharacterController _controller;
    private GameObject _mainCamera;

    #endregion

    #region ステータス

    [Header("ステータス")]
    [Tooltip("走りの移動速度"),SerializeField] private float SPRINT_SPEED = 5.335f;
    [Tooltip("歩きの移動速度"),SerializeField] private float MOVE_SPEED = 5.335f;

    [Tooltip("移動速度の変化率")]private float SPEED_CHANGE_RATE = 10.0f;
    [Tooltip("最大回転速度"),Range(0.0f, 0.3f)] private float ROTATION_SMOOTH_TIME = 0.12f;

    [Space(10)]
    [Tooltip("ジャンプクールタイム"),SerializeField] private float JUMP_TIMEOUT = 0.50f;
    [Tooltip("落下までの時間"),SerializeField] private float FALL_TIMEOUT = 0.15f;

    [Space(10)]
    [Header("スライディング関係")]
    [Tooltip("スライディングの時間"),SerializeField] private float SLIDING_TIMEOUT = 1.0f;
    [Tooltip("スライディングの速度"),SerializeField] private float SLIDING_SPEED = 6.0f;

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
    private float _slidingTimeoutDelta;

    #endregion

    #region インターフェース

    [Space(10)]
    [Header("インターフェース")]
    [Tooltip("移動"), SerializeReference, SubclassSelector] I_Move move = new ControlledMove();
    [Tooltip("スライディング"), SerializeReference, SubclassSelector] I_Sliding sliding = new DefaultSliding();
    [Tooltip("ジップライン"), SerializeReference, SubclassSelector] I_ZipLine zipLine = new DefaultZipLine();
    [Tooltip("クライミング"), SerializeReference, SubclassSelector] I_Climbing climbing = new DefaultClimbing();

    #endregion

    #region 着地判定用の変数

    [Space(10)]
    [Header("着地判定")]
    [Tooltip("地面に接しているが"),SerializeField,ReadOnly] private bool isGrounded = true;
    [Tooltip("PlayerのTransformから計測する頂点の距離"),SerializeField] private float GROUNDED_OFFSET = -0.14f;
    [Tooltip("地面と離れた判定をする球の半径"),SerializeField] private float GROUNDED_RADIUS = 0.28f;
    [Tooltip("地面のレイヤー"),SerializeField] private LayerMask GroundLayers;

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
    [Tooltip("上限"),SerializeField] private float TopClamp = 70.0f;
    [Tooltip("下限"), SerializeField] private float BottomClamp = -30.0f;
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

    #region サウンド関係

    [SerializeField] private AudioClip LandingAudioClip;
    [SerializeField] private AudioClip[] FootstepAudioClips;
    [Range(0, 1),SerializeField] private float FOOTSTEP_AUDIO_VOLUME = 0.5f;

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
    private int _animIDSliding;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;
    private int _animIDClimbing_x;
    private int _animIDClimbing_y;
    private int _animIDClimbingStart;
    private int _animIDClimbingUp;
    private int _animIDClimbingDown;
    private int _animIDZipLine;

    //IKアニメーションの設定
    private Vector3 rightHandIKPosition;
    private Vector3 leftHandIKPosition;
    private Vector3 rightLegIKPosition;
    private Vector3 leftLegIKPosition;

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

    #region ZipLine設定用関数



    #endregion

    #region ActionListenerのコールバック関数

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
        if (isGrounded == false) return; //着地してない場合
        if (sliding.IsGuard() == true) return;//ステート特有のガード節

        base.OnSlide(context);

        if (context.phase == InputActionPhase.Started)
        {
            CustomEvent.Trigger(gameObject, "useSliding");
        }
    }

    public override void OnZipLine(InputAction.CallbackContext context)
    {
        if (GameData.G_AllCheck() == true) return;
        if (zipLine.IsGuardOnTrigger() == true) return;//ステート特有のガード節

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
        move.Move();
        CaluculateJumpSettingsOnGround();
    }

    public void InAirState()
    {
        GameData.G_AllCheck();

        Rot();
        move.Move();//要検証、空中入力を受け付けるべきかどうか
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

    #region インターフェース呼び出し用関数

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

    #region アニメーターIK用
    
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

    #region 外部インターフェース

    public void TrampolineJump(float trampolineJumpPower)
    {
        _verticalVelocity = trampolineJumpPower;
        _animator.SetBool(_animIDJump, true);
    }

    #endregion
}

