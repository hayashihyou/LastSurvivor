namespace LastSurvivor
{
    using UnityEngine;
    using R3;

    /// <summary>
    /// プレイヤーの移動と回転を制御するクラス
    /// </summary>
    public class PlayerMover : MonoBehaviour
    {
        [Header("キャラクターコントローラー"), SerializeField]
        private CharacterController _characterController;

        [Header("カメラ"), SerializeField]
        private Transform _cameraTransform;

        [Header("マウス感度"), SerializeField]
        private float _mouseSensitivity = 2f;

        [Header("視点上限"), SerializeField]
        private float _maxViewAngle = 80f;

        [Header("視点下限"), SerializeField]
        private float _minViewAngle = -80f;

        [Header("重力"), SerializeField]
        private float _gravity = -9.81f;

        [Header("地面チェック"), SerializeField]
        private Transform _groundCheck;
        private float _groundDistance = 0.3f;

        [Header("地面マスク"), SerializeField]
        private LayerMask _groundMask;

        [Header("FPSカメラ"), SerializeField]
        private Transform _fpsCamera;

        // プレイヤーステータス
        private PlayerStatus _playerStatus;

        // プレイヤー速度
        private Vector3 _velocity;

        // 地面接触判定
        private bool _isGrounded;

        // カメラのピッチ角度
        private float _cameraPitch = 0f;

        // プレイヤーの走行状態と移動状態
        public ReactiveProperty<bool> IsRunning { get; private set; } 
        public ReactiveProperty<bool> IsMoving { get; private set; }
        void Awake()
        {
            IsRunning = new ReactiveProperty<bool>(false);
            IsMoving = new ReactiveProperty<bool>(false);

            // マウスを固定して非表示にする
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        /// <summary>
        /// インスタンス化直後に呼び出される初期化処理
        /// </summary>
        void Start()
        {
            _playerStatus = GetComponent<PlayerStatus>();
        }

        /// <summary>
        /// 更新
        /// </summary>
        void Update()
        {
            if (_playerStatus.IsDead.Value)
            {
                return;
            }

            CheckGroundTask();
            HandleRotationTask();
            HandleMovementTask();
            ApplyGravityTask();
        }

        /// <summary>
        /// 地面接触チェック
        /// </summary>
        private void CheckGroundTask()
        {
            _isGrounded = Physics.CheckSphere(
                 _groundCheck.position,
                 _groundDistance,
                 _groundMask
             );

            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }
        }

        private void HandleRotationTask()
        {
            // マウス入力の取得
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;
            
            // プレイヤーの水平回転
            transform.Rotate(Vector3.up * mouseX);
            
            // カメラの垂直回転
            _cameraPitch -= mouseY;
            _cameraPitch = Mathf.Clamp(_cameraPitch, _minViewAngle, _maxViewAngle);
            _cameraTransform.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);

            // FPSカメラの回転をプレイヤーの回転に合わせる  
            _fpsCamera.rotation = _cameraTransform.rotation;
        }

        /// <summary>
        /// プレイヤーの移動
        /// </summary>
        private void HandleMovementTask()
        {
            // 入力の取得
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            IsRunning.Value = Input.GetKey(KeyCode.LeftShift);
            IsMoving.Value = horizontal != 0 || vertical != 0;

            if (!IsMoving.Value)
            {
                return;
            }

            // カメラの向きに基づいて移動方向を変換
            Vector3 moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;

            // 速度の計算
            float speed = IsRunning.Value ? _playerStatus.RunSpeed : _playerStatus.WalkSpeed;

            // プレイヤーを移動
            _characterController.Move(moveDirection * speed * Time.deltaTime);
        }

        /// <summary>
        /// 重力の適用
        /// </summary>
        private void ApplyGravityTask()
        {
            if (_characterController.isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            _velocity.y += _gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }
    }
}