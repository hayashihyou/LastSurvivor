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

        [Header("エネミーステータス"),SerializeField]
        private EnemyStatus _enemyStatus;

        [Header("コライダー"),SerializeField]
        private Collider _collider;

        [Header("ナビメッシュ"),SerializeField]
        private NavMeshAgent _navMeshAgent;

        // NOTE: 死亡エフェクトを実装したらコメントアウトを外す
        //[Header("死亡エフェクト"), SerializeField]
        //private GameObject _deadEffectPrefab;

        /// <summary>
        /// ゲーム開始時に呼び出される初期化メソッド
        /// </summary>
        private void Start()
        {
            // エネミーの死亡状態を監視し、死亡した場合の処理を実行
            _enemyStatus.IsDead
                .Where(isDead => isDead)
                .Subscribe(_ => HandleDead())
                .AddTo(this);
        }

        /// <summary>
        /// エネミーが死亡した際の処理を行うメソッド
        /// </summary>
        private void HandleDead()
        {
            DisableComponents();
            //PlayDeadEffect();

            // 一定時間後にエネミーを削除
            Observable
                .Timer(System.TimeSpan.FromSeconds(_destroyTime))
                .Subscribe(_ => Destroy(gameObject))
                .AddTo(this);
        }

        /// <summary>
        /// エネミーの死亡後に無効化するコンポーネントを管理するメソッド
        /// </summary>
        private void DisableComponents()
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

        /// NOTE: 死亡エフェクトを実装したらコメントアウトを外す
        /// <summary>
        /// エネミーの死亡エフェクトを再生するメソッド
        /// </summary>
        //void PlayDeadEffect()
        //{
        //   if(_deadEffectPrefab == null)
        //    {
        //        return;
        //    }

        //    // 死亡エフェクトをインスタンス化
        //    Instantiate(_deadEffectPrefab, transform.position, Quaternion.identity);
        //}
    }
}
