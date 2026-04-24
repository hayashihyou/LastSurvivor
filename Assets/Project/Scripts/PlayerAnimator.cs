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

        [Header("射撃設定"), SerializeField]
        private BulletController _bulletController;

        [Header("連射速度"), SerializeField]
        private float _fireRate = 0.1f;

        [Header("反動設定"), SerializeField]
        private Transform _weaponPivot;

        [Header("反動の強さ"), SerializeField]
        private float _recoilForce = 0.05f;

        [Header("元に戻る速さ"), SerializeField]
        private float _returnSpeed = 5f;

        // 武器の元の位置を保存する変数
        private Vector3 _originalPosition;

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
            _originalPosition = _weaponPivot.localPosition;
            SubscribeEvents();
        }

        void Update()
        {
            // 武器の位置を元に戻す
            _weaponPivot.localPosition = Vector3.Lerp(_weaponPivot.localPosition, _originalPosition, Time.deltaTime * _returnSpeed);
        }

        void ApplyRecoilTask()
        {
            // 武器の位置を反動分だけ後ろに移動させる
            _weaponPivot.localPosition -= new Vector3(0, _recoilForce, -_recoilForce);
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

            // 毎フレーム、射撃ボタンが押されているかとプレイヤーが死亡していないかをチェック
            Observable.EveryUpdate()
                    .Subscribe(_ =>
                    {
                        bool isFiring = Input.GetButton("Fire1") && !_playerStatus.IsDead.Value;
                        _animator.SetBool(IsFiring, isFiring);
                    })
                    .AddTo(this);


            // 射撃ボタンが押されていて、プレイヤーが死亡していない場合に、一定の連射速度で弾を発射する
            Observable.EveryUpdate()
                .Where(_ => Input.GetButton("Fire1") && !_playerStatus.IsDead.Value)
                .ThrottleFirst(System.TimeSpan.FromSeconds(_fireRate),UnityTimeProvider.Update)
                .Subscribe(_ =>
                {
                    _bulletController.ShootTask();
                    // 武器の反動を適用するタスク
                    ApplyRecoilTask();
                })
                .AddTo(this);

            // プレイヤーの死亡状態を購読し、アニメーターのisDeadパラメータを更新
            _playerStatus.IsDead
                .Subscribe(isDead =>
                    _animator.SetBool(IsDead, isDead))
                .AddTo(this);
        }
    }
}
