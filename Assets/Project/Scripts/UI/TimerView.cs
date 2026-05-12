namespace LastSurvivor
{
    using UnityEngine;
    using TMPro;

    /// <summary>
    /// ゲームタイマーの時間を表示するクラス
    /// </summary>
    public class TimerView : MonoBehaviour
    {
        [Header("タイマー"), SerializeField]
        private GameTimer _gameTimer;

        [Header("タイマーテキスト"),SerializeField]
        private TextMeshProUGUI _timerText;

        /// <summary>
        /// ゲームタイマーの時間が変化したときに、テキストを更新するリスナーを追加
        /// </summary>
        private void Start()
        {
            _gameTimer.OnTimeChanged.AddListener(UpdateTimerText);
        }

        /// <summary>
        /// ゲームタイマーの時間が変化したときに、テキストを更新するためのメソッド
        /// </summary>
        /// <param name="time">現在のゲームタイマーの時間</param>
        private void UpdateTimerText(float time)
        {
            _timerText.text = Mathf.CeilToInt(time).ToString();
        }
    }
}
