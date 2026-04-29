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

        // ダメージ量
        private int _damage = 10;

        /// <summary>
        /// プレイヤーのHPが変化したときにビューを更新するための初期化処理
        /// </summary>
        void Start()
        {
            // モデルのHPが変化したときにビューを更新する
            _model.CurrentHP
                .Subscribe(currentHP =>_view.UpdateHP(currentHP, _model.MaxHP))
                .AddTo(this);

            // ビューのダメージ入力イベントに対して、モデルのTakeDamageメソッドを呼び出す
            _view.OnDamageInput += () => _model.TakeDamage(_damage);
        }
    }
}
