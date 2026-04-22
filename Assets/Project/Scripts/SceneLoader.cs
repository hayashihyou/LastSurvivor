namespace LastSurvivor
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// シーン遷移管理
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        /// <summary>
        /// シーンローダーのインスタンス
        /// </summary>
        public static SceneLoader Instance { get; private set; }

        /// <summary>
        /// インスタンス化直後に呼び出される初期化処理
        /// </summary>
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// シーンを非同期で遷移する処理
        /// </summary>
        /// <param name="sceneName"> 遷移先のシーン名 </param>
        public async UniTask LoadSceneAsyncTask(string sceneName)
        {
            await FadeOutTask();

            await SceneManager.LoadSceneAsync(sceneName);

            await FadeInTask();
        }

        /// <summary>
        /// フェードアウト処理
        /// </summary>
        private async UniTask FadeOutTask()
        {
            // 0.5秒待機
            await UniTask.Delay(500);
        }

        /// <summary>
        /// フェードイン処理
        /// </summary>
        private async UniTask FadeInTask()
        {
            // 0.5秒待機
            await UniTask.Delay(500);
        }
    }
}