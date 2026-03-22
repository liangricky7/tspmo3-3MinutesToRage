using UnityEngine;
using UnityEngine.UI;

public class TrapezoidalGraphic : Graphic
{
    [Range(0f, 1f)]
    [SerializeField] private float topWidthPercent = 0.4f;  // narrow at top
    [Range(0f, 1f)]
    [SerializeField] private float bottomWidthPercent = 1f; // full width at bottom

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect r = GetPixelAdjustedRect();

        float left = r.xMin;
        float right = r.xMax;
        float top = r.yMax;
        float bottom = r.yMin;
        float fullWidth = r.width;

        float topWidth = fullWidth * topWidthPercent;
        float bottomWidth = fullWidth * bottomWidthPercent;

        UIVertex vertex = new UIVertex();
        vertex.color = color;

        // Bottom-left (angled inward from right)
        vertex.position = new Vector3(right - bottomWidth, bottom);
        vertex.color = color;
        vh.AddVert(vertex); // 0

        // Bottom-right (straight right edge)
        vertex.position = new Vector3(right, bottom);
        vertex.color = color;
        vh.AddVert(vertex); // 1

        // Top-right (straight right edge)
        vertex.position = new Vector3(right, top);
        vertex.color = color;
        vh.AddVert(vertex); // 2

        // Top-left (angled inward from right)
        vertex.position = new Vector3(right - topWidth, top);
        vertex.color = color;
        vh.AddVert(vertex); // 3

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }
}