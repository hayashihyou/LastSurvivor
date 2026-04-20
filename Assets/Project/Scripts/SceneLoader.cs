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
        /// インスタンス
        /// </summary>
        public static SceneLoader Instance { get; private set; }

        /// <summary>
        /// インスタンスの初期化
        /// </summary>
        private void Awake()
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
        public async UniTask LoadSceneAsync(string sceneName)
        {
            await FadeOut();

            await SceneManager.LoadSceneAsync(sceneName);

            await FadeIn();
        }

        /// <summary>
        /// フェードアウトを実行
        /// </summary>
        private async UniTask FadeOut()
        {
            // 0.5秒待機
            await UniTask.Delay(500); 
        }

        /// <summary>
        /// フェードインを実行
        /// </summary>
        private async UniTask FadeIn()
        {
            // 0.5秒待機
            await UniTask.Delay(500); 
        }
    }
}