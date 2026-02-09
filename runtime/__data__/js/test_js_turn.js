import { UTMX, Vector2, Color } from "UTMX";

export default class MyCustomTurn extends UTMX.BattleTurn {
	constructor() {
		super();
		this.soulInitPosition = new Vector2(0, UTMX.battle.arena.getMainArena().size.y / 2 - 9);
		this.jumping = false;
		this.moveSpeed = 130.0;
		this.gravity = 300.0;
		this.jumpSpeed = 0.0;
	}

	onTurnInit() {
		UTMX.battle.soul.sprite.color = Color.Blue;
	}

	onTurnStart() {
		UTMX.battle.soul.movable = false; // 禁用移动，实现自定义控制器
	}

	onTurnUpdate(delta) {
		let moveSpeed = new Vector2(0, 0);
		
		// 检测是否在地面
		if (UTMX.battle.soul.isOnArenaFloor()) {
			if (UTMX.input.isActionHeld("up") && !this.jumping) {
				this.jumping = true;
				this.jumpSpeed = -200; // 向上的初始速度
				moveSpeed.y = this.jumpSpeed;
			} else {
				this.jumping = false;
				this.jumpSpeed = 0.0;
			}
		}
		else
		{
			this.jumpSpeed += this.gravity * delta;
			moveSpeed.y = this.jumpSpeed;
		}
		
		// 触顶或松开上键，停止跳跃
		if (this.jumping && moveSpeed.y < 0) {
			if (UTMX.input.isActionReleased("up") || UTMX.battle.soul.isOnArenaCeiling()) {
				this.jumping = false;
				this.jumpSpeed = 0.0;
			}
		}
		
		moveSpeed.x = UTMX.input.getActionAxis("left", "right") * this.moveSpeed; // 处理水平移动
		let soulPosition = new Vector2().copy(UTMX.battle.soul.position);
		UTMX.battle.soul.tryMoveTo(
			soulPosition.add(moveSpeed.multiply(delta).rotated(UTMX.battle.soul.rotation * Math.PI / 180)));
	}
	
	onTurnEnd() {}
}
