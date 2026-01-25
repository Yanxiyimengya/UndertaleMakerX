using Godot;
using System;

[Tool]
[GlobalClass]
public partial class BattleArenaGroup : Node2D
{
	public Rid MainCanvasItem;
	public Rid MainCanvas;
	
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

	public Transform2D CameraTransform;
	public Transform2D CameraTransformInverse;

	static BattleArenaGroup()  
	{
		_cullingMaterial = new CanvasItemMaterial();  
		_cullingMaterial.BlendMode = CanvasItemMaterial.BlendModeEnum.Sub;  
	}
	public override void _EnterTree()
	{
		_borderCanvas = RenderingServer.CanvasCreate();
		_maskCanvas = RenderingServer.CanvasCreate();
		MainCanvas = RenderingServer.CanvasCreate();

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

		RenderingServer.ViewportAttachCanvas(GetViewport().GetViewportRid(), MainCanvas);

		_borderViewportTextureRid = RenderingServer.ViewportGetTexture(_borderViewport);
		_maskViewportTextureRid = RenderingServer.ViewportGetTexture(_maskViewport);

		MainCanvasItem = RenderingServer.CanvasItemCreate();
		_arenaBorderRenderingCanvasItem = RenderingServer.CanvasItemCreate();
		_arenaBorderCullingCanvasItem = RenderingServer.CanvasItemCreate();
		_arenaMaskRenderingCanvasItem = RenderingServer.CanvasItemCreate();
		_arenaMaskCullingCanvasItem = RenderingServer.CanvasItemCreate();

		RenderingServer.CanvasItemSetParent(MainCanvasItem, GetCanvas());
		RenderingServer.CanvasItemSetParent(_arenaBorderRenderingCanvasItem, _borderCanvas);
		RenderingServer.CanvasItemSetParent(_arenaBorderCullingCanvasItem, _borderCanvas);
		RenderingServer.CanvasItemSetParent(_arenaMaskRenderingCanvasItem, _maskCanvas);
		RenderingServer.CanvasItemSetParent(_arenaMaskCullingCanvasItem, _maskCanvas);

		_cullingMaterial.BlendMode = CanvasItemMaterial.BlendModeEnum.Sub;
		RenderingServer.CanvasItemSetMaterial(_arenaBorderCullingCanvasItem, _cullingMaterial.GetRid());
		RenderingServer.CanvasItemSetMaterial(_arenaMaskCullingCanvasItem, _cullingMaterial.GetRid());
	}

	public override void _ExitTree()
	{
		if (MainCanvasItem.IsValid) RenderingServer.FreeRid(MainCanvasItem);
		if (_arenaBorderRenderingCanvasItem.IsValid) RenderingServer.FreeRid(_arenaBorderRenderingCanvasItem);
		if (_arenaBorderCullingCanvasItem.IsValid) RenderingServer.FreeRid(_arenaBorderCullingCanvasItem);
		if (_arenaMaskRenderingCanvasItem.IsValid) RenderingServer.FreeRid(_arenaMaskRenderingCanvasItem);
		if (_arenaMaskCullingCanvasItem.IsValid) RenderingServer.FreeRid(_arenaMaskCullingCanvasItem);
		if (_borderCanvas.IsValid) RenderingServer.FreeRid(_borderCanvas);
		if (_maskCanvas.IsValid) RenderingServer.FreeRid(_maskCanvas);
		if (MainCanvas.IsValid) RenderingServer.FreeRid(MainCanvas);
		if (_borderViewport.IsValid) RenderingServer.FreeRid(_borderViewport);
		if (_maskViewport.IsValid) RenderingServer.FreeRid(_maskViewport);
	}

	public override void _Ready()
	{
		QueueRedraw();
	}

	public override void _Process(double delta)
	{
		GetCameraTransform();
		RenderingServer.CanvasItemSetTransform(MainCanvasItem, CameraTransform);
		if (IsInsideTree() && Visible)
		{
			RenderingServer.CanvasItemClear(_arenaBorderRenderingCanvasItem);
			RenderingServer.CanvasItemClear(_arenaMaskRenderingCanvasItem);
			RenderingServer.CanvasItemClear(_arenaBorderCullingCanvasItem);
			RenderingServer.CanvasItemClear(_arenaMaskCullingCanvasItem);
			bool _requireRedraw = false;
			foreach (Node _child in GetChildren())
			{
				if (_child is BaseBattleArena arena)
				{
					if (!arena.Visible) continue;
					Transform2D arenaTransform2D = CameraTransformInverse * arena.GetTransform();
					RenderingServer.CanvasItemAddSetTransform(_arenaBorderRenderingCanvasItem, arenaTransform2D);
					RenderingServer.CanvasItemAddSetTransform(_arenaMaskRenderingCanvasItem, arenaTransform2D);
					RenderingServer.CanvasItemAddSetTransform(_arenaBorderCullingCanvasItem, arenaTransform2D);
					RenderingServer.CanvasItemAddSetTransform(_arenaMaskCullingCanvasItem, arenaTransform2D);
					arena.DrawFrame(_arenaBorderRenderingCanvasItem, _arenaMaskRenderingCanvasItem,
						_arenaBorderCullingCanvasItem, _arenaMaskCullingCanvasItem);
					if (arena.IsDirty)
					{
						arena.IsDirty = false;
						_requireRedraw = true;
					}
				}
			}
			if (_requireRedraw)
			{
				RenderingServer.ViewportSetActive(GetViewport().GetViewportRid(), false);
				RenderingServer.ViewportSetActive(GetViewport().GetViewportRid(), true);
			}
		}
	}

	public override void _Draw()
	{
		RenderingServer.CanvasItemClear(MainCanvasItem);
		Vector2I viewportSize = (Vector2I)GetViewport().GetVisibleRect().Size;
		RenderingServer.ViewportSetSize(_borderViewport, viewportSize.X, viewportSize.Y);
		RenderingServer.ViewportSetSize(_maskViewport, viewportSize.X, viewportSize.Y);
		RenderingServer.CanvasItemAddTextureRect(MainCanvasItem, new Rect2(Vector2.Zero, viewportSize),
			_borderViewportTextureRid);
		RenderingServer.CanvasItemAddTextureRect(MainCanvasItem, new Rect2(Vector2.Zero, viewportSize),
				_maskViewportTextureRid);
	}

	public Vector2 GetScreenTopLeftPosition()  
	{  
		if (GetViewport().GetCamera2D() is Camera2D camera)  
		{  
			Vector2 screenSize = GetViewport().GetVisibleRect().Size; 
			Transform2D cameraTransform = camera.GetTransform();
			Vector2 halfScreenOffset = -screenSize * 0.5F * (Vector2.One / camera.Zoom);
			cameraTransform.Origin = Vector2.Zero;
			return camera.GetScreenCenterPosition() + cameraTransform * halfScreenOffset;  
		}  
		return Vector2.Zero;  
	}
	
	public void GetCameraTransform()
	{
		CameraTransform = Transform2D.Identity;
		CameraTransformInverse = CameraTransform.Inverse();
		Viewport viewport = GetViewport();
		if (viewport.GetCamera2D() is Camera2D camera)
		{
			Rect2 visibleRect = viewport.GetVisibleRect();
			Vector2 screenSize = visibleRect.Size;
			Vector2 zoom = new Vector2(Math.Abs(camera.Zoom.X), Math.Abs(camera.Zoom.Y));
			Vector2 zoomScale = new Vector2(1f / zoom.X, 1f / zoom.Y);
			float rotation = camera.IgnoreRotation ? 0F : camera.GlobalRotation;
			CameraTransform = new Transform2D(
				rotation,
				zoomScale,
				0f,
				(camera.AnchorMode == Camera2D.AnchorModeEnum.FixedTopLeft) ? 
					camera.GlobalPosition : GetScreenTopLeftPosition()
			);
			CameraTransformInverse = CameraTransform.AffineInverse();
		}
	}

	public bool IsPointInArenas(Vector2 pos)
	{
		foreach (Node child in GetChildren())
		{
			if (child is not BattleArenaExpand arena || !arena.Enabled) continue;
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
			if (child is not BattleArenaExpand arena || !arena.Enabled) continue;
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
			if (child is not BattleArenaExpand arena || !arena.Enabled) continue;

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
