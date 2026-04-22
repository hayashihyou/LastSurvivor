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

        /// <summary>
        /// インスタンス化直後に呼び出される初期化処理
        /// </summary>
        void Start()
        {
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

            // インゲームシーンに遷移
            await SceneLoader.Instance.LoadSceneAsyncTask(SceneNameConstants.InGame);
        }
    }
}