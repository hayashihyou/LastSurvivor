namespace LastSurvivor
{
    using R3;
    using UnityEngine;

    /// <summary>
    /// プレイヤーの足音を管理するクラス
    /// </summary>
    public class PlayerFootStep : MonoBehaviour
    {
        [Header("歩き足音SE"), SerializeField]
        private AudioClip _walkSE;

        [Header("走り足音SE"), SerializeField]
        private AudioClip _runSE;

        [Header("歩く音の間隔"), SerializeField]
        private float _walkInterval = 0.5f;

        [Header("走る音の間隔"), SerializeField]
        private float _runInterval = 0.3f;

        [Header("プレイヤーの移動"), SerializeField]
        private PlayerMover _playerMover;

        // オーディオソース
        private AudioSource _audioSource;

        // 移動中かどうかを管理するフラグ
        private bool _isMoving = false;

        // 走っているかどうかを管理するフラグ
        private bool _isRunning = false;

        // ジャンプ中かどうかを管理するフラグ
        private bool _isJumping = false;

        // 足音の再生間隔を管理するタイマー
        private float _timer = 0f;

        /// <summary>
        /// インスタンス化直後に呼び出される初期化処理
        /// </summary>
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>()
                ?? gameObject.AddComponent<AudioSource>();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Start()
        {
            _playerMover.IsMoving
                .Subscribe(OnIsMovingChanged)
                .AddTo(this);

            _playerMover.IsRunning
                .Subscribe(OnIsRunningChanged)
                .AddTo(this);

            // IsGroundedの代わりにIsJumpingを監視
            _playerMover.IsJumping
                .Subscribe(OnIsJumpingChanged)
                .AddTo(this);
        }

        /// <summary>
        /// 移動状態が変化したときの処理
        /// </summary>
        /// <param name="isMoving">移動中かどうか</param>
        private void OnIsMovingChanged(bool isMoving)
        {
            _isMoving = isMoving;
            if (!isMoving)
            {
                _audioSource.Stop();
                _timer = 0f;
            }
        }

        /// <summary>
        /// 走行状態が変化したときの処理
        /// </summary>
        /// <param name="isRunning">走行中かどうか</param>
        private void OnIsRunningChanged(bool isRunning)
        {
            _isRunning = isRunning;
            // 切り替え時にタイマーリセットして即次のSEへ
            _audioSource.Stop();
            _timer = 0f;
        }

        /// <summary>
        /// ジャンプ状態が変化したときの処理
        /// </summary>
        /// <param name="isJumping">ジャンプ中かどうか</param>
        private void OnIsJumpingChanged(bool isJumping)
        {
            _isJumping = isJumping;
            if (isJumping)
            {
                _audioSource.Stop();
                _timer = 0f;
            }
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        private void Update()
        {
            // ジャンプ中・停止中は再生しない
            if (!_isMoving || _isJumping) return;

            _timer -= Time.deltaTime;

            if (_timer <= 0f)
            {
                _audioSource.Stop();
                _audioSource.clip = _isRunning ? _runSE : _walkSE;
                _audioSource.Play();

                // 間隔をタイマーで管理
                _timer = _isRunning ? _runInterval : _walkInterval;
            }
        }
    }
}