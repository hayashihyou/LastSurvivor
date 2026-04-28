namespace LastSurvivor
{
    using R3;
    using UnityEngine;

    /// <summary>
    /// プレイヤーのHPを管理するモデルクラス
    /// </summary>
    public class PlayerHealthModel : MonoBehaviour
    {
        [Header("プレイヤーのステータス"),SerializeField]
        private PlayerStatus _playerStatus;

        // 現在のHPを読み取り専用のリアクティブプロパティ
        public ReadOnlyReactiveProperty<int> CurrentHP => _playerStatus.CurrentHP;

        // 最大HP
        public int MaxHP => _playerStatus.MaxHP;

        // ダメージを受けるメソッド
        public void TakeDamage(int damage)
        {
            _playerStatus.TakeDamageTask(damage);
        }
    }
}
