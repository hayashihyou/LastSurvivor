namespace LastSurvivor
{
    using Cysharp.Threading.Tasks;
    using R3;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// タイトルシーンを管理するスクリプト
    /// </summary>
    public class TitleSceneScript : MonoBehaviour
    {
        [Header("スタートボタン"), SerializeField]
        private Button _startButton;

        [Header("BGM"), SerializeField]
        private AudioClip _bgm;

        [Header("決定音の効果音"), SerializeField]
        private AudioClip _decisionSound;

        private AudioSource _audioSource;

        /// <summary>
        /// インスタンス化直後に呼び出される初期化処理
        /// </summary>
        private void Start()
        {
            _audioSource = GetComponent<AudioSource>()
                ?? gameObject.AddComponent<AudioSource>();

            // BGMを再生
            if (_bgm != null)
            {
                _audioSource.clip = _bgm;
                _audioSource.loop = true;
                _audioSource.Play();
            }

            // スタートボタンのクリックイベントを購読
            _startButton.onClick.AsObservable()
                .Subscribe(_ => OnStartButtonClickedTask())
                .AddTo(this);
        }

        /// <summary>
        /// スタートボタンがクリックされたときの処理
        /// </summary>
        private void OnStartButtonClickedTask()
        {
            GoToInGameTask().Forget();
        }

        /// <summary>
        /// インゲームシーンに遷移する処理
        /// </summary>
        private async UniTask GoToInGameTask()
        {
            // スタートボタンを非活性化
            _startButton.interactable = false;

            if(_decisionSound != null)
            {
                _audioSource.PlayOneShot(_decisionSound);
            }

            await UniTask.Delay(
                (int)(_decisionSound != null ? _decisionSound.length * 1000 : 0),
                cancellationToken: this.GetCancellationTokenOnDestroy());

            _audioSource.Stop();

            // インゲームシーンに遷移
            await SceneLoader.Instance.LoadSceneTask(SceneNameConstants.InGame, this.GetCancellationTokenOnDestroy());
        }
    }
}