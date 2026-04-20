namespace LastSurvivor
{

    using Cysharp.Threading.Tasks;
    using R3;
    using UnityEngine;
    using UnityEngine.UI;


    public class InGameSceneScript : MonoBehaviour
    {
        [SerializeField] private Button gameOverButton;

        public ReactiveProperty<int> Score = new ReactiveProperty<int>(0);
        public ReactiveProperty<bool> IsGameOver = new ReactiveProperty<bool>(false);


        void Start()
        {
            gameOverButton.onClick.AsObservable()
                .Subscribe(_ => GameOver())
                .AddTo(this);


            IsGameOver
                .Where(isGameOver => isGameOver)
                .Subscribe(_ => GoToResult().Forget())
                .AddTo(this);
        }


        public void AddScore(int amount)
        {
            Score.Value += amount;
        }


        private void GameOver()
        {
            IsGameOver.Value = true;
        }


        private async UniTask GoToResult()
        {
            PlayerPrefs.SetInt("Score", Score.Value); // スコアを保存しておく（例）)
            PlayerPrefs.Save(); // PlayerPrefsの変更を保存


            await SceneLoaderScript.Instance.LoadSceneAsync("Result");
        }
    }
}


