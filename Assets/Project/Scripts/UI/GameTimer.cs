namespace LastSurvivor
{
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// ゲーム内のタイマーを管理するクラス
    /// </summary>
    public class GameTimer : MonoBehaviour
    {
        [Header("タイマー設定"), SerializeField]
        private float _timeLimit = 60f;

        // タイマーの現在の時間を保持する変数
        private float _currentTime;

        // タイマーが動作中かどうかを示すフラグ
        private bool _isRunning = false;

        // タイムアップ時に呼び出されるイベント
        public UnityEvent OnTimeUp = new UnityEvent();

        // タイマーの時間が変化したときに呼び出されるイベント 引数は現在の時間
        public UnityEvent<float> OnTimeChanged = new UnityEvent<float>();

        // タイムリミットを取得するプロパティ
        public float TimeLimit => _timeLimit;

        // 現在の時間を取得するプロパティ
        public float CurrentTime => _currentTime;

        // タイマーの現在の時間をタイムリミットで割った値を取得するプロパティ
        public float NormalizedTime => _currentTime / _timeLimit;

        /// <summary>
        /// ゲーム開始時に呼び出されるメソッド
        /// </summary>
        private void Start()
        {
            _currentTime = _timeLimit;
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        private void Update()
        {
            if(!_isRunning)
            {
                return;
            }

            _currentTime -= Time.deltaTime;
            OnTimeChanged.Invoke(_currentTime);

            if(_currentTime <= 0f)
            {
                _currentTime = 0f;
                _isRunning = false;
                OnTimeUp.Invoke();
            }
        }

        // タイマーを開始するメソッド
        public void StartTimer() => _isRunning = true;

        // タイマーを停止するメソッド
        public void StopTimer() => _isRunning = false;

        /// <summary>
        /// タイマーをリセットして、時間を初期値に戻す。タイマーは停止状態になる。
        /// </summary>
        public void ResetTimer()
        {
            _currentTime = _timeLimit;
            _isRunning = false;
        }
    }
}
