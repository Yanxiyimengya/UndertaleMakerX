import { UTMX , Vector2 } from "UTMX";
import {
    SANS_LEGS_TEXTURES,
    SANS_BODY_TEXTURES,
    SANS_BLUE_EYES_TEXTURES,
    SANS_DIALOG,
    SANS_FACE_TEXTURES,
    SANS_SLAM_BODY_TEXTURES
} from "enemies/sans-assets.js";
import {
    enableSansMenuBone,
    processSansBlueEyes,
    processSansSlam,
    resolveSansNextTurn,
    updateSansIdle,
    updateSansMenuBone
} from "enemies/sans-logic.js";

export default class EnemySans extends UTMX.Enemy
{
    static instance;
    
    legSprite = null;
    bodySprite = null;
    faceSprite = null;
    
    slamming = false;
    slammingDir = "down";
    slammingTimer = 0;
    slammingTimerLength = 0;
    
    blueEyes = false;
    blueEyesTimer = 0;
    blueEyesStatus = false;
    
    anim = 1;
    attackCount = 0;
    turnIndex = 4;
    attackCountPrev = -1;
    stage = 2;
    spared = false;
    menuBoneCanMoveIn = true;
    
    blueEyesTextures = SANS_BLUE_EYES_TEXTURES;
    faces = SANS_FACE_TEXTURES;
    bodies = SANS_BODY_TEXTURES;
    slamBody = SANS_SLAM_BODY_TEXTURES;
    
    constructor() 
    {
        super();
        EnemySans.instance = this;
    }
    
    start()
    {
        this.displayName = "Sans";
        this.defence = 99999999;
        this.missText = "MISS";
        this.position = new Vector2(0, -10);
        this.actions = ["检查"];
        
        this.legSprite = UTMX.Sprite.new();
        this.bodySprite = UTMX.Sprite.new();
        this.faceSprite = UTMX.Sprite.new();
        this.sweatSprite = UTMX.Sprite.new();
        
        this.sprite.addChild(this.legSprite);
        this.legSprite.addChild(this.bodySprite);
        this.bodySprite.addChild(this.faceSprite);
        this.faceSprite.addChild(this.sweatSprite);
        
        this.legSprite.textures = SANS_LEGS_TEXTURES[0];
        this.setBody(0);
        this.setFace(4);
        
        this.legSprite.position = new Vector2(0, 0);
        this.legSprite.scale = new Vector2(2, 2);
        this.legSprite.offset =  new Vector2(0, -23 / 2);
        
        this.bodySprite.position = new Vector2(0, -36);
        
        this.faceSprite.position = new Vector2(0, -22);

        this.sweatSprite.position = new Vector2(0, -10);
        this.sweatSprite.textures = "textures/sans/faces/spr_sansb_sweat_1.png";
        this.sweatSprite.visible = false;
        
        UTMX.battle.switchStatus(UTMX.battle.BattleStatus.ENEMY_DIALOGUE);
    }
    
    timer = 0;
    bodyOffset = Vector2.Zero;
    update(delta)
    {
        this.sweatSprite.offset = this.faceSprite.offset;
        updateSansIdle(this, delta);
        
        processSansSlam(this);
        processSansBlueEyes(this);
        updateSansMenuBone(this);
    }
    
    onHandleAction(action)
    {
        if (action == "检查")
        {
            if (this.canSpare) 
            {
                return "* 他不可能永远躲下去的。\n* 继续攻击。"
            }
            return "* Sans 1 攻击 1 防御\n* 最简单的敌人。\n* 只能造成 1 点伤害。";
        }
    }
    
    onGetNextTurn()
    {
        return resolveSansNextTurn(this);
    }
    
    onHandleAttack(status)
    {
        switch(status)
        {
            case UTMX.battle.AttackStatus.HIT : 
            {
                this.playMissAnim();
                this.attackCount += 1;
                if (this.canSpare)
                {
                    this.canSpare = false;
                    this.stage = 2;
                    this.attackCount = 1;
                    this.turnIndex = 0;
                }
                break;
            }
        }
    }

    onPlayerUsedItem()
    {
    }
    
    onSpare()
    {
        this.spared = true;
    }
    
    onDialogueStarting() {
        this.menuBoneCanMoveIn = false;
        let customProcessCmd = (cmd, args) => {
            if (cmd == "setFace") { this.setFace(args.value); return true; }
            if (cmd == "setBody") { this.setBody(args.value); return true; }
            if (cmd == "spare") {
                this.canSpare = true;
                if (!UTMX.audio.isBgmValid("BGM_SPARE")) {
                    UTMX.audio.playBgm("BGM_SPARE", "audios/bgm/mus_chokedup.ogg");
                }
                return true;
            }
            return false;
        };
        
        if (this.spared) 
        {
            UTMX.audio.stopBgm("BGM_SPARE");
             this.appendDialogue([
                SANS_DIALOG + "[setFace=4]...",
                SANS_DIALOG + "你要饶恕我？",
                SANS_DIALOG + "[setFace=1]终于。",
                SANS_DIALOG + "[setFace=3]伙伴。[wait=0.2]\n朋友。",
                SANS_DIALOG + "[setFace=4]我知道\n这有多困难...",
                SANS_DIALOG + "去下这个决定。",
                SANS_DIALOG + "那会让\n你做的这一切\n都从零开始。",
                SANS_DIALOG + "[setFace=1]希望你能明白",
                SANS_DIALOG + "[setFace=0]我并不希望\n让这一切\n白费...",
                SANS_DIALOG + "[setBody=1]...",
                SANS_DIALOG + "[setFace=3]来吧，[wait=0.2]朋友。" 
            ], new Vector2(56, -20), new Vector2(180, 90), customProcessCmd);
            return;
        }
        
        if (this.attackCountPrev == this.attackCount) { return; }
        if (this.stage == 1) {
            switch (this.attackCount) {
                case 0: {
                    this.setFace(4);
                    this.appendDialogue(SANS_DIALOG+"准备好了？", new Vector2(56, -20));
                    break;
                }
                case 1: {
                    this.setFace(3);
                    this.setBody(1);
                    this.appendDialogue(SANS_DIALOG+"怎么，[wait=0.2]\n你以为我会\n站在这儿\n乖乖挨打吗？", 
                        new Vector2(56, -20), new Vector2(180, 105));
                    break;
                }
                case 2: {
                    this.setFace(0);
                    this.setBody(0);
                    this.appendDialogue(SANS_DIALOG+"我们的报告显示，\n时空连续体中出现了\n巨大的异常。", 
                        new Vector2(56, -20), new Vector2(215, 90));
                    this.appendDialogue(SANS_DIALOG+"时间线左右横跳，\n停停走走...", 
                        new Vector2(56, -20), new Vector2(215, 90));
                    break;
                }
                case 3: {
                    this.setFace(4);
                    this.appendDialogue(SANS_DIALOG+"直到最后，\n万物归于沉寂。", 
                        new Vector2(56, -20), new Vector2(215, 90));
                    break;
                }
                case 4: {
                    this.setFace(4);
                    this.appendDialogue(SANS_DIALOG+"呵。呵。呵...", new Vector2(56, -20));
                    this.appendDialogue(SANS_DIALOG+"[setFace=5]这一切\n都是你搞的鬼，\n对吧？", 
                        new Vector2(56, -20), new Vector2(180, 90), customProcessCmd);
                    break;
                }
                case 5: {
                    this.setFace(1);
                    this.appendDialogue(SANS_DIALOG+"你不可能理解\n那种感受。", new Vector2(56, -20));
                    break;
                }
                case 6: {
                    this.setFace(4);
                    this.appendDialogue(SANS_DIALOG+"要知道，\n总有一天\n毫无征兆地...", 
                        new Vector2(56, -20), new Vector2(180, 90));
                    this.appendDialogue(SANS_DIALOG+"[setFace=9]一切事物都会\n归于零点。", 
                        new Vector2(56, -20), new Vector2(180, 90), customProcessCmd);
                    break;
                }
                case 7: {
                    this.setFace(9);
                    this.setBody(1);
                    this.appendDialogue(SANS_DIALOG+"说实话。[wait=0.1]\n我早就不再打算\n回去了...", 
                        new Vector2(56, -20), new Vector2(210, 90));
                    break;
                }
                case 8: {
                    this.setFace(1);
                    this.appendDialogue(SANS_DIALOG+"就连前往地表，\n对我来说也没什么\n吸引力了。", 
                        new Vector2(56, -20), new Vector2(210, 90));
                    break;
                }
                case 9: {
                    this.setFace(4);
                    this.appendDialogue(SANS_DIALOG+"因为即便\n我们出去了...", new Vector2(56, -20));
                    this.appendDialogue(SANS_DIALOG+"[setFace=9]最后也还是\n会不明不白地\n回到起点，\n不是吗？", 
                        new Vector2(56, -20), new Vector2(180, 105), customProcessCmd);
                    break;
                }
                case 10: {
                    this.setFace(1);
                    this.appendDialogue(SANS_DIALOG+"老实说...", new Vector2(56, -20));
                    this.appendDialogue(SANS_DIALOG+"[setFace=4]这让我很难\n打起精神。", 
                        new Vector2(56, -20), new Vector2(180, 90), customProcessCmd);
                    break;
                }
                case 11: {
                    this.setFace(0);
                    this.setBody(0);
                    this.appendDialogue(SANS_DIALOG+"[setFace=3]或者这只是我\n在为了自己的\n懒惰找借口？", 
                        new Vector2(56, -20), new Vector2(180, 90), customProcessCmd);
                    this.appendDialogue(SANS_DIALOG+"[setFace=1]管他呢。", 
                        new Vector2(56, -20), new Vector2(180, 90), customProcessCmd);
                    break;
                }
                case 12: {
                    this.setFace(3);
                    this.setBody(1);
                    this.appendDialogue(SANS_DIALOG+"我知道的就只有...[wait=0.2]\n看着接下来要\n发生的一切...", new Vector2(56, -20));
                    this.appendDialogue(SANS_DIALOG+"[setFace=9]我没办法\n再漠不关心。", 
                        new Vector2(56, -20), new Vector2(180, 90), customProcessCmd);
                    break;
                }
                case 13: {
                    if (UTMX.audio.isBgmValid("BGM")) {
                        UTMX.audio.setBgmPaused("BGM", true);
                    }
                    this.sweatSprite.visible = true;
                    this.setFace(9);
                    this.setBody(0);
                    this.appendDialogue(
                        [
                        SANS_DIALOG + "[setFace=9]呃...[wait=0.2]\n话说回来。",
                        SANS_DIALOG + "[setFace=1]你，[wait=0.1]呃，[wait=0.2]\n真的很喜欢摆动\n那个东西，[wait=0.1]huh？",
                        SANS_DIALOG + "[setFace=4][spare]听着...",
                        SANS_DIALOG + "我知道\n你不曾回应我，[wait=0.2]\n但...[wait=0.2]在某处。",
                        SANS_DIALOG + "[setFace=0]还有一点\n人性的善良\n在你的心中。",
                        SANS_DIALOG + "[setFace=4]记得曾一度\n想做对的事。[wait=0.2]\n",
                        SANS_DIALOG + "[setFace=4]那个可能在\n某个时候的[wait=0.2]\n某个人...",
                        SANS_DIALOG + "[setFace=1]也许我们\n还能成为...[wait=0.3]\n[wait=0.2]朋友？",
                        SANS_DIALOG + "[setFace=3]来吧，[wait=0.1]朋友。[wait=0.2]\n你还记得我吗？",
                        SANS_DIALOG + "[setFace=9]拜托，[wait=0.2]如果你有\n在听我说话...",
                        SANS_DIALOG + "我们忘了\n这一切，[wait=0.1]好吗？",
                        SANS_DIALOG + "[setFace=3]只要放下\n你的武器，[wait=0.2]\n然后...[wait=0.2]嗯。",
                        SANS_DIALOG + "[setFace=4]我的任务\n也会轻松许多。"
                        ],
                        new Vector2(56, -20), new Vector2(180, 90), customProcessCmd
                    );
                    break;
                }
            }
        } else if (this.stage == 2) {
            switch (this.attackCount) {
                case 1: {
                    UTMX.audio.stopBgm("BGM_SPARE");
                    this.appendDialogue(SANS_DIALOG+"[setFace=3][setBody=1]得了，\n[wait=0.2]试一试\n还是值的。", 
                        new Vector2(56, -20), new Vector2(180, 90), customProcessCmd);
                    this.appendDialogue(SANS_DIALOG+"[setFace=5]看来你更喜欢\n吃点苦头。\n[wait=0.2]嗯？", 
                        new Vector2(56, -20), new Vector2(180, 90), customProcessCmd);
                    break;
                }
                case 2: {
                    this.appendDialogue([
                        SANS_DIALOG+"[setFace=4]听上去很奇怪\n[wait=0.2]但在这之前\n我还暗想着\n我们能成为朋友。", 
                        SANS_DIALOG+"[setFace=3]我一直认为\n那个异常者\n做出这种事来\n是因为不幸福。",
                        SANS_DIALOG+"而当这家伙的愿望\n被满足时，[wait=0.2]\n就会浪子回头。"
                    ], new Vector2(56, -20), new Vector2(195, 105), customProcessCmd);
                    break;
                }
                case 3: {
                    this.appendDialogue([
                        SANS_DIALOG+"[setFace=3]或许那个家伙\n只是想要...\n我不理解。", 
                        SANS_DIALOG+"[setFace=3][setBody=1]几份美食，\n几则烂笑话，\n还有一些朋友？"
                    ], new Vector2(56, -20), new Vector2(195, 105), customProcessCmd);
                    break;
                }
                case 4: {
                    this.appendDialogue([
                        SANS_DIALOG+"[setFace=4]但那太荒谬了，\n不是吗？", 
                        SANS_DIALOG+"[setFace=5]没错，你是\n那种永远都不会\n感到幸福的人。",
                    ], new Vector2(56, -20), new Vector2(195, 105), customProcessCmd);
                    break;
                }
                case 5: {
                    this.appendDialogue([
                        SANS_DIALOG+"[setFace=5]你总是会\n不停消耗着时间线，\n直到...", 
                        SANS_DIALOG+"[setFace=4]算了。",
                        SANS_DIALOG+"[setBody=1]嘿。",
                        SANS_DIALOG+"[setFace=3]听听我的忠告吧，[wait=0.2]\n小子。",
                        SANS_DIALOG+"总有一天...",
                        SANS_DIALOG+"你得学会\n知难而退"
                    ], new Vector2(56, -20), new Vector2(195, 105), customProcessCmd);
                    break;
                }
            }
        }
    }
    
    onTurnStarting()
    {
        this.attackCountPrev = this.attackCount;
        if (this.stage == 1)
        {
            if (this.attackCount != 0)
            {
                this.setFace(0);
                this.setBody(0);
            }
        }
        
        if (this.stage == 2)
        {
            this.setFace(0);
            this.setBody(0);
        }
    }
    
    onTurnEnd()
    {
        this.turnIndex += 1;
        if (this.stage == 2)
        {
            if (this.turnIndex >= 5) {
                enableSansMenuBone(this, true);
            }
            this.menuBoneCanMoveIn = true;
        }
    }

    menuBone = null;
    setFace(index)
    {
        this.faceSprite.textures = this.faces[index];
    }
    setBody(index)
    {
        this.bodySprite.textures = this.bodies[index];
    }
    setBlueEyes(enabled = true)
    {
        this.blueEyes = enabled;
        this.blueEyesTimer = 0;
        this.blueEyesStatus = false;
    }
    setSlam(dir = "down", len = 0)
    {
        this.slamming = true;
        this.slammingTimer = 0;
        this.slammingTimerLength = len;
        this.slammingDir = dir;
        this.bodySprite.offset = Vector2.Zero;
        this.faceSprite.offset =  Vector2.Zero;
    }
    
    missAnimTween = null;
    playMissAnim()
    {
        var missAnimTween = UTMX.tween.createTween();
        let posX = this.legSprite.position.x;
        missAnimTween.addTweenProperty(this.legSprite, "position", 
            new Vector2(posX - 100, this.legSprite.position.y), 0.5).trans(UTMX.tween.TransitionType.Sine).ease(UTMX.tween.EaseType.Out);
        missAnimTween.addTweenProperty(this.legSprite, "position", 
            new Vector2(posX, this.legSprite.position.y), 0.4).ease(UTMX.tween.TransitionType.Sine).ease(UTMX.tween.EaseType.Out).delay(0.6);
    }
}
