namespace LastSurvivor
{
    using UnityEngine;
    using R3;

    /// <summary>
    /// プレイヤーのアニメーションを制御するクラス
    /// </summary>
    public class PlayerAnimator : MonoBehaviour
    {
        [Header("アニメーター"), SerializeField]
        private Animator _animator;

        [Header("プレイヤーステータス"), SerializeField]
        private PlayerStatus _playerStatus;

        [Header("移動"), SerializeField]
        private PlayerMover _playerMover;

        [Header("射撃"), SerializeField]
        private PlayerShooter _playerShooter;

        /// <summary>
        /// アニメーターのパラメータ
        /// </summary>
        public static readonly int IsRunning = Animator.StringToHash("isRunning");
        public static readonly int IsMoving = Animator.StringToHash("isMoving");
        public static readonly int IsFiring = Animator.StringToHash("isFiring");
        public static readonly int IsDead = Animator.StringToHash("isDead");

        /// <summary>
        /// インスタンス化直後に呼び出される初期化処理
        /// </summary>
        void Awake()
        {
            // アニメーターがnullの場合、取得
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
        }

        /// <summary>
        /// インスタンス化直後に呼び出される初期化処理
        /// </summary>
        void Start()
        {
            SubscribeEvents();
        }

        /// <summary>
        ///　プレイヤーの移動状態と死亡状態のイベントを購読し、アニメーターのパラメータを更新する
        /// </summary>
        private void SubscribeEvents()
        {
            // プレイヤーの移動状態を購読し、アニメーターのisMovingパラメータを更新
            _playerMover.IsMoving
             .Subscribe(isMoving =>
                _animator.SetBool(IsMoving, isMoving))
             .AddTo(this);

            // プレイヤーの走行状態を購読し、アニメーターのisRunningパラメータを更新
            _playerMover.IsRunning
                .Subscribe(isRunning =>
                    _animator.SetBool(IsRunning, isRunning))
                .AddTo(this);

            // プレイヤーの射撃状態を購読し、アニメーターのisFiringパラメータを更新
            _playerShooter.IsFiring
                .Subscribe(isFiring =>
                    _animator.SetBool(IsFiring, isFiring))
                .AddTo(this);

            // プレイヤーの死亡状態を購読し、アニメーターのisDeadパラメータを更新
            _playerStatus.IsDead
                .Subscribe(isDead =>
                    _animator.SetBool(IsDead, isDead))
                .AddTo(this);
        }
    }
}
