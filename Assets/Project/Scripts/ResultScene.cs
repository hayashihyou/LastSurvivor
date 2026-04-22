namespace LastSurvivor
{
    using Cysharp.Threading.Tasks;
    using R3;
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;
    using TMPro;

    /// <summary>
    /// リザルト画面を管理するスクリプト
    /// </summary>
    public class ResultScene : MonoBehaviour
    {
        [Header("リトライボタン"),SerializeField] 
        private Button _retryButton;

        [Header("タイトルボタン"),SerializeField] 
        private Button _titleButton;

        [Header("スコアテキスト"),SerializeField] 
        private TextMeshProUGUI _scoreText;

        /// <summary>
        /// シーン開始時の処理
        /// </summary>
        void Start()
        {
            // スコアを取得
            var score = PlayerPrefs.GetInt("Score", 0);
            _scoreText.text = $"Score: {score}";

            //DOTweenを使ってスコアテキストをアニメーション表示
            _scoreText.transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack);

            // リトライボタンのクリックイベント
            _retryButton.onClick.AsObservable()
                .Subscribe(_ => OnRetryClickedTask().Forget())
                .AddTo(this);

            // タイトルボタンのクリックイベント
            _titleButton.onClick.AsObservable()
                .Subscribe(_ => OnTitleClickedTask().Forget())
                .AddTo(this);
        }

        /// <summary>
        /// リトライボタンをクリックした時の処理
        /// </summary>
        private async UniTask OnRetryClickedTask()
        {
            // ボタンを無効化して多重クリックを防止
            _retryButton.interactable = false;
            await SceneLoader.Instance.LoadSceneAsyncTask(SceneNameConstants.InGame);
        }

        /// <summary>
        /// タイトルボタンをクリックしたときの処理
        /// </summary>
        private async UniTask OnTitleClickedTask()
        {
            // ボタンを無効化して多重クリックを防止
            _titleButton.interactable = false;

            // タイトルシーンに遷移
            await SceneLoader.Instance.LoadSceneAsyncTask(SceneNameConstants.Title);
        }
    }
}