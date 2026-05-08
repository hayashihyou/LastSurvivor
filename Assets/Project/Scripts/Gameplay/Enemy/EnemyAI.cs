namespace LastSurvivor
{
    using R3;
    using UnityEngine;

    /// <summary>
    /// 敵のAIを管理するクラス
    /// </summary>
    public class EnemyAI : MonoBehaviour
    {
        /// <summary>
        /// 敵の状態を表す列挙型
        /// </summary>
        public enum EnemyState
        {
            Idle,
            Chase,
            Attack,
            Dead
        }

        // 敵のステータスを管理するクラスへの参照
        [Header("敵のステータス"),SerializeField]
        private EnemyStatus _enemyStatus;

        // 敵の移動を管理するクラスへの参照
        [Header("敵の移動"),SerializeField]
        private EnemyMover _enemyMover;

        // 敵の攻撃を管理するクラスへの参照
        [Header("敵の攻撃"),SerializeField]
        private EnemyAttacker _enemyAttacker;

        // プレイヤーのTransformへの参照
        [Header("プレイヤーのTransform"),SerializeField]
        private Transform _playerTransform;

        // 敵の現在の状態を管理するReactiveProperty
        public ReactiveProperty<EnemyState> CurrentState { get; private set; } = new ReactiveProperty<EnemyState>(EnemyState.Idle); 

        /// <summary>
        /// ゲーム開始時に呼び出される初期化メソッド
        /// </summary>
        private void Start()
        {
            var playerObject = GameObject.FindGameObjectWithTag(TagConsts.Player);
            if (playerObject != null)
            {
                _playerTransform = playerObject.transform;
            }

            // 敵の死亡状態を監視し、死亡した場合に状態をDeadに遷移させる
            _enemyStatus.IsDead
                .Where(isDead => isDead)
                .Subscribe(_ => TransitionTo(EnemyState.Dead))
                .AddTo(this);
        }

        /// <summary>
        /// 毎フレーム呼び出される更新メソッド
        /// </summary>
        private void Update()
        {
            // 現在の状態に応じて適切な更新処理を呼び出す
            switch (CurrentState.Value)
            {
                case EnemyState.Idle:
                    UpdateIdle();
                    break;

                case EnemyState.Chase:
                    UpdateChase();
                    break;

                case EnemyState.Attack:
                    UpdateAttack();
                    break;

                case EnemyState.Dead:
                    break;
            }
        }

        /// <summary>
        /// Idle状態の更新処理
        /// </summary>
        private void UpdateIdle()
        {
            if(_playerTransform == null)
            {
                return;
            }

            if(IsPlayerInRange(_enemyStatus.DetectionRange))
            {
                TransitionTo(EnemyState.Chase);
            }
        }

        /// <summary>
        /// Chase状態の更新処理
        /// </summary>
        private void UpdateChase()
        {
            if(_playerTransform == null)
            {
                return;
            }

            if (IsPlayerInRange(_enemyStatus.AttackRange))
            {
                TransitionTo(EnemyState.Attack);
                return;
            }

            if(!IsPlayerInRange(_enemyStatus.DetectionRange))
            {
                TransitionTo(EnemyState.Idle);
            }
        }

        /// <summary>
        /// Attack状態の更新処理
        /// </summary>
        private void UpdateAttack()
        {
            if(_playerTransform == null)
            {
                return;
            }

            if(!IsPlayerInRange(_enemyStatus.AttackRange))
            {
                TransitionTo(EnemyState.Chase);
                return;
            }

            _enemyAttacker.TryAttack();
        }

        /// <summary>
        /// 状態遷移を管理するメソッド
        /// </summary>
        /// <param name="nextState">遷移先の状態</param>
        private void TransitionTo(EnemyState nextState)
        {
            if(CurrentState.Value == nextState)
            {
                return;
            }

            OnExitState(CurrentState.Value);
            CurrentState.Value = nextState;
            OnEnterState(nextState);
        }

        /// <summary>
        /// 状態に入る際の処理を管理するメソッド
        /// </summary>
        /// <param name="state">遷移先の状態</param>
        private void OnEnterState(EnemyState state)
        {
            switch (state)
            {
                case EnemyState.Idle:
                    _enemyMover.SetChasing(false);
                    break;

                case EnemyState.Chase:
                    _enemyMover.SetChasing(true);
                    break;

                case EnemyState.Attack:
                    _enemyMover.SetChasing(false);
                    break;

                case EnemyState.Dead:
                    _enemyMover.SetChasing(false);
                    break;
            }
        }

        /// <summary>
        /// 状態から出る際の処理を管理するメソッド
        /// </summary>
        /// <param name="state">遷移元の状態</param>
        private void OnExitState(EnemyState state) { }

        /// <summary>
        /// プレイヤーが指定した範囲内にいるかどうかを判定するメソッド
        /// </summary>
        /// <param name="range">判定する範囲</param>
        private bool IsPlayerInRange(float range)
        {
            return Vector3.Distance(transform.position, _playerTransform.position) <= range;
        }
    }
}
