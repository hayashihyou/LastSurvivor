namespace LastSurvivor
{
    using Cysharp.Threading.Tasks;
    using R3;
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;
    using TMPro;

    /// <summary>
    /// リザルトシーン管理
    /// </summary>
    public class ResultScene : MonoBehaviour
    {
        [Header(" リトライボタン "), SerializeField]
        private Button _retryButton;

        [Header("タイトルボタン"), SerializeField]
        private Button _titleButton;

        [Header("スコアテキスト"), SerializeField]
        private TextMeshProUGUI _scoreText;

        /// <summary>
        /// インスタンス化直後に呼び出される初期化処理
        /// </summary>
        private void Start()
        {
            // スコアの取得と表示
            var score = PlayerPrefs.GetInt("Score", 0);
            _scoreText.text = $"Score: {score}";

            // スコアテキストのアニメーション
            _scoreText.transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack);

            // リトライボタンのクリックイベントを購読
            _retryButton.onClick.AsObservable()
                .Subscribe(_ => OnRetryClickedTask().Forget())
                .AddTo(this);

            // タイトルボタンのクリックイベントを購読
            _titleButton.onClick.AsObservable()
                .Subscribe(_ => OnTitleClickedTask().Forget())
                .AddTo(this);
        }

        /// <summary>
        /// リトライボタンがクリックされたときの処理
        /// </summary>
        private async UniTask OnRetryClickedTask()
        {
            // リトライボタンを非活性化
            _retryButton.interactable = false;
            await SceneLoader.Instance.LoadSceneAsyncTask(SceneNameConstants.InGame);
        }

        /// <summary>
        /// タイトルボタンがクリックされたときの処理
        /// </summary>
        private async UniTask OnTitleClickedTask()
        {
            // タイトルボタンを非活性化
            _titleButton.interactable = false;

            // タイトルシーンに遷移
            await SceneLoader.Instance.LoadSceneAsyncTask(SceneNameConstants.Title);
        }
    }
}