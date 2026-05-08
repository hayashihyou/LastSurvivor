namespace LastSurvivor
{
    using R3;
    using UnityEngine;

    /// <summary>
    /// 敵のアニメーションを管理するクラス
    /// </summary>
    public class EnemyAnimator : MonoBehaviour
    {
        [Header("アニメーター"),SerializeField]
        private Animator _animator;

        [Header("エネミーステータス"),SerializeField]
        private EnemyStatus _enemyStatus;

        [Header("エネミー移動"),SerializeField]
        private EnemyMover _enemyMover;

        [Header("エネミー攻撃"),SerializeField]
        private EnemyAttacker _enemyAttacker;

        // Animatorのパラメータ名
        private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
        private static readonly int IsChasingHash = Animator.StringToHash("IsChasing");
        private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
        private static readonly int IsDeadHash = Animator.StringToHash("IsDead");

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Start()
        {
            SubscribeAll();
        }

        /// <summary>
        /// エネミーの状態を監視し、アニメーターのパラメータを更新するための購読処理
        /// </summary>
        private void SubscribeAll()
        {
            _enemyMover.IsMoving
                .Subscribe(isMoving => _animator.SetBool(IsMovingHash, isMoving))
                .AddTo(this);

            _enemyMover.IsChasing
                .Subscribe(isChasing => _animator.SetBool(IsChasingHash, isChasing))
                .AddTo(this);

            _enemyAttacker.IsAttacking
                .Subscribe(isAttacking => _animator.SetBool(IsAttackingHash, isAttacking))
                .AddTo(this);

            _enemyStatus.IsDead
                .Where(isDead => isDead)
                .Take(1)
                .Subscribe(isDead => _animator.SetBool(IsDeadHash, isDead))
                .AddTo(this);
        }
    }
}
