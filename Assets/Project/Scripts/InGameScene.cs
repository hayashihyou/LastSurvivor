namespace LastSurvivor
{
    using Cysharp.Threading.Tasks;
    using R3;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// インゲームシーンを管理するスクリプト
    /// </summary>
    public class InGameScene : MonoBehaviour
    {
        [Header("リザルトボタン"),SerializeField] 
        private Button resultButton;

        // スコア管理するReactiveProperty
        public ReactiveProperty<int> Score = new ReactiveProperty<int>(0);

        // リザルト状態を管理するReactiveProperty
        public ReactiveProperty<bool> IsResult = new ReactiveProperty<bool>(false);

        /// <summary>
        /// シーン開始時の処理
        /// </summary>
        void Start()
        {
            resultButton.onClick.AsObservable()
                .Subscribe(_ => Result())
                .AddTo(this);

            IsResult
                .Where(isResult => isResult)
                .Subscribe(_ => GoToResult().Forget())
                .AddTo(this);
        }

        /// <summary>
        /// スコアの加算
        /// </summary>
        /// <param name="amount"> 加算するスコアの量 </param>
        public void AddScore(int amount)
        {
            Score.Value += amount;
        }

        /// <summary>
        /// リザルトシーンに遷移するためのフラグを立てる処理
        /// </summary>
        private void Result()
        {
            IsResult.Value = true;
        }

        /// <summary>
        /// リザルトシーンに遷移する処理
        /// </summary>
        private async UniTask GoToResult()
        {
            // スコアを保存しておく
            PlayerPrefs.SetInt("Score", Score.Value); 

            // PlayerPrefsの変更を保存
            PlayerPrefs.Save();

            // リザルトシーンに遷移
            await SceneLoader.Instance.LoadSceneAsync(SceneNameConstants.Result);
        }
    }
}


