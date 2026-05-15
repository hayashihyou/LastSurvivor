namespace LastSurvivor
{
    using R3;
    using UnityEngine;
    using UnityEngine.AI;

    /// <summary>
    /// 敵の移動を管理するクラス
    /// </summary>
    public class EnemyMover : MonoBehaviour
    {
        [Header("ナビメッシュ"), SerializeField]
        private NavMeshAgent _navMeshAgent;

        [Header("重力"), SerializeField]
        private float _gravity = -9.81f;

        [Header("地面チェック"), SerializeField]
        private Transform _groundCheck;
        private float _groundDistance = 0.3f;

        [Header("地面マスク"), SerializeField]
        private LayerMask _groundMask;

        [Header("索敵範囲"), SerializeField]
        private float _detectionRange = 10f;

        [Header("敵のステータス"), SerializeField]
        private EnemyStatus _enemyStatus;

        // 追跡速度
        private float _chaseSpeed;

        // 徘徊速度
        private float _walkSpeed;

        // プレイヤーの位置を追跡するためのTransformへの参照
        private Transform _playerTransform;

        // 重力の影響を受ける速度ベクトル
        private Vector3 _velocity;

        // 地面に接地しているかどうかを管理するフラグ
        private bool _isGrounded;

        // 敵が移動中かどうかを管理するReactiveProperty
        public ReactiveProperty<bool> IsWalking { get; private set; }
        public ReactiveProperty<bool> IsChasing { get; private set; }

        public void SetWalkSpeed()
        {
            _navMeshAgent.speed = _walkSpeed;
        }

        public void SetChaseSpeed()
        {
            _navMeshAgent.speed = _chaseSpeed;
        }

        public void StopMovement()
        {
            if (_navMeshAgent.isActiveAndEnabled)
            {
                _navMeshAgent.isStopped = true;
            }
        }

        /// <summary>
        /// インスタンス直後に呼び出される初期化メソッド
        /// </summary>
        private void Awake()
        {
            // ReactivePropertyの初期化
            IsWalking = new ReactiveProperty<bool>(false);
            IsChasing = new ReactiveProperty<bool>(false);
        }

        /// <summary>
        /// ゲーム開始時に呼び出される初期化メソッド
        /// </summary>
        private void Start()
        {
            _walkSpeed = _enemyStatus.MoveSpeed * 0.5f;
            _chaseSpeed = _enemyStatus.MoveSpeed;

            // ナビメッシュエージェントの速度と停止距離をEnemyStatusから設定
            _navMeshAgent.speed = _walkSpeed;
            _navMeshAgent.stoppingDistance = _enemyStatus.AttackRange;

            // プレイヤーオブジェクトをタグで検索し、そのTransformへの参照を取得
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _playerTransform = playerObj.transform;
            }
        }

        /// <summary>
        /// 毎フレーム呼び出される更新メソッド
        /// </summary>
        private void Update()
        {
            // 敵が死亡している場合は移動を停止
            if (_enemyStatus.IsDead.Value)
            {
                if (_navMeshAgent.isActiveAndEnabled)
                {
                    _navMeshAgent.isStopped = true;
                }

                return;
            }

            CheckGroundTask();
            HandleMovementTask();
            ApplyGravity();
        }

        /// <summary>
        /// 地面に接地しているかどうかをチェックするタスク
        /// </summary>
        private void CheckGroundTask()
        {
            // 地面に接地しているかどうかをチェックするために、Physics.CheckSphereを使用
            _isGrounded = Physics.CheckSphere(
                _groundCheck.position,
                _groundDistance,
                _groundMask
            );

            // 地面に接地している場合、垂直速度をリセットして安定させる
            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }
        }

        /// <summary>
        /// プレイヤーを追跡するかどうかのフラグを設定するメソッド
        /// </summary>
        /// <param name="isChasing">プレイヤーを追跡するかどうかのフラグ</param>
        public void SetChasing(bool isChasing)
        {
            IsChasing.Value = isChasing;
        }

        /// <summary>
        /// 敵が移動中かどうかのフラグを設定するメソッド
        /// </summary>
        /// <param name="isWalking">敵が移動中かどうかのフラグ</param>
        public void SetWalking(bool isWalking)
        {
            IsWalking.Value = isWalking;
        }

        /// <summary>
        /// プレイヤーに向かって移動するタスク
        /// </summary>
        private void HandleMovementTask()
        {
            if (!_navMeshAgent.isActiveAndEnabled)
            {
                return;
            }

            // プレイヤーのTransformが取得できていない場合や、索敵範囲外の場合は移動を停止
            if (_playerTransform == null || (!IsChasing.Value && !IsWalking.Value))
            {
                _navMeshAgent.isStopped = true;
                return;
            }

            _navMeshAgent.isStopped = false;

            if (IsChasing.Value)
            {
                _navMeshAgent.SetDestination(_playerTransform.position);
                HandleRotationTask();
            }
        }

        /// <summary>
        /// プレイヤーの方向に向かって回転するタスク
        /// </summary>
        private void HandleRotationTask()
        {
            // プレイヤーのTransformが取得できていない場合は、回転を行わない
            Vector3 direction = (_playerTransform.position - transform.position).normalized;
            direction.y = 0;

            // プレイヤーの方向がゼロベクトルの場合は、回転を行わない
            if (direction == Vector3.zero)
            {
                return;
            }

            // プレイヤーの方向に向かって回転するためのQuaternionを計算し、スムーズに回転させる
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * _navMeshAgent.angularSpeed / 100f
            );
        }

        /// <summary>
        /// 重力を適用するタスク
        /// </summary>
        private void ApplyGravity()
        {
            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            _velocity.y += _gravity * Time.deltaTime;

            if (!_navMeshAgent.isActiveAndEnabled)
            {
                transform.position += _velocity * Time.deltaTime;
            }
        }

        /// <summary>
        /// 敵が目的地に到達したかどうかを判断するメソッド
        /// </summary>
        public bool HasReachedDestination()
        {
            // NavMeshAgentが有効でない場合は到達済みとして扱う
            if (!_navMeshAgent.isActiveAndEnabled)
            {
                return true;
            }

            // パスの計算中は未到達
            if (_navMeshAgent.pathPending)
            {
                return false;
            }

            // 残り距離が停止距離以下になったら到達
            return _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance;
        }

        /// <summary>
        /// ランダムな目的地を設定するメソッド
        /// </summary>
        public bool SetRandomDestination()
        {
            Vector3 randomDirection = Random.insideUnitSphere * _detectionRange;
            randomDirection += transform.position;

            if(NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _detectionRange, NavMesh.AllAreas))
            {
                _navMeshAgent.SetDestination(hit.position);
                return true;
            }

            return false;
        }

        public void SetRotationControl(bool enabled)
        {
            _navMeshAgent.updateRotation = enabled;
        }
    }
}
