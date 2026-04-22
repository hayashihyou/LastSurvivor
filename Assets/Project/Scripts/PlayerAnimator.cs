namespace LastSurvivor
{
    using UnityEngine;
    using R3;

    /// <summary>
    /// プレイヤーのアニメーションを管理するクラス
    /// </summary>
    public class PlayerAnimator : MonoBehaviour
    {
        [Header("アニメーター"), SerializeField]
        private Animator _animator;

        [Header("プレイヤーのステータス"), SerializeField]
        private PlayerStatus _playerStatus;

        [Header("プレイヤーの移動"), SerializeField]
        private PlayerMover _playerMover;

        /// <summary>
        /// アニメーターのパラメーターを定義
        /// </summary>
        public static readonly int IsRunning = Animator.StringToHash("isRunning");
        public static readonly int IsMoving = Animator.StringToHash("isMoving");
        public static readonly int IsDead = Animator.StringToHash("isDead");



        /// <summary>
        /// Start()より先に呼ばれる処理
        /// </summary>
        void Awake()
        {
            // アニメーターがnullの時、アニメーターコンポーネントを取得
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        void Start()
        {
            SubscribeEvents();
        }

        /// <summary>
        /// プレイヤーの状態に応じてアニメーターのパラメーターを更新
        /// </summary>
        private void SubscribeEvents()
        {
            _playerMover.IsMoving
             .Subscribe(isMoving =>
                _animator.SetBool(IsMoving, isMoving))
             .AddTo(this);

            _playerMover.IsRunning
                .Subscribe(isRunning =>
                    _animator.SetBool(IsRunning, isRunning))
                .AddTo(this);

            _playerStatus.IsDead
                .Subscribe(isDead =>
                    _animator.SetBool(IsDead, isDead))
                .AddTo(this);
        }
    }
}
