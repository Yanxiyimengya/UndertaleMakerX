using Godot;
using System;

[Tool]
[GlobalClass]
public partial class BattleArenaGroup : Node2D
{
	private Rid _arenaBorderRenderingCanvasItem;
	private Rid _arenaBorderCullingCanvasItem;
	private Rid _arenaMaskRenderingCanvasItem;
	private Rid _arenaMaskCullingCanvasItem;
	private Rid _borderViewport;
	private Rid _maskViewport;
	private Rid _borderCanvas;
	private Rid _maskCanvas;
	private Rid _borderViewportTextureRid;
	private Rid _maskViewportTextureRid;
	private Rid _borderViewportCamera;
	private static CanvasItemMaterial _cullingMaterial = new CanvasItemMaterial();

	public override void _EnterTree()
	{
		_borderCanvas = RenderingServer.CanvasCreate();
		_maskCanvas = RenderingServer.CanvasCreate();

		_borderViewport = RenderingServer.ViewportCreate();
		RenderingServer.ViewportSetActive(_borderViewport, true);
		RenderingServer.ViewportSetTransparentBackground(_borderViewport, true);
		RenderingServer.ViewportSetClearMode(_borderViewport, RenderingServer.ViewportClearMode.Always);
		RenderingServer.ViewportSetUpdateMode(_borderViewport, RenderingServer.ViewportUpdateMode.Always);
		RenderingServer.ViewportAttachCanvas(_borderViewport, _borderCanvas);

		_maskViewport = RenderingServer.ViewportCreate();
		RenderingServer.ViewportSetActive(_maskViewport, true);
		RenderingServer.ViewportSetTransparentBackground(_maskViewport, true);
		RenderingServer.ViewportSetClearMode(_maskViewport, RenderingServer.ViewportClearMode.Always);
		RenderingServer.ViewportSetUpdateMode(_maskViewport, RenderingServer.ViewportUpdateMode.Always);
		RenderingServer.ViewportAttachCanvas(_maskViewport, _maskCanvas);

		_borderViewportTextureRid = RenderingServer.ViewportGetTexture(_borderViewport);
		_maskViewportTextureRid = RenderingServer.ViewportGetTexture(_maskViewport);


		_cullingMaterial.BlendMode = CanvasItemMaterial.BlendModeEnum.Sub;
		_arenaBorderRenderingCanvasItem = RenderingServer.CanvasItemCreate();
		_arenaBorderCullingCanvasItem = RenderingServer.CanvasItemCreate();
		_arenaMaskRenderingCanvasItem = RenderingServer.CanvasItemCreate();
		_arenaMaskCullingCanvasItem = RenderingServer.CanvasItemCreate();
		RenderingServer.CanvasItemSetMaterial(_arenaBorderCullingCanvasItem, _cullingMaterial.GetRid());
		RenderingServer.CanvasItemSetMaterial(_arenaMaskCullingCanvasItem, _cullingMaterial.GetRid());

		Rid _canvasItem = GetCanvasItem();
		RenderingServer.CanvasItemSetParent(_arenaBorderRenderingCanvasItem, _borderCanvas);
		RenderingServer.CanvasItemSetParent(_arenaBorderCullingCanvasItem, _borderCanvas);
		RenderingServer.CanvasItemSetParent(_arenaMaskRenderingCanvasItem, _maskCanvas);
		RenderingServer.CanvasItemSetParent(_arenaMaskCullingCanvasItem, _maskCanvas);
	}

	public override void _ExitTree()
	{
		if (_borderCanvas.IsValid) RenderingServer.FreeRid(_borderCanvas);
		if (_maskCanvas.IsValid) RenderingServer.FreeRid(_maskCanvas);

		if (_borderViewport.IsValid) RenderingServer.FreeRid(_borderViewport);
		if (_maskViewport.IsValid) RenderingServer.FreeRid(_maskViewport);

		if (_arenaBorderRenderingCanvasItem.IsValid) RenderingServer.FreeRid(_arenaBorderRenderingCanvasItem);
		if (_arenaBorderCullingCanvasItem.IsValid) RenderingServer.FreeRid(_arenaBorderCullingCanvasItem);
		if (_arenaMaskRenderingCanvasItem.IsValid) RenderingServer.FreeRid(_arenaMaskRenderingCanvasItem);
		if (_arenaMaskCullingCanvasItem.IsValid) RenderingServer.FreeRid(_arenaMaskCullingCanvasItem);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		_DrawArenas();
	}

	public override void _Draw()
	{
		Vector2I viewportSize = (Vector2I)GetViewportRect().Size;
		RenderingServer.ViewportSetSize(_borderViewport, viewportSize.X, viewportSize.Y);
		RenderingServer.ViewportSetSize(_maskViewport, viewportSize.X, viewportSize.Y);

		Rid _canvasItem = GetCanvasItem();
		RenderingServer.CanvasItemClear(_canvasItem);
		RenderingServer.CanvasItemAddTextureRect(_canvasItem, GetViewportRect(),
			_borderViewportTextureRid);
		RenderingServer.CanvasItemAddTextureRect(_canvasItem, GetViewportRect(),
				_maskViewportTextureRid);
	}

	private void _DrawArenas()
	{
		RenderingServer.CanvasItemClear(_arenaBorderRenderingCanvasItem);
		RenderingServer.CanvasItemClear(_arenaMaskRenderingCanvasItem);
		RenderingServer.CanvasItemClear(_arenaBorderCullingCanvasItem);
		RenderingServer.CanvasItemClear(_arenaMaskCullingCanvasItem);

		foreach (Node _child in GetChildren())
		{
			if (_child is BaseBattleArena arena)
			{
				if (!arena.Visible) continue;
				RenderingServer.CanvasItemAddSetTransform(_arenaBorderRenderingCanvasItem, arena.GetTransform());
				RenderingServer.CanvasItemAddSetTransform(_arenaMaskRenderingCanvasItem, arena.GetTransform());
				RenderingServer.CanvasItemAddSetTransform(_arenaBorderCullingCanvasItem, arena.GetTransform());
				RenderingServer.CanvasItemAddSetTransform(_arenaMaskCullingCanvasItem, arena.GetTransform());
				arena.DrawFrame(_arenaBorderRenderingCanvasItem, _arenaMaskRenderingCanvasItem,
					_arenaBorderCullingCanvasItem, _arenaMaskCullingCanvasItem);
			}
		}
	}

	public bool IsPointInArenas(Vector2 pos)
	{
		foreach (Node child in GetChildren())
		{
			if (child is not BattleArenaExpand arena) continue;
			var localPoint = arena.GlobalTransform.AffineInverse() * pos;
			var insideChild = arena.IsPointInArena(localPoint);
			if (insideChild) return true;
		}
		return false;
	}

	public bool IsLineInArenas(Vector2 from, Vector2 to)
	{
		foreach (Node child in GetChildren())
		{
			if (child is not BattleArenaExpand arena) continue;
			var insideChild = arena.IsSegmentInArena(
				arena.GlobalTransform.AffineInverse() * from,
				arena.GlobalTransform.AffineInverse() * to);
			if (insideChild) return true;
		}
		return false;
	}

	public Vector2 PushBackInside(Vector2 center, Vector2[] checkPoints, float tolerance = 0.001f)
	{
		var closestCenter = center;
		var minDistSq = float.MaxValue;

		foreach (Node child in GetChildren())
		{
			if (child is not BattleArenaExpand arena) continue;

			var tempCenter = center;
			var isValid = false;

			foreach (var offset in checkPoints)
			{
				var worldPoint = tempCenter + offset;
				var localPoint = arena.GlobalTransform.AffineInverse() * worldPoint;
				var insideChild = arena.IsPointInArena(localPoint);
				if (!insideChild)
				{
					var closestPoint = arena.GlobalTransform * arena.GetRecentPointInArena(localPoint);
					var pushDir = (closestPoint - worldPoint).Normalized();
					closestPoint += pushDir * tolerance;
					tempCenter += closestPoint - worldPoint;
					isValid = true;
				}
			}

			var distSq = center.DistanceSquaredTo(tempCenter);
			if (distSq < minDistSq && isValid)
			{
				minDistSq = distSq;
				closestCenter = tempCenter;
			}
		}
		return closestCenter;
	}

	public Rid GetMaskViewportTexture()
	{
		return _maskViewportTextureRid;
	}
}
