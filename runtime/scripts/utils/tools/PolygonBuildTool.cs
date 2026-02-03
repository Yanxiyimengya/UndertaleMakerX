using Godot;

public static class PolygonBuildTool
{
    public static float GetPolygonSignedArea(Vector2[] polygon)
    {
        float area = 0.0f;
        int n = polygon.Length;

        for (int i = 0; i < n; i++)
        {
            var p1 = polygon[i];
            var p2 = polygon[(i + 1) % n];
            area += p1.Cross(p2);
        }

        return area / 2.0f;
    }

    public static Vector2[] ExpandPolygon(Vector2[] polygon, float weight)
    {
        var result = new Vector2[polygon.Length];
        bool isCw = GetPolygonSignedArea(polygon) > 0.0f;

        for (int i = 0; i < polygon.Length; i++)
        {
            var current = polygon[i];
            var prev = polygon[(i - 1 + polygon.Length) % polygon.Length];
            var next = polygon[(i + 1) % polygon.Length];

            // 获取左右两个相邻点  
            var vPrev = (current - prev).Normalized();
            var vNext = (current - next).Normalized();

            // 构造相邻边方向单位向量  
            var cross = vNext.Cross(vPrev);
            var isConcave = isCw ? cross > 0 : cross < 0;
            var area = Mathf.Abs(cross);

            if (Mathf.IsZeroApprox(area)) continue;

            var sumDir = vPrev + vNext;
            var s = weight / area * (isConcave ? 1 : -1);
            var offset = sumDir * s;

            result[i] = current + offset;
        }

        return result;
    }


    public static Rect2 GetBBox(Vector2[] polygon) {
        if (polygon.Length <= 0) return new Rect2 ();
        Rect2 bounds = new Rect2(polygon[0], Vector2.Zero);
        for (int i = 1; i < polygon.Length; i++) {
            bounds = bounds.Expand(polygon[i]);
        }
        return bounds;
    }
}
