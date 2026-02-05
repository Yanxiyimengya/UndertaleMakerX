import { UTMX , Vector2} from "UTMX";
import MyBattleTurn from "./test_js_turn.js";

export default class PapyrusEnemy extends UTMX.Enemy {
	constructor() {
		super();
		UTMX.debug.print("PapyrusEnemy: Constructor Started"); // 1. 检查构造开始

		this.displayName = "Papyrus";
		this.hp = 10;
		this.maxHp = 10;
		this.attack = 20;
		this.defense = 5;

		// 这里是之前报错可能相关的地方，检查 Vector2 是否成功创建
		this.centerPosition = new Vector2(0.0, -70.0);
		UTMX.debug.print("PapyrusEnemy: CenterPosition set to " + JSON.stringify(this.centerPosition)); 

		UTMX.debug.print("PapyrusEnemy: Constructor Finished"); // 2. 检查构造结束
	}

	onSpare() {
		UTMX.debug.print("PapyrusEnemy: onSpare called");
	}

	onHandleAction(action) {
		UTMX.debug.print("PapyrusEnemy: onHandleAction called with action: " + action);
	}

	onHandleAttack(status) {
		UTMX.debug.print("PapyrusEnemy: onHandleAttack called");
	}

	onBattleStart() {
		UTMX.debug.print("PapyrusEnemy: onBattleStart called");
		// 如果这里没有打印，说明在战斗开始前对象就已经崩溃或未加载
	}

	onBattleEnd() {
		UTMX.debug.print("PapyrusEnemy: onBattleEnd called");
	}

	onDialogueStarting() {
		UTMX.debug.print("PapyrusEnemy: onDialogueStarting called");
		try {
			this.appendDialogue(["嘻嘻嘻"]); // 假设 appendDialogue 是全局或父类方法
			UTMX.debug.print("PapyrusEnemy: Dialogue appended");
		} catch (e) {
			UTMX.debug.print("PapyrusEnemy: Error inside onDialogueStarting: " + e);
		}
	}

	onDialogueEnding() {
		UTMX.debug.print("PapyrusEnemy: onDialogueEnding called");
	}

	onGetNextTurn() {
		UTMX.debug.print("PapyrusEnemy: onGetNextTurn called");
		let turn = new MyBattleTurn();
		
		if (!turn) {
			UTMX.debug.print("ERROR: MyBattleTurn created null/undefined turn object!");
		} else {
			UTMX.debug.print("PapyrusEnemy: Returning new turn object");
		}
		
		return turn;
	}
}
