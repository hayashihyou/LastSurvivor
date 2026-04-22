namespace LastSurvivor
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// シーンのロードを管理するスクリプト
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        /// <summary>
        /// シングルトンインスタンス
        /// </summary>
        public static SceneLoader Instance { get; private set; }

        /// <summary>
        /// シングルトンの初期化 
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
        /// 指定されたシーンを非同期ロード
        /// </summary>
        /// <param name="sceneName"> シーン名 </param>
        public async UniTask LoadSceneAsyncTask(string sceneName)
        {
            await FadeOutTask();

            await SceneManager.LoadSceneAsync(sceneName);

            await FadeInTask();
        }

        /// <summary>
        /// フェードアウトを実行
        /// </summary>
        private async UniTask FadeOutTask()
        {
            // 0.5秒待機
            await UniTask.Delay(500); 
        }

        /// <summary>
        /// フェードインを実行
        /// </summary>
        private async UniTask FadeInTask()
        {
            // 0.5秒待機
            await UniTask.Delay(500); 
        }
    }
}