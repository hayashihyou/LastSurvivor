namespace LastSurvivor
{
    using UnityEngine;
    using System;

    /// <summary>
    /// プレイヤーのHPを表示するビュークラス
    /// </summary>
    public class PlayerHealthView : MonoBehaviour
    {
        [Header("プレイヤーのHPバー"), SerializeField]
        private HPBar _hpBar;

        // 入力をPresenterに伝えるためのイベント
        public event Action OnDamageInput;

        /// <summary>
        /// HPバーを更新するためのメソッド
        /// </summary>
        /// <param name="current">現在のHP</param>
        /// <param name="max">最大HP</param>
        public void UpdateHP(int current, int max)
        {
            // HPバーを更新する
            _hpBar.SetUPTask((float)current, (float)max);
        }

        /// <summary>
        /// ダメージ入力を検出するための更新処理
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                // ダメージ入力があったときにイベントを発火する
                OnDamageInput?.Invoke();
            }
        }
    }
}