using Godot;

[GlobalClass]
public partial class DrawableObject : Node2D, IObjectPoolObject
{

    public Rid canvasItemRid;
    public GameShader ShaderInstance
    {
        get => _shaderInstance;
        set
        {
            _shaderInstance = value;
            Material = value.GetShaderMaterial();
        }
    }

    protected GameShader _shaderInstance;

    public virtual void Awake()
    {
        Transform = Transform2D.Identity;
        Modulate = Colors.White;
        Material = null;
        canvasItemRid = GetCanvasItem();
        Redraw();
    }

    public virtual void Disabled()
    {
    }

    public virtual void Destroy()
    {
        UtmxSceneManager.DeleteDrawableObject(this);
    }

    public void Redraw()
    {
        RenderingServer.CanvasItemClear(canvasItemRid);
    }

    public void DrawCircle(Vector2 pos, double radius, string color = default)
    {
        RenderingServer.CanvasItemAddCircle(canvasItemRid, pos, (float)radius, 
            Color.FromString(color, Colors.White));
    }
    public void DrawRect(Vector2 pos, Vector2 size, string color = default)
    {
        RenderingServer.CanvasItemAddRect(canvasItemRid, new Rect2(pos, size), 
            Color.FromString(color, Colors.White));
    }
    public void DrawLine(Vector2 from, Vector2 to, string color = default, double width = -1)
    {
        RenderingServer.CanvasItemAddLine(canvasItemRid, from, to, 
            Color.FromString(color, Colors.White), (float)width);
    }
    public void DrawPolygon(Vector2[] vertices, Color[] colors = default, Vector2[] uvs = default, string path = "")
    {
        Resource res = UtmxResourceLoader.Load(path);
        Texture2D texture = null;
        if (!string.IsNullOrEmpty(path) && res is Texture2D)
        {
            texture = (Texture2D)res;
        }
        else
        {
            if (uvs.Length == 0)
            {
                uvs = new Vector2[vertices.Length];
            }
        }
        if (colors.Length == 0)
        {
            colors = new Color[vertices.Length];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = Colors.White;
        }
        RenderingServer.CanvasItemAddPolygon(canvasItemRid, vertices, colors, uvs,
            (texture == null) ? default: texture.GetRid());
    }

    public void DrawTextureRect(string path, Vector2 pos, Vector2 size, Color color = default)
    {
        Resource res = UtmxResourceLoader.Load(path);
        if (res != null && res is Texture2D texture)
        {
            RenderingServer.CanvasItemAddTextureRect(canvasItemRid, new Rect2(pos, size), texture.GetRid(), false,
                color);
        }
    }
    public void DrawTexturePos(string path, Vector2 posTopLeft, 
            Vector2 posTopRight, Vector2 posBottomRight, Vector2 posBottomLeft, Color[] colors = default)
    {
        DrawPolygon(
            [posTopLeft, posTopRight, posBottomRight, posBottomLeft],
            colors, 
            [Vector2.Zero, new Vector2(1, 0), Vector2.One, new Vector2(0, 1)], path);
    }
    public void DrawText(Vector2 pos, string text, Color color = default, double size = 16, string fontPath = "")
    {
        Font font;
        if (!string.IsNullOrEmpty(fontPath))
        {
            Resource res = UtmxResourceLoader.Load(fontPath);
            font = (res as Font) ?? ThemeDB.FallbackFont;
        }
        else
        {
            font = ThemeDB.FallbackFont;
        }
        font.DrawString(canvasItemRid, pos, text, HorizontalAlignment.Left, -1F, (int)size, color);
    }
}
