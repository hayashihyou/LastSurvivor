namespace LastSurvivor
{
    using UnityEngine;
    using R3;

    /// <summary>
    /// プレイヤーの移動を管理するクラス
    /// </summary>
    public class PlayerMover : MonoBehaviour
    {
        [Header("移動速度"), SerializeField]
        private CharacterController _characterController;

        [Header("カメラ"), SerializeField]
        private Transform _cameraTransform;

        [Header("重力"), SerializeField]
        private float _gravity = -9.81f;

        [Header("設置判定"), SerializeField]
        private Transform _groundCheck;
        private float _groundDistance = 0.3f;

        [Header("地面判定"), SerializeField]
        private LayerMask _groundMask;

        // プレイヤーの状態を管理するクラス
        private PlayerStatus _playerStatus;

        // 重力の影響を受ける速度
        private Vector3 _velocity;

        // 地面に接触しているかのフラグ
        private bool _isGrounded;

        // R3で値の変化を監視
        public ReactiveProperty<bool> IsRunning { get; private set; }
        public ReactiveProperty<bool> IsMoving { get; private set; }

        /// <summary>
        /// Start()より前に呼ばれる処理
        /// </summary>
        void Awake()
        {
            // R3のReactivePropertyを初期化
            IsRunning = new ReactiveProperty<bool>(false);
            IsMoving = new ReactiveProperty<bool>(false);
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        void Start()
        {
            _playerStatus = GetComponent<PlayerStatus>();

            if(_cameraTransform == null)
            {
                _cameraTransform = Camera.main.transform;
            }
        }

        /// <summary>
        /// 毎フレーム呼ばれる更新処理
        /// </summary>
        void Update()
        {
            if(_playerStatus.IsDead.Value)
            {
                return;
            }

            CheckGroundTask();
            HandleMovementTask();
            ApplyGravity();
        }

        /// <summary>
        /// 地面に接触しているかを確認
        /// </summary>
        private void CheckGroundTask()
        {
           _isGrounded = Physics.CheckSphere(
                _groundCheck.position,
                _groundDistance,
                _groundMask
            );

            if(_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }
        }

        /// <summary>
        /// プレイヤーの移動
        /// </summary>
        private void HandleMovementTask()
        {
            // 入力を取得
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            IsRunning.Value = Input.GetKey(KeyCode.LeftShift);
            IsMoving.Value = horizontal != 0 || vertical != 0;

            if (!IsMoving.Value)
            {
                return;
            }

            // 入力に基づいて移動方向を計算
            Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
            Vector3 cameraForward = _cameraTransform.forward;
            Vector3 cameraRight = _cameraTransform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;

            // カメラの向きに基づいて移動方向を調整
            Vector3 moveDirection = (cameraForward * direction.z + cameraRight * direction.x).normalized;

            // 移動速度を取得
            float speed = IsRunning.Value ? _playerStatus.RunSpeed : _playerStatus.WalkSpeed;

            // キャラクターを移動
            _characterController.Move(moveDirection * speed * Time.deltaTime);

            transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        /// <summary>
        /// 重力を適用
        /// </summary>
        private void ApplyGravity()
        {
            if(_characterController.isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            _velocity.y += _gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }
    }
}