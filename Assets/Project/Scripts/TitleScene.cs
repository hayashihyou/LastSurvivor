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
        [Header("スタートボタン"),SerializeField] 
        private Button startButton;

        /// <summary>
        /// シーン開始時の処理
        /// </summary>
        void Start()
        {
            // スタートボタンのクリックイベント
            startButton.onClick.AsObservable()
                .Subscribe(_ => OnStartButtonClicked())
                .AddTo(this);
        }

        /// <summary>
        /// スタートボタンをクリックしたときの処理
        /// </summary>
        private void OnStartButtonClicked()
        {
            GoToInGame().Forget();
        }

        /// <summary>
        /// インゲームシーンに遷移するための処理
        /// </summary>
        private async UniTask GoToInGame()
        {
            // ボタンを無効化して多重クリックを防止
            startButton.interactable = false;

            // インゲームシーンに遷移
            await SceneLoader.Instance.LoadSceneAsync(SceneNameConstants.InGame);
        }
    }
}