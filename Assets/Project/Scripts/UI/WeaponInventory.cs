namespace LastSurvivor
{
    using UnityEngine;
    using Cysharp.Threading.Tasks;
    using System.Threading;

    /// <summary>
    /// プレイヤーの武器インベントリを管理するクラス
    /// </summary>
    public class WeaponInventory : MonoBehaviour
    {
        [Header("Weapon Data"), SerializeField]
        private WeaponData[] _weaponData;

        [Header("UI Slots"), SerializeField]
        private WeaponSlotUI[] _weaponSlotUI;

        [Header("リロードのSE"), SerializeField]
        private AudioClip _reloadSE;

        // リロードのSEを再生するためのAudioSource
        private AudioSource _audioSource;

        /// <summary>
        /// 現在選択されている武器がフルオートかどうかを返す
        /// </summary>
        public bool IsCurrentWeaponFillAuto =>
            _weaponData != null &&
            _selectedIndex < _weaponData.Length &&
            _weaponData[_selectedIndex] != null &&
            _weaponData[_selectedIndex].IsFullAuto;

        /// <summary>
        /// 現在選択されている武器に弾薬が残っているかどうかを返す
        /// </summary>
        public bool HasAmmo =>
            !_isReloading &&
            _weaponData != null &&
            _selectedIndex < _weaponData.Length &&
            _weaponData[_selectedIndex] != null &&
            _weaponData[_selectedIndex].CurrentAmmo > 0;

        // 現在選択されている武器のデータを返す
        private int _selectedIndex = 0;

        // リロード中かどうかを管理するフラグ
        private bool _isReloading = false;

        private CancellationTokenSource _reloadCts;

        private int[] _runtimeMaxReserveAmmos;

        /// <summary>
        /// ゲーム開始時に武器スロットのUIをセットアップし、最初の武器を選択する
        /// </summary>
        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();

            // 実行時上限を WeaponData の初期値で初期化
            _runtimeMaxReserveAmmos = new int[_weaponData.Length];

            for (var i = 0; i < _weaponSlotUI.Length; i++)
            {
                if (i < _weaponData.Length && _weaponData[i] != null)
                {
                    _weaponData[i].CurrentAmmo = _weaponData[i].MaxAmmo;
                    _weaponData[i].ReserveAmmo = _weaponData[i].MaxReserveAmmo;
                    _runtimeMaxReserveAmmos[i] = _weaponData[i].MaxReserveAmmo;
                }
                _weaponSlotUI[i].Setup(i < _weaponData.Length ? _weaponData[i] : null);
            }
        }

        private void OnDestroy()
        {
            // GameObjectが破棄されるときにリロードのキャンセル
            _reloadCts?.Cancel();
            _reloadCts?.Dispose();
        }

        public void StartReload()
        {
            var w = _weaponData[_selectedIndex];
            var CannotReload = _isReloading || w.CurrentAmmo == w.MaxAmmo || w.ReserveAmmo <= 0;

            if (CannotReload)
            {
                return;
            }

            // 既にリロード中の場合はキャンセルしてから新しいリロードを開始
            _reloadCts?.Cancel();
            _reloadCts?.Dispose();
            _reloadCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

            ReloadTask(w, _reloadCts.Token).Forget();
        }

        private async UniTaskVoid ReloadTask(WeaponData w, CancellationToken ct)
        {
            _isReloading = true;

            if(_reloadSE != null)
            {
                _audioSource.PlayOneShot(_reloadSE);
            }

            // リロード時間を待機する。キャンセルされた場合は例外がスローされるので、catchブロックで処理する
            try
            {
                await UniTask.Delay(
                    (int)(w.ReloadTime * 1000),
                    cancellationToken: ct
                    );
            }

            catch (System.OperationCanceledException)
            {
                _audioSource.Stop();
                // リロードがキャンセルされた場合は何もしない
                _isReloading = false;
                return;
            }

            int needed = w.MaxAmmo - w.CurrentAmmo;
            int take = Mathf.Min(needed, w.ReserveAmmo);
            w.CurrentAmmo += take;
            w.ReserveAmmo -= take;
            _weaponSlotUI[_selectedIndex].UpdateAmmo();
            _isReloading = false;
        }

        /// <summary>
        /// 毎フレーム、マウスホイールの入力をチェックして武器の切り替えを行う また、数字キーで直接武器を選択できるようにする
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartReload();
            }

            // マウスホイールの入力をチェックして武器の切り替え
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f)
            {
                // 配列の範囲を考慮して次の武器を選択
                SelectWeapon((_selectedIndex + 1) % _weaponData.Length);
            }

            // スクロールが負の場合は前の武器を選択（配列の範囲を考慮してループさせる）
            else if (scroll < 0f)
            {
                // 配列の範囲を考慮して前の武器を選択
                SelectWeapon((_selectedIndex - 1 + _weaponData.Length) % _weaponData.Length);
            }

            var limit = Mathf.Min(_weaponData.Length, 4);

            for (var i = 0; i < limit; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    SelectWeapon(i);
                }
            }
        }

        /// <summary>
        /// 指定されたインデックスの武器を選択し、UIを更新する
        /// </summary>
        /// <param name="index"> 選択する武器のインデックス </param>
        private void SelectWeapon(int index)
        {
            _selectedIndex = index;
            for (var i = 0; i < _weaponSlotUI.Length; i++)
            {
                // 各スロットの選択状態を更新
                _weaponSlotUI[i].SetSelected(i == _selectedIndex);
            }
        }

        /// <summary>
        /// 現在選択されている武器の弾薬を消費し、UIを更新する
        /// </summary>
        public void ConsumeAmmo()
        {
            var w = _weaponData[_selectedIndex];
            if (w.CurrentAmmo > 0)
            {
                w.CurrentAmmo--;
                _weaponSlotUI[_selectedIndex].UpdateAmmo();
            }
        }

        /// <summary>
        /// 指定された武器の弾薬を充填し、UIを更新する
        /// </summary>
        /// <param name="weaponIndex"> 充填する武器のインデックス </param>
        public void RefreshAmmo(int weaponIndex)
        {
            if (weaponIndex < 0 || weaponIndex >= _weaponSlotUI.Length)
            {
                return;
            }
            _weaponSlotUI[weaponIndex].UpdateAmmo();
        }

        /// <summary>
        /// 全ての武器の弾薬を充填し、UIを更新する
        /// </summary>
        /// <param name="amount"> 充填する弾薬の量 </param>
        public void AddReserveAmmo(int amount)
        {
            for (var i = 0; i < _weaponData.Length; i++)
            {
                var w = _weaponData[i];
                if (w == null)
                {
                    continue;
                }

                var newReserve = w.ReserveAmmo + amount;

                // 弾薬アイテムを回収した時、実行時上限を超える場合は上限を引き上げる
                if (newReserve > _runtimeMaxReserveAmmos[i])
                {
                    var overflow = newReserve - _runtimeMaxReserveAmmos[i];
                    _runtimeMaxReserveAmmos[i] += overflow;
                }

                w.ReserveAmmo = newReserve;
                RefreshAmmo(i);
            }
        }
    }
}
