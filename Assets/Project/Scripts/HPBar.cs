namespace LastSurvivor
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// HPバーを管理するクラス
    /// </summary>
    public class HPBar : MonoBehaviour
    {
        [Header("HPバー"),SerializeField]
        private Slider _hpSlider;

        [Header("HPバーの塗りつぶし"), SerializeField]
        private Image _fillImage;

        [Header("高HPの色"), SerializeField]
        private Color _highHPColor = Color.green;

        [Header("中HPの色"), SerializeField]
        private Color _midHPColor = Color.yellow;      

        [Header("低HPの色"), SerializeField]
        private Color _lowHPColor = Color.red;
        

        [Header("中HPの閾値"), Range(0, 1), SerializeField]
        private float _midHPThreshold = 0.5f;
        [Header("低HPの閾値"), Range(0, 1), SerializeField]
        private float _lowHPThreshold = 0.25f;

        /// <summary>
        /// HPバーを更新するメソッド
        /// </summary>
        /// <param name="current">現在のHP</param>
        /// <param name="max">最大HP</param>
        public void SetUPTask(float current, float max)
        {
            float ratio = current / max;
            _hpSlider.value = ratio;

            if (ratio > _midHPThreshold)
            {
                _fillImage.color = _highHPColor;
            }

            else if (ratio > _lowHPThreshold)
            {
                _fillImage.color = _midHPColor;
            }
            else
            {
                _fillImage.color = _lowHPColor;
            }
        }
    }
}
