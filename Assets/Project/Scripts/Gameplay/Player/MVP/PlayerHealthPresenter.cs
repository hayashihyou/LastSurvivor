namespace LastSurvivor
{
    using R3;
    using UnityEngine;

    /// <summary>
    /// プレイヤーのHPを管理するプレゼンタークラス
    /// </summary>
    public class PlayerHealthPresenter : MonoBehaviour
    {
        [Header("プレイヤーのHPビュー"),SerializeField]
        private PlayerHealthView _view;

        [Header("プレイヤーのステータス"),SerializeField]
        private PlayerStatus _playerStatus;

        // プレイヤーのHPを管理するモデル
        private PlayerHealthModel _model;

        /// <summary>
        /// プレイヤーのHPが変化したときにビューを更新するための初期化処理
        /// </summary>
        private void Start()
        {
            _model = new PlayerHealthModel(_playerStatus);

            // モデルのHPが変化したときにビューを更新する
            _model.CurrentHP
                .Subscribe(currentHP =>_view.UpdateHP(currentHP, _model.MaxHP))
                .AddTo(this);

            _model.OnDead
                .Subscribe(_ => GameManager.Instance?.OnPlayerDead())
                .AddTo(this);
        }

        public void Heal(int healAmount)
        {
            _model.HealTask(healAmount);
        }
    }
}
