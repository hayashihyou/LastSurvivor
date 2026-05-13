namespace LastSurvivor
{
    using UnityEngine;

    /// <summary>
    /// プレイヤーのHPを表示するビュークラス
    /// </summary>
    public class PlayerHealthView : MonoBehaviour
    {
        [Header("プレイヤーのHPバー"), SerializeField]
        private HPBar _hpBar;

        /// <summary>
        /// HPバーを更新するためのメソッド
        /// </summary>
        /// <param name="current">現在のHP</param>
        /// <param name="max">最大HP</param>
        public void UpdateHP(int current, int max)
        {
            // HPバーを更新する
            _hpBar.SetUP((float)current, (float)max);
        }
    }
}