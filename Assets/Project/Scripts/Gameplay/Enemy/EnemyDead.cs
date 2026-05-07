namespace LastSurvivor
{
    using R3;
    using UnityEngine;
    using UnityEngine.AI;

    /// <summary>
    /// 敵の死亡を管理するクラス
    /// </summary>
    public class EnemyDead : MonoBehaviour
    {
        [Header("死亡後の削除までの時間"),SerializeField]
        private float _destroyTime = 3f;

        [Header("死亡エフェクト"), SerializeField]
        private GameObject _deadEffectPrefab;

        private EnemyStatus _enemyStatus;
        private Collider _collider;
        private NavMeshAgent _navMeshAgent;

        /// <summary>
        /// ゲーム開始時に呼び出される初期化メソッド
        /// </summary>
        void Start()
        {
            // エネミーのステータス、移動、攻撃を管理するクラスへの参照を取得
            _enemyStatus = GetComponent<EnemyStatus>();
            _collider = GetComponent<Collider>();
            _navMeshAgent = GetComponent<NavMeshAgent>();

            // エネミーの死亡状態を監視し、死亡した場合の処理を実行
            _enemyStatus.IsDead
                .Where(isDead => isDead)
                .Subscribe(_ => HandleDead())
                .AddTo(this);
        }

        /// <summary>
        /// エネミーが死亡した際の処理を行うメソッド
        /// </summary>
        void HandleDead()
        {
            DisableComponents();
            PlayDeadEffect();

            // 一定時間後にエネミーを削除
            Observable
                .Timer(System.TimeSpan.FromSeconds(_destroyTime))
                .Subscribe(_ => Destroy(gameObject))
                .AddTo(this);
        }

        /// <summary>
        /// エネミーの死亡後に無効化するコンポーネントを管理するメソッド
        /// </summary>
        void DisableComponents()
        {
            // コライダーとNavMeshAgentを無効化して、エネミーが物理的に存在しないようにする
            if (_collider != null)
            {
                _collider.enabled = false;
            }

            // NavMeshAgentを停止して無効化
            if (_navMeshAgent != null)
            {
                _navMeshAgent.isStopped = true;
                _navMeshAgent.enabled = false;
            }
        }

        /// <summary>
        /// エネミーの死亡エフェクトを再生するメソッド
        /// </summary>
        void PlayDeadEffect()
        {
           if(_deadEffectPrefab == null)
            {
                return;
            }

            // 死亡エフェクトをインスタンス化
            Instantiate(_deadEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}
