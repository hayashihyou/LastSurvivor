namespace LastSurvivor
{
    using UnityEngine;
    using R3;

    /// <summary>
    /// �v���C���[�̃A�j���[�V�������Ǘ�����N���X
    /// </summary>
    public class PlayerAnimator : MonoBehaviour
    {
        [Header("�A�j���[�^�["), SerializeField]
        private Animator _animator;

        [Header("�v���C���[�̃X�e�[�^�X"), SerializeField]
        private PlayerStatus _playerStatus;

        [Header("�v���C���[�̈ړ�"), SerializeField]
        private PlayerMover _playerMover;

        /// <summary>
        /// �A�j���[�^�[�̃p�����[�^�[���`
        /// </summary>
        public static readonly int IsRunning = Animator.StringToHash("isRunning");
        public static readonly int IsMoving = Animator.StringToHash("isMoving");
        public static readonly int IsDead = Animator.StringToHash("isDead");



        /// <summary>
        /// インスタンス化直後に呼び出される初期化処理
        /// </summary>
        void Awake()
        {
            // �A�j���[�^�[��null�̎��A�A�j���[�^�[�R���|�[�l���g���擾
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
        }

        /// <summary>
        /// インスタンス化直後に呼び出される初期化処理
        /// </summary>
        void Start()
        {
            SubscribeEvents();
        }

        /// <summary>
        /// �v���C���[�̏�Ԃɉ����ăA�j���[�^�[�̃p�����[�^�[���X�V
        /// </summary>
        private void SubscribeEvents()
        {
            _playerMover.IsMoving
             .Subscribe(isMoving =>
                _animator.SetBool(IsMoving, isMoving))
             .AddTo(this);

            _playerMover.IsRunning
                .Subscribe(isRunning =>
                    _animator.SetBool(IsRunning, isRunning))
                .AddTo(this);

            _playerStatus.IsDead
                .Subscribe(isDead =>
                    _animator.SetBool(IsDead, isDead))
                .AddTo(this);
        }
    }
}
