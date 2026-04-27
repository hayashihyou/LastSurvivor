using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [Header("見た目")]
    public float lineLength = 20f;
    public float lineThickness = 2f;
    public Color crosshairColor = Color.white;

    private void Start()
    {
        CreateCrosshair();  
    }

    void CreateCrosshair()
    {
        CreateLine("Horizontal", new Vector2(lineLength, lineThickness));

        CreateLine("Vertical", new Vector2(lineThickness, lineLength));
    }

    void CreateLine(string name, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
        go.transform.SetParent(transform, false);

        var rt =go.GetComponent<RectTransform>();
        rt.sizeDelta = size;

        go.GetComponent<UnityEngine.UI.Image>().color = crosshairColor;
    }
}
