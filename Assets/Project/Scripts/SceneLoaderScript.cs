namespace LastSurvivor
{

    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.SceneManagement;


    public class SceneLoaderScript : MonoBehaviour
    {
        public static SceneLoaderScript Instance { get; private set; }


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


        public async UniTask LoadSceneAsync(string sceneName)
        {
            await FadeOut();

            await SceneManager.LoadSceneAsync(sceneName);

            await FadeIn();
        }


        private async UniTask FadeOut()
        {
            // フェードアウトの実装
            await UniTask.Delay(500); // 例: 0.5秒待機
        }

        private async UniTask FadeIn()
        {
            // フェードインの実装
            await UniTask.Delay(500); // 例: 0.5秒待機
        }
    }
}