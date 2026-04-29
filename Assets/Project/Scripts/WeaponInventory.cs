namespace LastSurvivor
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.InputSystem;

    /// <summary>
    /// プレイヤーの武器インベントリを管理するクラス
    /// </summary>
    public class WeaponInventory : MonoBehaviour
    {
        [Header("Weapon Data")]
        public WeaponData[] weapons;

        [Header("UI Slots")]
        public WeaponSlotUI[] weaponSlots;

        /// <summary>
        /// 現在選択されている武器がフルオートかどうかを返す
        /// </summary>
        public bool IsCurrentWeaponFillAuto => weapons != null &&
            _selectedIndex < weapons.Length &&
            weapons[_selectedIndex] != null &&
            weapons[_selectedIndex].IsFullAuto;

        /// <summary>
        /// 現在選択されている武器に弾薬が残っているかどうかを返す
        /// </summary>
        public bool HasAmmo => 
            !_isReloading &&
            weapons != null &&
            _selectedIndex < weapons.Length &&
            weapons[_selectedIndex] != null &&
            weapons[_selectedIndex].CurrentAmmo > 0;

        // 現在選択されている武器のデータを返す
        private int _selectedIndex = 0;

        // リロード中かどうかを管理するフラグ
        private bool _isReloading = false;

        /// <summary>
        /// ゲーム開始時に武器スロットのUIをセットアップし、最初の武器を選択する
        /// </summary>
        void Start()
        {
            for (var i = 0; i < weaponSlots.Length; i++)
            {
                if (i < weapons.Length && weapons[i] != null)
                {
                    weapons[i].CurrentAmmo = weapons[i].MaxAmmo;
                    weapons[i].ReserveAmmo = weapons[i].MaxReserveAmmo;
                }
                weaponSlots[i].Setup(i < weapons.Length ? weapons[i] : null);
            }

            // 最初の武器を選択
            SelectWeapon(0);
        }

        public void StartReload()
        {
            var w = weapons[_selectedIndex];
            if (_isReloading || w.CurrentAmmo == w.MaxAmmo || w.ReserveAmmo <= 0)
            {
                return;
            }

            StartCoroutine(ReloadCoroutine(w));
        }

        private IEnumerator ReloadCoroutine(WeaponData w)
        {
            _isReloading = true;
            yield return new WaitForSeconds(w.ReloadTime);

            int needed = w.MaxAmmo - w.CurrentAmmo;
            int take = Mathf.Min(needed, w.ReserveAmmo);
            w.CurrentAmmo += take;
            w.ReserveAmmo -= take;

            weaponSlots[_selectedIndex].UpdateAmmo();
            _isReloading = false;
        }

        /// <summary>
        /// 毎フレーム、マウスホイールの入力をチェックして武器の切り替えを行う また、数字キーで直接武器を選択できるようにする
        /// </summary>
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                StartReload();
            }

            // マウスホイールの入力をチェックして武器の切り替え
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f)
            {
                // 配列の範囲を考慮して次の武器を選択
                SelectWeapon((_selectedIndex + 1) % weapons.Length);
            }

            // スクロールが負の場合は前の武器を選択（配列の範囲を考慮してループさせる）
            else if (scroll < 0f)
            {
                // 配列の範囲を考慮して前の武器を選択
                SelectWeapon((_selectedIndex - 1 + weapons.Length) % weapons.Length);
            }

            // 数字キーで直接武器を選択
            for (var i = 0; i < weapons.Length && i < 4; i++)
            {
                // 数字キーはKeyCode.Alpha1から始まるため、iを足してチェック
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    // 数字キーに対応する武器を選択
                    SelectWeapon(i);
                }
            }
        }

        /// <summary>
        /// 指定されたインデックスの武器を選択し、UIを更新する
        /// </summary>
        /// <param name="index"> 選択する武器のインデックス </param>
        void SelectWeapon(int index)
        {
            _selectedIndex = index;
            for (var i = 0; i < weaponSlots.Length; i++)
            {
                // 各スロットの選択状態を更新
                weaponSlots[i].SetSelected(i == _selectedIndex);
            }
        }

        /// <summary>
        /// 現在選択されている武器の弾薬を消費し、UIを更新する
        /// </summary>
        public void ConsumeAmmo()
        {
            var w = weapons[_selectedIndex];
            if (w.CurrentAmmo > 0)
            {
                w.CurrentAmmo--;
                weaponSlots[_selectedIndex].UpdateAmmo();
            }
        }

        /// <summary>
        /// 指定された武器の弾薬を充填し、UIを更新する
        /// </summary>
        /// <param name="weaponIndex"> 充填する武器のインデックス </param>
        public void RefreshAmmo(int weaponIndex)
        {
            if (weaponIndex < 0 || weaponIndex >= weaponSlots.Length)
            {
                return;
            }
            weaponSlots[weaponIndex].UpdateAmmo();
        }
    }
}
