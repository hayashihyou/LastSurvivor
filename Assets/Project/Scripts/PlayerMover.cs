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
        private CharacterController characterController;

        [Header("カメラ"), SerializeField]
        private Transform cameraTransform;

        [Header("重力"), SerializeField]
        private float gravity = -9.81f;

        [Header("設置判定"), SerializeField]
        private Transform groundCheck;
        private float groundDistance = 0.3f;

        [Header("地面判定"), SerializeField]
        private LayerMask groundMask;

        private PlayerStatus playerStatus;
        private Vector3 velocity;
        private bool isGrounded;

        // R3で値の変化を監視
        public ReactiveProperty<bool> isRunning { get; private set; }
        public ReactiveProperty<bool> isMoving { get; private set; }

        /// <summary>
        /// Start()より前に呼ばれる処理
        /// </summary>
        void Awake()
        {
            // R3のReactivePropertyを初期化
            isRunning = new ReactiveProperty<bool>(false);
            isMoving = new ReactiveProperty<bool>(false);
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        void Start()
        {
            playerStatus = GetComponent<PlayerStatus>();

            if(cameraTransform == null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        /// <summary>
        /// 毎フレーム呼ばれる更新処理
        /// </summary>
        void Update()
        {
            if(playerStatus.isDead.Value)
            {
                return;
            }

            CheckGround();
            HandleMovement();
            ApplyGravity();
        }

        /// <summary>
        /// 地面に接触しているかを確認
        /// </summary>
        private void CheckGround()
        {
           isGrounded = Physics.CheckSphere(
                groundCheck.position,
                groundDistance,
                groundMask
            );

            if(isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
        }

        /// <summary>
        /// プレイヤーの移動
        /// </summary>
        private void HandleMovement()
        {
            // 入力を取得
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            isRunning.Value = Input.GetKey(KeyCode.LeftShift);
            isMoving.Value = horizontal != 0 || vertical != 0;

            if (!isMoving.Value)
            {
                return;
            }

            // 入力に基づいて移動方向を計算
            Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;

            // カメラの向きに基づいて移動方向を調整
            Vector3 moveDirection = (cameraForward * direction.z + cameraRight * direction.x).normalized;

            // 移動速度を取得
            float speed = isRunning.Value ? playerStatus.RunSpeed : playerStatus.WalkSpeed;

            // キャラクターを移動
            characterController.Move(moveDirection * speed * Time.deltaTime);

            transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        /// <summary>
        /// 重力を適用
        /// </summary>
        private void ApplyGravity()
        {
            if(characterController.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }
    }
}