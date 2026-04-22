namespace LastSurvivor
{
    using Cysharp.Threading.Tasks;
    using R3;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// ?��C?��?��?��Q?��[?��?��?��V?��[?��?��?��?��?��Ǘ�?��?��?��?��X?��N?��?��?��v?��g
    /// </summary>
    public class InGameScene : MonoBehaviour
    {
        [Header("?��?��?��U?��?��?��g?��{?��^?��?��"), SerializeField]
        private Button _resultButton;

        // ?��X?��R?��A?��Ǘ�?��?��?��?��ReactiveProperty
        public ReactiveProperty<int> Score = new ReactiveProperty<int>(0);

        // ?��?��?��U?��?��?��g?��?��Ԃ�?��Ǘ�?��?��?��?��ReactiveProperty
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
        /// ?��X?��R?��A?��̉�?��Z
        /// </summary>
        /// <param name="amount"> ?��?��?��Z?��?��?��?��X?��R?��A?��̗� </param>
        public void AddScoreTask(int amount)
        {
            Score.Value += amount;
        }

        /// <summary>
        /// ?��?��?��U?��?��?��g?��V?��[?��?��?��ɑJ?��ڂ�?��邽?��߂̃t?��?��?��O?��?��Ă鏈?��?��
        /// </summary>
        private void ResultTask()
        {
            IsResult.Value = true;
        }

        /// <summary>
        /// ?��?��?��U?��?��?��g?��V?��[?��?��?��ɑJ?��ڂ�?��鏈�?��
        /// </summary>
        private async UniTask GoToResultTask()
        {
            // ?��X?��R?��A?��?��ۑ�?��?��?��Ă�?��?��
            PlayerPrefs.SetInt("Score", Score.Value);

            // PlayerPrefs?��̕ύX?��?��ۑ�
            PlayerPrefs.Save();

            // ?��?��?��U?��?��?��g?��V?��[?��?��?��ɑJ?��?��
            await SceneLoader.Instance.LoadSceneAsyncTask(SceneNameConstants.Result);
        }
    }
}


