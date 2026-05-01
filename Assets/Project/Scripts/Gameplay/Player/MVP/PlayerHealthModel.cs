namespace LastSurvivor
{
    using R3;
    using UnityEngine;

    /// <summary>
    /// プレイヤーのHPを管理するモデルクラス
    /// </summary>
    public class PlayerHealthModel
    {
        private PlayerStatus _playerStatus;

        // 現在のHPを読み取り専用のリアクティブプロパティ
        public ReadOnlyReactiveProperty<int> CurrentHP => _playerStatus.CurrentHP;

        // 最大HP
        public int MaxHP => _playerStatus.MaxHP;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="playerStatus">プレイヤーのステータス情報</param>
        public PlayerHealthModel(PlayerStatus playerStatus)
        {
            _playerStatus = playerStatus;
        }

        // ダメージを受けるメソッド
        public void TakeDamage(int damage)
        {
            _playerStatus.TakeDamageTask(damage);
        }
    }
}
