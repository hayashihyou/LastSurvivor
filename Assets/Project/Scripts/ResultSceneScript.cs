namespace LastSurvivor
{

    using Cysharp.Threading.Tasks;
    using R3;
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;
    using TMPro;


    public class ResultSceneScript : MonoBehaviour
    {
        [SerializeField] private Button retryButton;
        [SerializeField] private Button titleButton;
        [SerializeField] private TextMeshProUGUI scoreText;


        void Start()
        {
            int score = PlayerPrefs.GetInt("Score", 0); // スコアを取得（例）
            scoreText.text = $"Score: {score}";


            //DOTweenを使ってスコアテキストをアニメーション表示
            scoreText.transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack);


            // リトライボタンのクリックイベント
            retryButton.onClick.AsObservable()
                .Subscribe(_ => OnRetryClicked().Forget())
                .AddTo(this);

            // タイトルボタンのクリックイベント
            titleButton.onClick.AsObservable()
                .Subscribe(_ => OnTitleClicked().Forget())
                .AddTo(this);
        }


        private async UniTask OnRetryClicked()
        {
            retryButton.interactable = false; // ボタンを無効化して多重クリックを防止
            await SceneLoaderScript.Instance.LoadSceneAsync("InGame");
        }


        private async UniTask OnTitleClicked()
        {
            Debug.Log("タイトルに戻るボタンがクリックされました"); // デバッグログを追加

            titleButton.interactable = false; // ボタンを無効化して多重クリックを防止
            await SceneLoaderScript.Instance.LoadSceneAsync("Title");
        }
    }
}