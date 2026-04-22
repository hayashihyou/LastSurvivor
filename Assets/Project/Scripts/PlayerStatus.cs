namespace LastSurvivor
{
    using R3;
    using UnityEngine;

    /// <summary>
    /// プレイヤーのステータスを管理するクラス
    /// </summary>
    public class PlayerStatus : MonoBehaviour
    {
        [Header("HP"), SerializeField]
        private int _maxHP = 100;

        [Header("歩行速度"), SerializeField]
        private float _walkSpeed = 3f;

        [Header("走行速度"), SerializeField]
        private float _runSpeed = 6f;

        [Header("攻撃力"), SerializeField]
        private int _attackPower = 10;

        // プレイヤーの現在のHPと死亡状態を管理するReactiveProperty
        public ReactiveProperty<int> CurrentHP { get; private set; }
        public ReactiveProperty<bool> IsDead { get; private set; }

        // プレイヤーの最大HP、歩行速度、走行速度、攻撃力を公開プロパティ
        public int MaxHP => _maxHP;
        public float WalkSpeed => _walkSpeed;
        public float RunSpeed => _runSpeed;
        public int AttackPower => _attackPower;

        /// <summary>
        /// インスタンス化直後に呼び出される初期化処理
        /// </summary>
        private void Awake()
        {
            CurrentHP = new ReactiveProperty<int>(_maxHP);
            IsDead = new ReactiveProperty<bool>(false);

            // HPが0以下になったときに死亡状態を更新
            CurrentHP
                .Where(hp => hp <= 0)
                .Subscribe(_ =>
                {
                    CurrentHP.Value = 0;
                    IsDead.Value = true;
                })
                .AddTo(this);
        }

        /// <summary>
        /// ダメージを受ける
        /// </summary>
        /// <param name="damage"> 受けるダメージ </param>
        public void TakeDamageTask(int damage)
        {
            if (IsDead.Value)
            {
                return;
            }
            CurrentHP.Value -= damage;
        }

        /// <summary>
        /// HPを回復する
        /// </summary>
        /// <param name="amount"> 回復するHP量 </param>
        public void HealTask(int amount)
        {
            if (IsDead.Value)
            {
                return;
            }
            CurrentHP.Value = Mathf.Min(CurrentHP.Value + amount, _maxHP);
        }
    }
}
