namespace LastSurvivor
{
    using Cysharp.Threading.Tasks;
    using R3;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    ///　インゲームシーンを管理するクラス
    /// </summary>
    public class InGameScene : MonoBehaviour
    {
        [Header("リザルトボタン"), SerializeField]
        private Button _resultButton;

        // スコアを管理するReactiveProperty
        public ReactiveProperty<int> Score = new ReactiveProperty<int>(0);

        // リザルト画面に遷移するかどうかを管理するReactiveProperty
        public ReactiveProperty<bool> IsResult = new ReactiveProperty<bool>(false);

        /// <summary>
        /// インスタンス化直後に呼び出される初期化処理
        /// </summary>
        void Start()
        {
            _resultButton.onClick.AsObservable()
                .Subscribe(_ => ResultTask())
                .AddTo(this);

            IsResult
                .Where(isResult => isResult)
                .Subscribe(_ => GoToResultTask().Forget())
                .AddTo(this);
        }

        /// <summary>
        /// スコアを加算する処理
        /// </summary>
        /// <param name="amount">加算するスコアの量</param>
        public void AddScoreTask(int amount)
        {
            Score.Value += amount;
        }

        /// <summary>
        /// リザルト画面に遷移するフラグを立てる処理
        /// </summary>
        private void ResultTask()
        {
            IsResult.Value = true;
        }

        /// <summary>
        /// リザルト画面に遷移する処理
        /// </summary>
        private async UniTask GoToResultTask()
        {
            // スコアをPlayerPrefsに保存
            PlayerPrefs.SetInt("Score", Score.Value);

            // PlayerPrefsを保存
            PlayerPrefs.Save();

            // リザルト画面に遷移
            await SceneLoader.Instance.LoadSceneAsyncTask(SceneNameConstants.Result);
        }
    }
}


