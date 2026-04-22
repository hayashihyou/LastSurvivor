namespace LastSurvivor
{
    using R3;
    using UnityEngine;

    /// <summary>
    /// プレイヤーのステータスを管理するクラス
    /// </summary>
    public class PlayerStatus : MonoBehaviour
    {
        [Header("HP"),SerializeField]
        private int maxHP = 100;

        [Header("歩く速度"),SerializeField]
        private float walkSpeed = 3f;

        [Header("走る速度"),SerializeField]
        private float runSpeed = 6f;

        [Header("攻撃力"),SerializeField]
        private int attackPower = 10;

        // R3で値の変化を監視
        public ReactiveProperty<int> currentHP {get; private set; }
        public ReactiveProperty<bool> isDead {get; private set; }

        // 読み取り専用
        public int MaxHP => maxHP;
        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;
        public int AttackPower => attackPower;

        /// <summary>
        /// Start()より前に呼ばれる処理
        /// </summary>
        private void Awake()
        {
            currentHP = new ReactiveProperty<int>(maxHP);
            isDead = new ReactiveProperty<bool>(false);

            // HPが0以下になったときに死亡フラグを立てる
            currentHP
                .Where(hp => hp <= 0)
                .Subscribe(_ =>
                {
                    currentHP.Value = 0; 
                    isDead.Value = true;
                })
                .AddTo(this);
        }

        /// <summary>
        /// ダメージを受ける処理
        /// </summary>
        /// <param name="damage">受けるダメージ量</param>
        public void TakeDamage(int damage)
        {
            if (isDead.Value) return; 
            currentHP.Value -= damage;
        }

        /// <summary>
        /// HPを回復する処理
        /// </summary>
        /// <param name="amount">回復するHP量</param>
        public void Heal(int amount)
        {
            if (isDead.Value) return; 
            currentHP.Value = Mathf.Min(currentHP.Value + amount, maxHP);
        }
    }
}
