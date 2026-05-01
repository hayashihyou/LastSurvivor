namespace LastSurvivor
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    /// <summary>
    /// 武器スロットのUIを管理するクラス
    /// </summary>
    public class WeaponSlotUI : MonoBehaviour
    {
        [Header("アイコン"),SerializeField]
        private Image _iconImage;

        [Header("弾薬数"),SerializeField]
        private TextMeshProUGUI _ammoText;

        [Header("選択ハイライト"),SerializeField]
        private Image _highlightBorder;

        [Header("Colors")]
        private Color _selectColor = new Color(0.2f, 1.0f, 0.2f, 1.0f);
        private Color _defaultColor = new Color(1.0f, 1.0f, 1.0f, 0.3f);

        // 現在の武器データを保持する
        private WeaponData _data;

        /// <summary>
        /// 武器スロットのUIをセットアップする
        /// </summary>
        /// <param name="data"> セットアップする武器データ </param>
        public void Setup(WeaponData data)
        {
            _data = data;
            if (_data == null)
            {
                _iconImage.enabled = false;
                _ammoText.text = "";
                return;
            }

            _iconImage.enabled = true;
            _iconImage.sprite = _data.Icon;
            UpdateAmmo();
        }

        /// <summary>
        /// 武器の弾薬数を更新する
        /// </summary>
        public void UpdateAmmo()
        {
            if (_data == null)
            {
                return;
            }

            _ammoText.text = $"{_data.CurrentAmmo} / {_data.ReserveAmmo}";
        }

        /// <summary>
        /// このスロットが選択されているかどうかを設定
        /// </summary>
        /// <param name="isSelected"> 選択されているかどうか </param>
        public void SetSelected(bool isSelected)
        {
            _highlightBorder.color = isSelected ? _selectColor : _defaultColor;
        }
    }
}
