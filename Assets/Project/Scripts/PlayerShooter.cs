namespace LastSurvivor
{
    using R3;
    using UnityEngine;

    /// <summary>
    /// プレイヤーの射撃を制御するクラス
    /// </summary>
    public class PlayerShooter : MonoBehaviour
    {
        [Header("射撃設定"), SerializeField]
        private BulletController _bulletController;

        [Header("プレイヤーステータス"), SerializeField]
        private PlayerStatus _playerStatus;

        [Header("連射速度"), SerializeField]
        private float _fireRate = 0.1f;

        [Header("反動設定"), SerializeField]
        private Transform _weaponPivot;

        [Header("反動の強さ"), SerializeField]
        private float _recoilForce = 0.05f;

        [Header("元に戻る速さ"), SerializeField]
        private float _returnSpeed = 5f;

        [Header("インベントリ"),SerializeField]
        private WeaponInventory _weaponInventory;

        // 武器の元の位置
        private Vector3 _originalPosition;

        // 射撃状態を管理するプロパティ
        public ReactiveProperty<bool> IsFiring { get; private set; } = new ReactiveProperty<bool>(false);

        /// <summary>
        /// ゲーム開始時に武器の元の位置を保存し、イベントを購読する
        /// </summary>
        private void Start()
        {
            // 武器の元の位置を保存
            _originalPosition = _weaponPivot.localPosition;
            SubscribeEvents();
        }

        /// <summary>
        /// 毎フレーム、武器を元の位置に戻す処理を行う
        /// </summary>
        private void Update()
        {
            // 武器を元の位置に戻す
            _weaponPivot.localPosition = Vector3.Lerp(_weaponPivot.localPosition, _originalPosition, Time.deltaTime * _returnSpeed);
        }

        /// <summary>
        /// プレイヤーの状態や入力に応じて射撃処理を行うイベントを購読する
        /// </summary>
        private void SubscribeEvents()
        {
            // 死亡時に射撃状態を強制でオフにする
            _playerStatus.IsDead
                .Where(isDead => isDead)
                .Subscribe(_ => IsFiring.Value = false)
                .AddTo(this);

            //射撃入力の検知と射撃処理
            Observable.EveryUpdate()
                .Where(_ => !_playerStatus.IsDead.Value)
                .Subscribe(_ => IsFiring.Value = Input.GetButton("Fire1"))
                .AddTo(this);

            // 連射の処理
            Observable.EveryUpdate()
                .Where(_ => Input.GetButton("Fire1") 
                && !_playerStatus.IsDead.Value 
                && _weaponInventory.IsCurrentWeaponFillAuto
                && _weaponInventory.HasAmmo)
                .ThrottleFirst(
                    System.TimeSpan.FromSeconds(_fireRate),
                    UnityTimeProvider.Update
                )
                .Subscribe(_ =>
                {
                    _bulletController.Shoot();
                    // 反動を加える
                    ApplyRecoil();
                    _weaponInventory.ConsumeAmmo();
                })
                .AddTo(this);

            // 単発射撃の処理
            Observable.EveryUpdate()
                .Where(_ => Input.GetButtonDown("Fire1") 
                && !_playerStatus.IsDead.Value 
                && !_weaponInventory.IsCurrentWeaponFillAuto
                && _weaponInventory.HasAmmo)
                .Subscribe(_ =>
                {
                    _bulletController.Shoot();
                    // 反動を加える
                    ApplyRecoil();
                    _weaponInventory.ConsumeAmmo();
                })
                .AddTo(this);
        }

        /// <summary>
        /// 射撃時に武器に反動を加えるメソッド
        /// </summary>
        private void ApplyRecoil()
        {
            // 武器を後ろに移動させる
            _weaponPivot.localPosition -= new Vector3(0, 0, -_recoilForce);
        }
    }
}