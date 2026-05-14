namespace LastSurvivor
{
    using Cysharp.Threading.Tasks;
    using System.Threading;
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
        /// シーンを非同期で遷移する処理
        /// </summary>
        /// <param name="sceneName"> 遷移先のシーン名 </param>
        public async UniTask LoadSceneTask(string sceneName, CancellationToken ct = default)
        {
            await FadeOutTask(ct);

            await SceneManager.LoadSceneAsync(sceneName).WithCancellation(ct);

            await FadeInTask(ct);
        }

        /// <summary>
        /// フェードアウト処理
        /// </summary>
        private async UniTask FadeOutTask(CancellationToken ct)
        {
            // 0.5秒待機
            await UniTask.Delay(500, cancellationToken: ct);
        }

        /// <summary>
        /// フェードイン処理
        /// </summary>
        private async UniTask FadeInTask(CancellationToken ct)
        {
            // 0.5秒待機
            await UniTask.Delay(500, cancellationToken: ct);
        }
    }
}