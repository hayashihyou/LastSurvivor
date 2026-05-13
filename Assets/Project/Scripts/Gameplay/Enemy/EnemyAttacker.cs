namespace LastSurvivor
{
    using R3;
    using UnityEngine;

    /// <summary>
    /// 敵の攻撃を管理するクラス
    /// </summary>
    public class EnemyAttacker : MonoBehaviour
    {
        [Header("攻撃クールタイム"), SerializeField]
        private float _attackCooldown = 1.5f;

        // 敵のステータスを管理するクラスへの参照
        private EnemyStatus _enemyStatus;

        // プレイヤーのステータスを管理するクラスへの参照
        private PlayerStatus _playerStatus;

        // 敵が攻撃中かどうかを管理するReactiveProperty
        public ReactiveProperty<bool> IsAttacking { get; private set; }

        // 攻撃のクールタイムを管理するタイマー
        private float _cooldownTimer = 0f;

        /// <summary>
        /// オブジェクトが生成されたときに呼び出される初期化メソッド
        /// </summary>
        private void Awake()
        {
            IsAttacking = new ReactiveProperty<bool>(false);
        }

        /// <summary>
        /// ゲーム開始時に呼び出される初期化メソッド
        /// </summary>
        private void Start()
        {
            // 敵のステータスを管理するクラスへの参照を取得
            _enemyStatus = GetComponent<EnemyStatus>();

            // プレイヤーのステータスを管理するクラスへの参照を取得
            var playerObject = GameObject.FindGameObjectWithTag(TagConsts.Player);
            if(playerObject != null)
            {
                _playerStatus = playerObject.GetComponent<PlayerStatus>();
            }
        }

        /// <summary>
        /// 毎フレーム呼び出される更新メソッド
        /// </summary>
        private void Update()
        {
            // 敵が死亡している場合は攻撃を行わない
            if (_enemyStatus.IsDead.Value)
            {
                return;
            }

            TickCooldown();
        }
        
        /// <summary>
        /// 攻撃を試みるメソッド
        /// </summary>
        public void TryAttack()
        {
            // 攻撃できない条件をチェック
            var cannotAttack = _enemyStatus.IsDead.Value || _playerStatus == null || _playerStatus.IsDead.Value || _cooldownTimer > 0f;

            if(cannotAttack)
            {
                return;
            }

            // プレイヤーが攻撃範囲内にいるかをチェック
            if (!IsInAttackRange())
            {
                IsAttacking.Value = false;
                return;
            }

            ExecuteAttack();
        }

        /// <summary>
        /// プレイヤーが攻撃範囲内にいるかをチェックするメソッド
        /// </summary>
        private bool IsInAttackRange()
        {
            if(_playerStatus == null)
            {
                return false;
            }

            var distance = Vector3.Distance(transform.position, _playerStatus.transform.position);
            return distance <= _enemyStatus.AttackRange;
        }

        /// <summary>
        /// 攻撃のクールタイムを管理するメソッド
        /// </summary>
        private void TickCooldown()
        {
            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
            }

            // クールタイムが終わったら IsAttacking を false に戻す
            if (_cooldownTimer <= 0f)
            {
                IsAttacking.Value = false;
            }
        }

        /// <summary>
        /// 攻撃を実行するメソッド
        /// </summary>
        private void ExecuteAttack()
        {
            _playerStatus.TakeDamage(_enemyStatus.AttackPower);
            _cooldownTimer = _attackCooldown;
            IsAttacking.Value = true;
        }

        /// <summary>
        /// 攻撃状態をリセットするメソッド
        /// </summary>
        public void ResetAttack()
        {
            IsAttacking.Value = false;
            _cooldownTimer = 0f;
        }
    }
}
