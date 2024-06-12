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

    #region 着地判定用の変数

    [Header("着地判定")]
    [Tooltip("地面に接しているが")]
    [SerializeField] private bool isGrounded = true;

    [Tooltip("PlayerのTransformから計測する頂点の距離")]
    [SerializeField] private float groundedOffset = -0.14f;

    [Tooltip("地面と離れた判定をする球の半径")]
    [SerializeField] private float groundedRadius = 0.28f;

    [Tooltip("地面のレイヤー")]
    [SerializeField] private LayerMask GroundLayers;

    #endregion

    #region ActionListenerの用変数

    private Vector2 move = Vector2.zero;
    private bool isJump = false;
    private bool isSlide = false;
    private bool isSlow = false;

    #endregion

    #region ジャンプ用の変数

    [SerializeField] private float JumpHeight = 1.2f;
    [SerializeField] private float Gravity = -15.0f;
    private float _jumpTimeoutDelta;
    private float _verticalVelocity;

    #endregion
    //アニメーターの設定用
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
    }

    /// <summary>
    /// キャラクターが着地しているか確認する処理
    /// </summary>
    private void GroundCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = LibVector.Add_Y(transform.position, -groundedOffset);
        isGrounded = Physics.CheckSphere(spherePosition, groundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        _animator.SetBool(_animIDGrounded, isGrounded); 
    }

    #region ActionListenerのコールバック関数

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
