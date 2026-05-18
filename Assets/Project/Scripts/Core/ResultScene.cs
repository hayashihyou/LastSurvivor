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

        [Header("リザルトテキスト"), SerializeField]
        private TextMeshProUGUI _resultText;

        [Header("BGM"), SerializeField]
        private AudioClip _bgmClip;

        [Header("決定時の効果音"), SerializeField]
        private AudioClip _decisionSound;

        private AudioSource _audioSource;

        /// <summary>
        /// インスタンス化直後に呼び出される初期化処理
        /// </summary>
        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _audioSource = gameObject.AddComponent<AudioSource>()
                ?? gameObject.AddComponent<AudioSource>();

            if(_bgmClip != null)
            {
                _audioSource.clip = _bgmClip;
                _audioSource.loop = true;
                _audioSource.Play();
            }

            // GameManagerから結果を受け取る
            bool survived = GameManager.PlayerSurvived;
            _resultText.text = survived ? "CLEAR" : "GAME OVER";

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

            if(_decisionSound != null)
            {
                _audioSource.PlayOneShot(_decisionSound);
                await UniTask.Delay(
                    (int)(_decisionSound.length * 1000),
                    cancellationToken: this.GetCancellationTokenOnDestroy()
                    );
            }

            _audioSource.Stop();
            await SceneLoader.Instance.LoadSceneTask(SceneNameConstants.InGame);
        }

        /// <summary>
        /// タイトルボタンがクリックされたときの処理
        /// </summary>
        private async UniTask OnTitleClickedTask()
        {
            // タイトルボタンを非活性化
            _titleButton.interactable = false;

            if(_decisionSound != null)
            {
                _audioSource.PlayOneShot(_decisionSound);
                await UniTask.Delay(
                    (int)(_decisionSound.length * 1000),
                    cancellationToken: this.GetCancellationTokenOnDestroy()
                    );
            }

            // タイトルシーンに遷移
            await SceneLoader.Instance.LoadSceneTask(SceneNameConstants.Title);
        }
    }
}