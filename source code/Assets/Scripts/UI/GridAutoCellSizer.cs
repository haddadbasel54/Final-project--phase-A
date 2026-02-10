using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class GridAutoCellSizer : MonoBehaviour
{
    public int columns = 2, rows = 2;
    GridLayoutGroup grid; RectTransform rt;

    void OnEnable() { grid = GetComponent<GridLayoutGroup>(); rt = GetComponent<RectTransform>(); Resize(); }
    void OnRectTransformDimensionsChange() { if (isActiveAndEnabled) Resize(); }

    void Resize()
    {
        if (!grid || !rt) return;
        var p = grid.padding;
        float w = rt.rect.width - p.left - p.right - grid.spacing.x * (columns - 1);
        float h = rt.rect.height - p.top - p.bottom - grid.spacing.y * (rows - 1);
        grid.cellSize = new Vector2(Mathf.Max(1, w / columns), Mathf.Max(1, h / rows));
    }
}
