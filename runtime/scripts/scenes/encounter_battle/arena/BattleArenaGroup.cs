using Godot;
using System;
using System.ComponentModel.Design;

[Tool]
[GlobalClass]
public partial class BattleArenaGroup : Node2D
{
	public int EnabledArenaCount;
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
	private CanvasItemMaterial _cullingMaterial;

	public Transform2D CameraTransform;
	public Transform2D CameraTransformInverse;
	
	public void Initialize()
	{
		_cullingMaterial = new CanvasItemMaterial();
		_cullingMaterial.BlendMode = CanvasItemMaterial.BlendModeEnum.Sub;
		
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

		MainCanvasItem = RenderingServer.CanvasItemCreate();
		_arenaBorderRenderingCanvasItem = RenderingServer.CanvasItemCreate();
		_arenaBorderCullingCanvasItem = RenderingServer.CanvasItemCreate();
		_arenaMaskRenderingCanvasItem = RenderingServer.CanvasItemCreate();
		_arenaMaskCullingCanvasItem = RenderingServer.CanvasItemCreate();

		RenderingServer.CanvasItemSetMaterial(_arenaBorderCullingCanvasItem, _cullingMaterial.GetRid());
		RenderingServer.CanvasItemSetMaterial(_arenaMaskCullingCanvasItem, _cullingMaterial.GetRid());

		RenderingServer.ViewportAttachCanvas(GetViewport().GetViewportRid(), MainCanvas);
		_borderViewportTextureRid = RenderingServer.ViewportGetTexture(_borderViewport);
		_maskViewportTextureRid = RenderingServer.ViewportGetTexture(_maskViewport);
		RenderingServer.CanvasItemSetParent(MainCanvasItem, GetCanvas());
		RenderingServer.CanvasItemSetParent(_arenaBorderRenderingCanvasItem, _borderCanvas);
		RenderingServer.CanvasItemSetParent(_arenaBorderCullingCanvasItem, _borderCanvas);
		RenderingServer.CanvasItemSetParent(_arenaMaskRenderingCanvasItem, _maskCanvas);
		RenderingServer.CanvasItemSetParent(_arenaMaskCullingCanvasItem, _maskCanvas);
	}

	public void Uninitialize()
	{
		if (_borderViewport.IsValid) RenderingServer.FreeRid(_borderViewport);
		if (_maskViewport.IsValid) RenderingServer.FreeRid(_maskViewport);
		
		if (_borderCanvas.IsValid) RenderingServer.FreeRid(_borderCanvas);
		if (_maskCanvas.IsValid) RenderingServer.FreeRid(_maskCanvas);
		if (MainCanvas.IsValid) RenderingServer.FreeRid(MainCanvas);

		if (MainCanvasItem.IsValid) RenderingServer.FreeRid(MainCanvasItem);
		if (_arenaBorderRenderingCanvasItem.IsValid) RenderingServer.FreeRid(_arenaBorderRenderingCanvasItem);
		if (_arenaBorderCullingCanvasItem.IsValid) RenderingServer.FreeRid(_arenaBorderCullingCanvasItem);
		if (_arenaMaskRenderingCanvasItem.IsValid) RenderingServer.FreeRid(_arenaMaskRenderingCanvasItem);
		if (_arenaMaskCullingCanvasItem.IsValid) RenderingServer.FreeRid(_arenaMaskCullingCanvasItem);
	}

	public override void _EnterTree()
	{
		Initialize();
		QueueRedraw();
	}

	public override void _ExitTree()
	{
		Uninitialize();
	}

	public override void _Process(double delta)
	{
		if (! Engine.IsEditorHint()) 
			GetCameraTransform();
		if (IsInsideTree() && IsVisibleInTree())
		{
			RenderingServer.CanvasItemSetTransform(MainCanvasItem, CameraTransform);
			RenderingServer.CanvasItemSetZIndex(MainCanvasItem, ZIndex);
			RenderingServer.CanvasItemClear(_arenaBorderRenderingCanvasItem);
			RenderingServer.CanvasItemClear(_arenaMaskRenderingCanvasItem);
			RenderingServer.CanvasItemClear(_arenaBorderCullingCanvasItem);
			RenderingServer.CanvasItemClear(_arenaMaskCullingCanvasItem);

			EnabledArenaCount = 0;
			foreach (Node _child in GetChildren())
			{
				if (_child is BaseBattleArena arena)
				{
					if (!arena.Enabled) continue;
					EnabledArenaCount += 1;
					Transform2D arenaTransform2D = CameraTransformInverse * arena.GetTransform();
					RenderingServer.CanvasItemAddSetTransform(_arenaBorderRenderingCanvasItem, arenaTransform2D);
					RenderingServer.CanvasItemAddSetTransform(_arenaMaskRenderingCanvasItem, arenaTransform2D);
					RenderingServer.CanvasItemAddSetTransform(_arenaBorderCullingCanvasItem, arenaTransform2D);
					RenderingServer.CanvasItemAddSetTransform(_arenaMaskCullingCanvasItem, arenaTransform2D);
					arena.DrawFrame(_arenaBorderRenderingCanvasItem, _arenaMaskRenderingCanvasItem,
					_arenaBorderCullingCanvasItem, _arenaMaskCullingCanvasItem);
				}
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
		RenderingServer.ViewportSetActive(GetViewport().GetViewportRid(), false);
		RenderingServer.ViewportSetActive(GetViewport().GetViewportRid(), true);
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
		if (EnabledArenaCount == 0) return false;
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
		if (EnabledArenaCount == 0) return false;
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
		if (EnabledArenaCount == 0) return center;
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
