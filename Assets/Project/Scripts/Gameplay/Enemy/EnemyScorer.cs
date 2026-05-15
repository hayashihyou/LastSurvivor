namespace LastSurvivor
{
    using UnityEngine;
    using R3;

    public class EnemyScorer : MonoBehaviour
    {
        [Header("撃破スコア"), SerializeField]
        private int _score = 100;

        [Header("敵のステータス"), SerializeField]
        private EnemyStatus _enemyStatus;

        private InGameScene _inGameScene;

        /// <summary>
        /// インスタンス直後に呼び出される初期化メソッド
        /// </summary>
        private void Awake()
        {
            _inGameScene = FindObjectOfType<InGameScene>();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Start()
        {
            _enemyStatus.IsDead
                 .Where(isDead => isDead)
                 .Take(1)
                 .Subscribe(_ => _inGameScene.AddScore(_score))
                 .AddTo(this);
        }
    } 
}