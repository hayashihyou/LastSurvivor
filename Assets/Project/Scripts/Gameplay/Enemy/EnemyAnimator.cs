namespace LastSurvivor
{
    using Cysharp.Threading.Tasks;
    using R3;
    using UnityEngine;

    /// <summary>
    /// 敵のアニメーションを管理するクラス
    /// </summary>
    public class EnemyAnimator : MonoBehaviour
    {
        [Header("アニメーター"), SerializeField]
        private Animator _animator;

        [Header("エネミーAI"), SerializeField]
        private EnemyAI _enemyAI;

        [Header("エネミーステータス"), SerializeField]
        private EnemyStatus _enemyStatus;

        [Header("エネミー移動"), SerializeField]
        private EnemyMover _enemyMover;

        [Header("エネミー攻撃"), SerializeField]
        private EnemyAttacker _enemyAttacker;

        [Header("ScreamのSE"), SerializeField]
        private AudioClip _screamSE;

        [Header("ダメージSE"), SerializeField]
        private AudioClip _damageSE;

        [Header("死亡SE"), SerializeField]
        private AudioClip _deadSE;

        private AudioSource _audioSource;

        // Animatorのパラメータ名
        private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
        private static readonly int IsChasingHash = Animator.StringToHash("IsChasing");
        private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
        private static readonly int IsDeadHash = Animator.StringToHash("IsDead");
        private static readonly int IsHitHash = Animator.StringToHash("IsHit");
        private static readonly int ScreamTriggerHash = Animator.StringToHash("ScreamTrigger");

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            SubscribeAll();
        }

        /// <summary>
        /// エネミーの状態を監視し、アニメーターのパラメータを更新するための購読処理
        /// </summary>
        private void SubscribeAll()
        {
            _enemyMover.IsWalking
                .Subscribe(isMoving => _animator.SetBool(IsWalkingHash, isMoving))
                .AddTo(this);

            _enemyAI.CurrentState
                .Where(state => state == EnemyAI.EnemyState.Scream)
                .Take(1)
                .Subscribe(_ =>
                {
                    _animator.SetTrigger(ScreamTriggerHash);
                    if(_screamSE != null)
                    {
                        _audioSource.PlayOneShot(_screamSE);
                    }
                })
                .AddTo(this);

            _enemyMover.IsChasing
                .Subscribe(isChasing => _animator.SetBool(IsChasingHash, isChasing))
                .AddTo(this);

        _enemyStatus.OnDamaged
            .Where(_ => !_enemyStatus.IsDead.Value)
                .Subscribe(_ =>
                {
                    SetIsHitTask().Forget();
                    if (_damageSE != null)
                    {
                        _audioSource.PlayOneShot(_damageSE);
                    }
                })
                .AddTo(this);

        _enemyAttacker.IsAttacking
            .Subscribe(isAttacking => _animator.SetBool(IsAttackingHash, isAttacking))
                .AddTo(this);

        _enemyStatus.IsDead
            .Where(isDead => isDead)
                .Take(1)
                .Subscribe(isDead =>
                {
                    _animator.SetBool(IsDeadHash, isDead);
                    if (_deadSE != null)
                    {
                        _audioSource.PlayOneShot(_deadSE);
                    }
                })
                .AddTo(this);
        }
    

        /// <summary>
        /// 敵がダメージを受けたときにIsHitパラメータを一時的にtrueにする非同期処理
        /// </summary>
        private async UniTaskVoid SetIsHitTask()
        {
            _animator.SetBool(IsHitHash, true);
            await UniTask.Delay(500, cancellationToken: this.GetCancellationTokenOnDestroy());
            _animator.SetBool(IsHitHash, false);
        }
    }
}
