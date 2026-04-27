namespace LastSurvivor
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using LastSurvivor;

    /// <summary>
    /// 武器スロットのUIを管理するクラス
    /// </summary>
    public class WeaponSlotUI : MonoBehaviour
    {
        [Header("UI References")]
        public Image IconImage;
        public TextMeshProUGUI AmmoText;
        public Image HighlightBorder;

        [Header("Colors")]
        public Color SelectColor = new Color(0.2f, 1.0f, 0.2f, 1.0f);
        public Color DefaultColor = new Color(1.0f, 1.0f, 1.0f, 0.3f);

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
                IconImage.enabled = false;
                AmmoText.text = "";
                return;
            }

            IconImage.enabled = true;
            IconImage.sprite = _data.Icon;
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

            AmmoText.text = $"{_data.CurrentAmmo} / {_data.ReserveAmmo}";
        }

        /// <summary>
        /// このスロットが選択されているかどうかを設定
        /// </summary>
        /// <param name="isSelected"> 選択されているかどうか </param>
        public void SetSelected(bool isSelected)
        {
            HighlightBorder.color = isSelected ? SelectColor : DefaultColor;
        }
    }
}
