namespace LastSurvivor
{
    using R3;
    using UnityEngine;

    /// <summary>
    /// 敵のAIを管理するクラス
    /// </summary>
    public class EnemyAI : MonoBehaviour
    {
        public enum EnemyState
        {
            Idle,
            Walk,
            Scream,
            Chase,
            Attack,
            Dead
        }

        [Header("敵のステータス"), SerializeField]
        private EnemyStatus _enemyStatus;

        [Header("敵の移動"), SerializeField]
        private EnemyMover _enemyMover;

        [Header("敵の攻撃"), SerializeField]
        private EnemyAttacker _enemyAttacker;

        [Header("Screamアニメーションの長さ"), SerializeField]
        private float _screamDuration = 2f;
        private float _screamTimer;

        private bool _hasScreamed = false;
        private Transform _playerTransform;

        public ReactiveProperty<EnemyState> CurrentState { get; private set; }
            = new ReactiveProperty<EnemyState>(EnemyState.Idle);

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Start()
        {
            var playerObject = GameObject.FindGameObjectWithTag(TagConsts.Player);
            if (playerObject != null)
            {
                _playerTransform = playerObject.transform;
            }

            _enemyStatus.IsDead
                .Where(isDead => isDead)
                .Subscribe(_ => TransitionTo(EnemyState.Dead))
                .AddTo(this);
        }

        /// <summary>
        /// 状態に応じた更新処理
        /// </summary>
        private void Update()
        {
            switch (CurrentState.Value)
            {
                case EnemyState.Idle: 
                    UpdateIdle(); 
                    break;

                case EnemyState.Walk: 
                    UpdateWalk();
                    break;

                case EnemyState.Scream: 
                    UpdateScream(); 
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
            if (_hasScreamed)
            {
                TransitionTo(EnemyState.Walk);
                return;
            }

            if (!IsPlayerInRange(_enemyStatus.DetectionRange))
            {
                TransitionTo(EnemyState.Walk);
                return;
            }

            TransitionTo(EnemyState.Scream);
        }

        /// <summary>
        /// Walk状態の更新処理
        /// </summary>
        private void UpdateWalk()
        {
            if (IsPlayerInRange(_enemyStatus.DetectionRange))
            {
                TransitionTo(_hasScreamed ? EnemyState.Chase : EnemyState.Scream);
                return;
            }

            if (_enemyMover.HasReachedDestination())
            {
                _enemyMover.SetRandomDestination();
            }
        }

        /// <summary>
        /// Scream状態の更新処理
        /// </summary>
        private void UpdateScream()
        {
            _screamTimer -= Time.deltaTime;

            if (_screamTimer <= 0f)
            {
                _hasScreamed = true;
                TransitionTo(EnemyState.Chase);
            }
        }

        /// <summary>
        /// Chase状態の更新処理
        /// </summary>
        private void UpdateChase()
        {
            if (_playerTransform == null)
            {
                return;
            }

            if (IsPlayerInRange(_enemyStatus.AttackRange))
            {
                TransitionTo(EnemyState.Attack);
                return;
            }

            if (!IsPlayerInRange(_enemyStatus.DetectionRange))
            {
                TransitionTo(EnemyState.Walk);
            }
        }

        /// <summary>
        /// Attack状態の更新処理
        /// </summary>
        private void UpdateAttack()
        {
            if (_playerTransform == null)
            {
                return;
            }

            if (!IsPlayerInRange(_enemyStatus.AttackRange))
            {
                TransitionTo(EnemyState.Chase);
                return;
            }

            _enemyAttacker.TryAttack();
        }

        /// <summary>
        /// 状態遷移処理
        /// </summary>
        /// <param name="nextState">遷移先の状態</param>
        private void TransitionTo(EnemyState nextState)
        {
            if (CurrentState.Value == nextState)
            {
                return;
            }

            OnExitState(CurrentState.Value);
            CurrentState.Value = nextState;
            OnEnterState(nextState);
        }

        /// <summary>
        /// 状態に入ったときの処理
        /// </summary>
        /// <param name="state">現在の状態</param>
        private void OnEnterState(EnemyState state)
        {
            switch (state)
            {
                case EnemyState.Idle:
                    _enemyMover.StopMovement();
                    _enemyMover.SetChasing(false);
                    _enemyMover.SetWalking(false);
                    break;

                case EnemyState.Walk:
                    _enemyMover.SetWalkSpeed();
                    _enemyMover.SetChasing(false);
                    _enemyMover.SetWalking(true);
                    _enemyMover.SetRandomDestination();
                    break;

                case EnemyState.Scream:
                    _enemyMover.StopMovement();
                    _enemyMover.SetRotationControl(false);
                    _enemyMover.SetChasing(false);
                    _screamTimer = _screamDuration;
                    LookAtPlayer();
                    break;

                case EnemyState.Chase:
                    _enemyMover.SetChaseSpeed();
                    _enemyMover.SetRotationControl(true);
                    _enemyMover.SetWalking(false);
                    _enemyMover.SetChasing(true);
                    _enemyAttacker.ResetAttack();
                    break;

                case EnemyState.Attack:
                    _enemyMover.StopMovement();
                    _enemyMover.SetChasing(false);
                    _enemyMover.SetWalking(false);
                    break;

                case EnemyState.Dead:
                    _enemyMover.StopMovement();
                    _enemyMover.SetChasing(false);
                    _enemyMover.SetWalking(false);
                    break;
            }
        }

        /// <summary>
        /// 状態から出るときの処理
        /// </summary>
        /// <param name="state">現在の状態</param>
        private void OnExitState(EnemyState state) { }

        /// <summary>
        /// プレイヤーの方向を向く処理
        /// </summary>
        private void LookAtPlayer()
        {
            if (_playerTransform == null)
            {
                return;
            }

            Vector3 direction = (_playerTransform.position - transform.position).normalized;
            direction = Vector3.ProjectOnPlane(direction, Vector3.up);

            if (direction == Vector3.zero)
            {
                return;
            }

            transform.rotation = Quaternion.LookRotation(direction);
        }

        /// <summary>
        /// プレイヤーが指定した範囲内にいるかどうかを判定する処理
        /// </summary>
        /// <param name="range">判定する範囲</param>
        private bool IsPlayerInRange(float range)
        {
            if (_playerTransform == null)
            {
                return false;
            }

            return Vector3.Distance(transform.position, _playerTransform.position) <= range;
        }
    }
}