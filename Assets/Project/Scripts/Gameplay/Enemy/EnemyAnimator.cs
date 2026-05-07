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

        // Animatorのパラメータ名
        private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
        private static readonly int IsChasingHash = Animator.StringToHash("IsChasing");
        private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
        private static readonly int IsDeadHash = Animator.StringToHash("IsDead");

        // エネミーのステータス、移動、攻撃を管理するクラスへの参照
        private EnemyStatus _enemyStatus;
        private EnemyMover _enemyMover;
        private EnemyAttacker _enemyAttacker;

        /// <summary>
        /// 初期化処理
        /// </summary>
        void Start()
        {
            // コンポーネントの取得
            _enemyStatus = GetComponent<EnemyStatus>();
            _enemyMover = GetComponent<EnemyMover>();
            _enemyAttacker = GetComponent<EnemyAttacker>();

            SubscribeAll();
        }

        /// <summary>
        /// エネミーの状態を監視し、アニメーターのパラメータを更新するための購読処理
        /// </summary>
        void SubscribeAll()
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

        /// <summary>
        /// Animatorのパラメータを安全に更新するためのヘルパーメソッド
        /// </summary>
        /// <param name="paramHash">更新するパラメータのハッシュ値</param>
        /// <param name="value">設定する値</param>
        void SetBool(int paramHash, bool value)
        {
            if(_animator == null)
            {
                return;
            }

            _animator.SetBool(paramHash, value);
        }
    }
}
