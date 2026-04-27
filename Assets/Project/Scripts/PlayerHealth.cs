namespace LastSurvivor
{
    using R3;
    using UnityEngine;

    /// <summary>
    /// プレイヤーのHPを管理するクラス
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [Header("最大HP"), SerializeField]
        private float _maxHP;

        [Header("現在のHP"), SerializeField]
        private float _currentHP;

        [Header("HPバー"), SerializeField]
        private HPBar _hpBar;

        [Header("プレイヤーのステータス"), SerializeField]
        private PlayerStatus _playerStatus;

        /// <summary>
        /// ゲーム開始時にHPを最大値に設定し、HPバーを更新する
        /// </summary>
        void Start()
        {
            _playerStatus.CurrentHP
                .Subscribe(hp =>
                    _hpBar.SetUPTask(hp, _playerStatus.MaxHP))
                .AddTo(this);
        }

        /// <summary>
        /// ダメージを受けたときにHPを減らし、HPバーを更新する
        /// </summary>
        /// <param name="damage"> 受けるダメージ量 </param>
        private void Update()
        {
            // 仮置きとして、Fキーを押すと10のダメージを受ける
            if (Input.GetKeyDown(KeyCode.F))
            {
                _playerStatus.TakeDamageTask(10); // 例として10のダメージを受ける
            }
            _playerStatus.CurrentHP.Value = Mathf.Clamp(_playerStatus.CurrentHP.Value, 0, _playerStatus.MaxHP); // HPが0未満にならないようにする

        }
    }
}
