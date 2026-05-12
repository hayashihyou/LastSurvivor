namespace LastSurvivor
{
    using R3;

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

        // HPが0以下になったときに発火するイベント
        public Observable<Unit> OnDead => CurrentHP
            .Where(hp => hp <= 0)
            .Take(1)
            .Select(_ => Unit.Default);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="playerStatus">プレイヤーのステータス情報</param>
        public PlayerHealthModel(PlayerStatus playerStatus)
        {
            _playerStatus = playerStatus;
        }

        /// <summary>
        /// ダメージを受けるメソッド
        /// </summary>
        /// <param name="damage">受けるダメージ量</param>
        public void TakeDamage(int damage)
        {
            _playerStatus.TakeDamageTask(damage);
        }
    }
}
