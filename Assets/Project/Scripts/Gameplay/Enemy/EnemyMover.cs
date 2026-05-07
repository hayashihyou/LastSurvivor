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

        // 敵のステータスを管理するクラスへの参照
        private EnemyStatus _enemyStatus;

        // プレイヤーの位置を追跡するためのTransformへの参照
        private Transform _playerTransform;

        // 重力の影響を受ける速度ベクトル
        private Vector3 _velocity;

        // 地面に接地しているかどうかを管理するフラグ
        private bool _isGrounded;

        // 敵が移動中かどうかを管理するReactiveProperty
        public ReactiveProperty<bool> IsMoving { get; private set; }
        public ReactiveProperty<bool> IsChasing { get; private set; }

        /// <summary>
        /// インスタンス直後に呼び出される初期化メソッド
        /// </summary>
        void Awake()
        {
            // ReactivePropertyの初期化
            IsMoving = new ReactiveProperty<bool>(false);
            IsChasing = new ReactiveProperty<bool>(false);
        }

        /// <summary>
        /// ゲーム開始時に呼び出される初期化メソッド
        /// </summary>
        private void Start()
        {
            // EnemyStatusコンポーネントへの参照を取得
            _enemyStatus = GetComponent<EnemyStatus>();

            // ナビメッシュエージェントの速度と停止距離をEnemyStatusから設定
            _navMeshAgent.speed = _enemyStatus.MoveSpeed;
            _navMeshAgent.stoppingDistance = _enemyStatus.AttackRange;

            // プレイヤーオブジェクトをタグで検索し、そのTransformへの参照を取得
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if(playerObj != null)
            {
                _playerTransform = playerObj.transform;
            }
        }

        /// <summary>
        /// 毎フレーム呼び出される更新メソッド
        /// </summary>
        void Update()
        {
            // 敵が死亡している場合は移動を停止
            if (_enemyStatus.IsDead.Value)
            {
                _navMeshAgent.isStopped = true;
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
        /// プレイヤーに向かって移動するタスク
        /// </summary>
        private void HandleMovementTask()
        {
            // プレイヤーのTransformが取得できていない場合や、索敵範囲外の場合は移動を停止
            if (_playerTransform == null || !IsChasing.Value)
            {
                _navMeshAgent.isStopped = true;
                IsMoving.Value = false;
                return;
            }

            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(_playerTransform.position);

            // プレイヤーとの距離を計算し、攻撃範囲内にいるかどうかを判断して移動状態を更新
            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
            IsMoving.Value = distanceToPlayer > _enemyStatus.AttackRange;

            HandleRotationTask();
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

        private void ApplyGravity()
        {
            if(_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            _velocity.y += _gravity * Time.deltaTime;

            if(!_navMeshAgent.isActiveAndEnabled)
            {
                transform.position += _velocity * Time.deltaTime;
            }
        }
    }
}
