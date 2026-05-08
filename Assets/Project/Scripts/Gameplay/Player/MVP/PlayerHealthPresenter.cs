namespace LastSurvivor
{
    using R3;
    using UnityEngine;

    /// <summary>
    /// プレイヤーのHPを管理するプレゼンタークラス
    /// </summary>
    public class PlayerHealthPresenter : MonoBehaviour
    {
        [Header("プレイヤーのHPモデル"),SerializeField]
        private PlayerHealthModel _model;

        [Header("プレイヤーのHPビュー"),SerializeField]
        private PlayerHealthView _view;

        [Header("プレイヤーのステータス"),SerializeField]
        private PlayerStatus _playerStatus;

        // NOTE: hayashi ダメージ量を設定しているが、敵を実装したら敵の攻撃力に応じてダメージ量を変えるようにする
        private int _damage = 10;

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

            // NOTE: hayashi Fキーの入力を受け取った時にダメージを受ける処理を実装しているが、敵を実装したら敵の攻撃に対してダメージを受ける処理に置き換える
            _view.OnDamageInput += () => _model.TakeDamage(_damage);
        }
    }
}
