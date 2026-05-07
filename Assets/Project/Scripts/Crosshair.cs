namespace LastSurvivor
{
    using UnityEngine;

    /// <summary>
    /// クロスヘアを生成するクラス
    /// </summary>
    public class Crosshair : MonoBehaviour
    {
        [Header("見た目")]
        private float _lineLength = 20f;
        private float _lineThickness = 2f;
        private Color _crosshairColor = Color.white;

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Start()
        {
            CreateCrosshair();
        }

        /// <summary>
        /// クロスヘアを生成するためのメソッド
        /// </summary>
        void CreateCrosshair()
        {
            CreateLine("Horizontal", new Vector2(_lineLength, _lineThickness));

            CreateLine("Vertical", new Vector2(_lineThickness, _lineLength));
        }

        /// <summary>
        /// クロスヘアの線を生成するためのメソッド
        /// </summary>
        /// <param name="name">線の名前</param>
        /// <param name="size">線のサイズ</param>
        void CreateLine(string name, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
            go.transform.SetParent(transform, false);

            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = size;

            go.GetComponent<UnityEngine.UI.Image>().color = _crosshairColor;
        }
    }
}
