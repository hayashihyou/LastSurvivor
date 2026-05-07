namespace LastSurvivor
{
    using R3;
    using UnityEngine;

    /// <summary>
    /// 敵のステータスを管理するクラス
    /// </summary>
    public class EnemyStatus : MonoBehaviour
    {
        [Header("HP"), SerializeField]
        private int _maxHP = 50;

        [Header("移動速度"), SerializeField]
        private float _moveSpeed = 2f;

        [Header("攻撃力"), SerializeField]
        private int _attackPower = 5;

        [Header("攻撃範囲"), SerializeField]
        private float _attackRange = 1.5f;

        [Header("索敵範囲"), SerializeField]
        private float _detectionRange = 8f;

        // ReactivePropertyを使用して、HPと死亡状態を管理
        public ReactiveProperty<int> CurrentHP { get; private set; }
        public ReactiveProperty<bool> IsDead { get; private set; }

        // 公開プロパティ
        public int MaxHP => _maxHP;
        public float MoveSpeed => _moveSpeed;
        public int AttackPower => _attackPower;
        public float AttackRange => _attackRange;
        public float DetectionRange => _detectionRange;

        /// <summary>
        /// インスタンス直後に呼び出される初期化メソッド
        /// </summary>
        void Awake()
        {
            // ReactivePropertyの初期化
            CurrentHP = new ReactiveProperty<int>(_maxHP);
            IsDead=new ReactiveProperty<bool>(false);

            // HPが0以下になったときの処理を購読
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
        /// ダメージを受ける処理
        /// </summary>
        /// <param name="damage">受けるダメージ量</param>
        public void TakeDamageTask(int damage)
        {
            if (IsDead.Value)
            {
                return;
            }
            CurrentHP.Value -= damage;
        }

        public void Heal(int amount)
        {
            if(IsDead.Value)
            {
                return;
            }
            CurrentHP.Value = Mathf.Min(CurrentHP.Value + amount, _maxHP);
        }
    }
}
