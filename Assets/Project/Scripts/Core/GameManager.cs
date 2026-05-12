namespace LastSurvivor
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// ゲーム全体の管理を行うクラス
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("シーン名"),SerializeField]
        private string _resultSceneName = "Result";

        [Header("ゲームタイマー"), SerializeField]
        private GameTimer _gameTimer;

        // シングルトンインスタンス
        public static GameManager Instance { get; private set; }

        // リザルトシーンへ渡すデータ　プレイヤーが生存しているかどうか
        public static bool PlayerSurvived { get; private set; }

        // ゲームオーバーかどうか確認するフラグ
        private bool _isGameOver = false;

        /// <summary>
        /// インスタンス化直後に呼び出される初期化処理
        /// </summary>
        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        /// <summary>
        /// ゲーム開始時に呼び出される処理
        /// </summary>
        private void Start()
        {
            if(_gameTimer != null)
            {
                _gameTimer.OnTimeUp.AddListener(OnTimeUp);
            }

            _gameTimer?.StartTimer();
        }

        /// <summary>
        /// ゲーム終了時に呼び出される処理
        /// </summary>
        private void OnTimeUp()
        {
            if(_isGameOver)
            {
                return;
            }
            GoToResult(survived: true);
        }

        /// <summary>
        /// プレイヤーが死亡したときに呼び出される処理
        /// </summary>
        public void OnPlayerDead()
        {
            if(_isGameOver)
            {
                return;
            }
            GoToResult(survived: false);
        }

        /// <summary>
        /// ゲーム終了後、リザルトシーンへ遷移する処理
        /// </summary>
        /// <param name="survived">プレイヤーが生存しているかどうか</param>
        private void GoToResult(bool survived)
        {
            _isGameOver = true;
            _gameTimer?.StopTimer();

            // リザルト画面へ渡すデータを保存
            PlayerSurvived = survived;

            SceneManager.LoadScene(_resultSceneName);
        }
    }
}
