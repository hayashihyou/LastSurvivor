namespace LastSurvivor
{
    using UnityEngine;
    using R3;

    /// <summary>
    /// �v���C���[�̈ړ����Ǘ�����N���X
    /// </summary>
    public class PlayerMover : MonoBehaviour
    {
        [Header("キャラクターコントローラー"), SerializeField]
        private CharacterController _characterController;

        [Header("カメラ"), SerializeField]
        private Transform _cameraTransform;

        [Header("重力"), SerializeField]
        private float _gravity = -9.81f;

        [Header("地面チェック"), SerializeField]
        private Transform _groundCheck;
        private float _groundDistance = 0.3f;

        [Header("地面マスク"), SerializeField]
        private LayerMask _groundMask;

        // プレイヤーステータス
        private PlayerStatus _playerStatus;

        // プレイヤー速度
        private Vector3 _velocity;

        // 地面接触判定
        private bool _isGrounded;

        // プレイヤーの走行状態と移動状態
        public ReactiveProperty<bool> IsRunning { get; private set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsMoving { get; private set; } = new ReactiveProperty<bool>(false);

        /// <summary>
        /// インスタンス化直後に呼び出される初期化処理
        /// </summary>
        void Start()
        {
            _playerStatus = GetComponent<PlayerStatus>();

            if (_cameraTransform == null)
            {
                _cameraTransform = Camera.main.transform;
            }
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
            HandleMovementTask();
            ApplyGravity();
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

        /// <summary>
        /// プレイヤーの移動
        /// </summary>
        private void HandleMovementTask()
        {
            // 入力の取得
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

            // カメラの向きに基づいて移動方向を変換
            Vector3 moveDirection = (cameraForward * direction.z + cameraRight * direction.x).normalized;

            // 速度の計算
            float speed = IsRunning.Value ? _playerStatus.RunSpeed : _playerStatus.WalkSpeed;

            // プレイヤーを移動
            _characterController.Move(moveDirection * speed * Time.deltaTime);

            transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        /// <summary>
        /// 重力の適用
        /// </summary>
        private void ApplyGravity()
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