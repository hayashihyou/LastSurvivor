namespace LastSurvivor
{
    using Cysharp.Threading.Tasks;
    using R3;
    using R3.Triggers;
    using System;
    using System.Threading;
    using UnityEngine;

    /// <summary>
    /// エネミーのスポーンを管理するクラス
    /// </summary>
    public class SpawnManager : MonoBehaviour
    {
        [Header("スポーン設定"),SerializeField]
        private GameObject _enemyPrefab;

        [Header("スポーンポイント"),SerializeField]
        private Transform[] _spawnPoints;

        [Header("スポーン間隔"),SerializeField]
        private float _spawnInterval = 3f;

        [Header("エネミー上限"),SerializeField]
        private int _maxEnemyCount = 10;

        // 現在のエネミー数（外部から購読可能）
        public ReadOnlyReactiveProperty<int> EnemyCount => _enemyCount;
        private readonly ReactiveProperty<int> _enemyCount = new(0);

        // スポーンイベント（外部からエフェクト等に購読可能）
        public Observable<Vector3> OnEnemySpawned => _onEnemySpawned;
        private readonly Subject<Vector3> _onEnemySpawned = new();

        private CancellationTokenSource _cts;

        /// <summary>
        /// スポーンループを開始
        /// </summary>
        private void Start()
        {
            _cts = new CancellationTokenSource();
            SpawnLoopTask(_cts.Token).Forget();
        }

        /// <summary>
        /// 一定間隔でエネミーをスポーンするループ
        /// </summary>
        /// <param name="ct">キャンセルトークン</param>d
        private async UniTaskVoid SpawnLoopTask(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await UniTask.Delay(
                    TimeSpan.FromSeconds(_spawnInterval),
                    cancellationToken: ct
                );

                // 上限に達している場合はスキップ
                if (_enemyCount.Value >= _maxEnemyCount)
                {
                    continue;
                }

                SpawnEnemy();
            }
        }

        /// <summary>
        /// エネミーをスポーンする
        /// </summary>
        private void SpawnEnemy()
        {
            if (_spawnPoints.Length == 0)
            {
                return;
            }

            int index = UnityEngine.Random.Range(0, _spawnPoints.Length);
            Transform spawnPoint = _spawnPoints[index];

            var enemyObj = Instantiate(_enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            _enemyCount.Value++;
            _onEnemySpawned.OnNext(spawnPoint.position);

            // EnemyDeadがDestroyするタイミングでカウントを減算
            enemyObj
                .OnDestroyAsObservable()
                .Subscribe(_ => _enemyCount.Value--)
                .AddTo(this);
        }

        /// <summary>
        /// スポーンループを停止し、リソースを解放
        /// </summary>
        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _onEnemySpawned.Dispose();
            _enemyCount.Dispose();
        }
    }
}