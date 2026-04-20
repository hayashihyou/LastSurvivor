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
        [Header("リトライボタン"),SerializeField] private Button retryButton;
        [Header("タイトルボタン"),SerializeField] private Button titleButton;
        [Header("スコアテキスト"),SerializeField] private TextMeshProUGUI scoreText;

        /// <summary>
        /// シーン開始時の処理
        /// </summary>
        void Start()
        {
            // スコアを取得
            int score = PlayerPrefs.GetInt("Score", 0);
            scoreText.text = $"Score: {score}";

            //DOTweenを使ってスコアテキストをアニメーション表示
            scoreText.transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack);

            // リトライボタンのクリックイベント
            retryButton.onClick.AsObservable()
                .Subscribe(_ => OnRetryClicked().Forget())
                .AddTo(this);

            // タイトルボタンのクリックイベント
            titleButton.onClick.AsObservable()
                .Subscribe(_ => OnTitleClicked().Forget())
                .AddTo(this);
        }

        /// <summary>
        /// リトライボタンをクリックした時の処理
        /// </summary>
        private async UniTask OnRetryClicked()
        {
            // ボタンを無効化して多重クリックを防止
            retryButton.interactable = false;
            await SceneLoader.Instance.LoadSceneAsync(SceneNameConstants.InGame);
        }

        /// <summary>
        /// タイトルボタンをクリックしたときの処理
        /// </summary>
        private async UniTask OnTitleClicked()
        {
            // ボタンを無効化して多重クリックを防止
            titleButton.interactable = false;

            // タイトルシーンに遷移
            await SceneLoader.Instance.LoadSceneAsync(SceneNameConstants.Title);
        }
    }
}