namespace LastSurvivor
{

    using Cysharp.Threading.Tasks;
    using R3;
    using UnityEngine;
    using UnityEngine.UI;


    public class TitleSceneScript : MonoBehaviour
    {
        [SerializeField] private Button startButton;


        void Start()
        {
            // スタートボタンのクリックイベント
            startButton.onClick.AsObservable()
                .Subscribe(_ => OnStartButtonClicked())
                .AddTo(this);
        }


        private void OnStartButtonClicked()
        {
            GoToInGame().Forget();
        }


        private async UniTask GoToInGame()
        {
            startButton.interactable = false; // ボタンを無効化して多重クリックを防止


            await SceneLoaderScript.Instance.LoadSceneAsync("InGame");
        }
    }
}