import { UTMX } from "UTMX";

// Undertale 经典 Papyrus 敌人 - 严格适配 UtmxBaseEnemy 基类
export default class PapyrusEnemy extends UTMX.Enemy {
    constructor() {
        super();
        // 核心属性：贴合原作设定，热血中二的骷髅兄弟，面板数值适中
        this.maxHp = 100;        // 比Sans血厚，符合原作“认真战斗”的设定
        this.hp = 100;           // 初始满血
        this.displayName = "Papyrus";// 敌人显示名（原作经典命名）
        this.attack = 8;         // 面板攻击适中，骨头攻击有实际伤害
        this.defence = 2;        // 防御较低，符合“战斗经验不足”的设定
        this.allowSpare = true;  // 允许被饶恕（Papyrus本心不坏）
        this.canSpare = false;   // 初始无法饶恕，需触发“手下留情”条件解锁
        this.missText = "NYEH HEH HEH!!";// 攻击打空时的经典口头禅

        // 玩家可交互动作列表（贴合Papyrus的性格，无恶意动作）
        this.actions = ["攻击", "夸赞", "饶恕", "检查"];
    }

    /**
     * 基类要求 - 处理玩家交互动作
     * @param {string} action - 玩家选择的动作（对应this.actions）
     * @returns {Array<string>} 战斗反馈文本
     */
    onHandleAction(action) {
        switch (action) {
            case "攻击":
                return this._onAttack();  // 玩家攻击：触发骨头反击+傲娇吐槽
            case "夸赞":
                return this._onPraise();  // 玩家夸赞：核心解锁饶恕的条件（贴合原作）
            case "饶恕":
                return this._onSpare();   // 玩家饶恕：未解锁拒绝，解锁后触发剧情
            case "检查":
                return this._onCheck();   // 玩家检查：还原原作经典检查信息
            default:
                return ["...", "这个动作对伟大的Papyrus无效！"]; // 未知动作兜底
        }
    }

    /**
     * 基类要求 - 敌人下一回合行为
     * 还原Papyrus的回合逻辑：经典口头禅、骨头攻击提示、中二台词
     */
    onGetNextTurn() {
        // Papyrus的经典中二台词，随机触发，贴合原作性格
        const papyrusLines = [
            "NYEH HEH HEH!! 准备好迎接伟大的Papyrus的攻击吧！",
            "我的骨头攻击可是无人能躲的！",
            "你以为能轻易打败我吗？太天真了！",
            "[play_sound path=papyrus_laugh.mp3]"]; // 笑声音效，可替换为你的资源路径
        // 随机选一句台词，追加到对话（基类方法）
        const randomLine = papyrusLines[Math.floor(Math.random() * papyrusLines.length)];
        this.appendDialogue([randomLine]);

        // 解锁饶恕后，Papyrus会手下留情，提示玩家可以饶恕他
        if (this.canSpare) {
            this.appendDialogue(["...好吧，我的攻击会手下留情的！", "你可以尝试饶恕伟大的Papyrus！"]);
        }
    }

    /**
     * 基类方法 - 处理玩家攻击（可选实现，保留空方法也可）
     * @param {any} status - 攻击状态（框架传参，可根据需求使用）
     */
    onHandleAttack(status) {
        // 被攻击后触发傲娇吐槽，符合Papyrus的性格
        this.appendDialogue(["唔！你居然真的攻击我！", "太不讲武德了！"]);
        // 若已解锁饶恕，被攻击后会有点委屈
        if (this.canSpare) {
            this.appendDialogue(["我都手下留情了，你还攻击我..."]);
        }
    }

    /**
     * 基类方法 - 被饶恕时的回调（可选实现）
     * 饶恕成功后触发的额外剧情
     */
    onSpare() {
        this.appendDialogue([
            "NYEH HEH HEH！不愧是我的对手！",
            "你通过了伟大的Papyrus的考验！",
            "[play_sound path=papyrus_victory.mp3]",
            "[stop_bgm]"]);
        // 调用框架战斗管理器结束战斗（适配UTMX框架）
    }

    // 私有方法：玩家选择「攻击」时的逻辑
    _onAttack() {
        this.appendDialogue([
            "哦？你居然敢攻击伟大的Papyrus！",
            "接招吧！骨头风暴！",
            "[play_sound path=bone_attack2.mp3]"]); // Papyrus的骨头攻击音效
        // 原作设定：Papyrus的攻击有伤害，但不会下死手
        return ["Papyrus发起了骨头攻击！", "你受到了轻微的伤害！"];
    }

    // 私有方法：玩家选择「夸赞」时的逻辑（解锁饶恕的核心条件，贴合原作）
    _onPraise() {
        // 未解锁饶恕时：被夸赞后傲娇又开心，直接解锁饶恕
        if (!this.canSpare) {
            this.appendDialogue([
                "哦？你居然夸赞伟大的Papyrus！",
                "眼光不错！看来你是个值得认可的对手！"]);
            this.canSpare = true; // 核心：夸赞后直接解锁饶恕
            return ["Papyrus的脸红了！", "他的战斗意志降低了！"];
        }
        // 已解锁饶恕后：被反复夸赞会更傲娇
        this.appendDialogue(["哼！这是当然的！伟大的Papyrus本来就很厉害！"]);
        return ["Papyrus变得更得意了！", "他完全不想攻击你了！"];
    }

    // 私有方法：玩家选择「饶恕」时的逻辑
    _onSpare() {
        if (this.canSpare) {
            // 解锁饶恕后：触发饶恕剧情，调用基类onSpare方法
            this.onSpare();
            return ["你饶恕了Papyrus！", "伟大的Papyrus认可了你的实力！"];
        } else {
            // 未解锁饶恕时：Papyrus拒绝，坚持要和你战斗
            this.appendDialogue([
                "现在还想饶恕我？不行！",
                "伟大的Papyrus还没展示完我的实力呢！",
                "NYEH HEH HEH!!"]);
            return ["Papyrus拒绝了你的饶恕！", "他的骨头攻击变得更猛烈了！"];
        }
    }

    // 私有方法：玩家选择「检查」时的逻辑（还原原作经典检查信息）
    _onCheck() {
        // 一字不差还原Undertale原作中Papyrus的检查描述
        return [
            "Papyrus - 骷髅怪物",
            "攻击 8 防御 2",
            "Sans的弟弟，\n渴望成为皇家守卫队队员",
            "极其中二，喜欢讲冷笑话和做意大利面。"
        ];
    }
}