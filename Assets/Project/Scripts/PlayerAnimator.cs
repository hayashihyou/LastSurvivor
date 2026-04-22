namespace LastSurvivor
{
    using UnityEngine;
    using R3;

    /// <summary>
    /// プレイヤーのアニメーションを管理するクラス
    /// </summary>
    public class PlayerAnimator : MonoBehaviour
    {
        [Header("アニメーター"),SerializeField]
        private Animator animator;

        private PlayerStatus playerStatus;
        private PlayerMover playerMover;

        /// <summary>
        /// アニメーターのパラメーターを定義するクラス
        /// </summary>
        private static class AnimatorParameters
        {
            public static readonly int IsRunning = Animator.StringToHash("isRunning");
            public static readonly int IsMoving = Animator.StringToHash("isMoving");
            public static readonly int IsDead = Animator.StringToHash("isDead");
        }

        /// <summary>
        /// Start()より先に呼ばれる処理
        /// </summary>
        void Awake()
        {
            // コンポーネントの取得
            playerStatus = GetComponent<PlayerStatus>();
            playerMover = GetComponent<PlayerMover>();

            // アニメーターがnullの時、アニメーターコンポーネントを取得
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        /// <summary>
        /// プレイヤーの状態に応じてアニメーターのパラメーターを更新する関数の呼び出し
        /// </summary>
        void Start()
        {
            SubscribeEvents();
        }

        /// <summary>
        /// プレイヤーの状態に応じてアニメーターのパラメーターを更新
        /// </summary>
        private void SubscribeEvents()
        {
            playerMover.isMoving
             .Subscribe(isMoving => 
                animator.SetBool(AnimatorParameters.IsMoving, isMoving))
             .AddTo(this);

            playerMover.isRunning
                .Subscribe(isRunning => 
                    animator.SetBool(AnimatorParameters.IsRunning, isRunning))
                .AddTo(this);

            playerStatus.isDead
                .Subscribe(isDead => 
                    animator.SetBool(AnimatorParameters.IsDead, isDead))
                .AddTo(this);
        }
    }
}
