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
        private Button _startButton;

        /// <summary>
        /// シーン開始時の処理
        /// </summary>
        void Start()
        {
            // スタートボタンのクリックイベント
            _startButton.onClick.AsObservable()
                .Subscribe(_ => OnStartButtonClickedTask())
                .AddTo(this);
        }

        /// <summary>
        /// スタートボタンをクリックしたときの処理
        /// </summary>
        private void OnStartButtonClickedTask()
        {
            GoToInGameTask().Forget();
        }

        /// <summary>
        /// インゲームシーンに遷移するための処理
        /// </summary>
        private async UniTask GoToInGameTask()
        {
            // ボタンを無効化して多重クリックを防止
            _startButton.interactable = false;

            // インゲームシーンに遷移
            await SceneLoader.Instance.LoadSceneAsyncTask(SceneNameConstants.InGame);
        }
    }
}