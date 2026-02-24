import { UTMX, Vector2 } from "UTMX";

const STAGE1_TURN_MAP = {
    1: "turns/stage1/sans-turn1.js",
    2: "turns/stage1/sans-turn2.js",
    3: "turns/stage1/sans-turn3.js",
    4: "turns/stage1/sans-turn4.js",
    5: "turns/stage1/sans-turn5.js",
    6: "turns/stage1/sans-turn6.js",
    7: "turns/stage1/sans-turn7.js",
    8: "turns/stage1/sans-turn8.js",
    9: "turns/stage1/sans-turn9.js",
    10: "turns/stage1/sans-turn1.js",
    11: "turns/stage1/sans-turn10.js",
    12: "turns/stage1/sans-turn3.js"
};

const STAGE2_TURN_MAP = {
    0: "turns/stage2/sans-turn1.js",
    1: "turns/stage2/sans-turn2.js",
    2: "turns/stage2/sans-turn3.js",
    3: "turns/stage2/sans-turn4.js",
    4: "turns/stage2/sans-turn5.js",
    5: "turns/stage2/sans-turn6.js",
    6: "turns/stage2/sans-turn7.js",
    7: "turns/stage2/sans-turn8.js",
    8: "turns/stage2/sans-turn9.js",
};

const MENU_BONE_ANIM_TEMPLATE = {
    speed: 0,
    startX: -5,
    hideX: -20,
    y: 285,
    startSpeed: 2.5,
    holdFrames: 20,
    holdTimer: 0
};

const SLAM_DIR_MAP = {
    down: 0,
    up: 1,
    right: 2,
    left: 3
};

const SLAM_FRAMES = {
    0: {
        1: { texture: { kind: "slam", index: 0 }, bodyOffset: [2, -10], faceOffset: [0, -2] },
        10: { texture: { kind: "slam", index: 1 }, bodyOffset: [3, -3], faceOffset: [0, 0] },
        18: { texture: { kind: "slam", index: 2 }, bodyOffset: [3, 5], faceOffset: [0, 3] }
    },
    1: {
        1: { texture: { kind: "slam", index: 2 }, bodyOffset: [3, 5], faceOffset: [0, 3] },
        10: { texture: { kind: "slam", index: 1 }, bodyOffset: [3, -3], faceOffset: [0, 0] },
        18: { texture: { kind: "slam", index: 0 }, bodyOffset: [2, -10], faceOffset: [0, -2] }
    },
    // Keep frame results aligned with the current in-file switch/fallthrough behavior.
    2: {
        1: { texture: { kind: "body", index: 0 }, bodyOffset: [-2, 0], faceOffset: [-2, 0] },
        5: { texture: { kind: "body", index: 0 }, bodyOffset: [-3, 0], faceOffset: [-4, 0] },
        12: { texture: { kind: "slam", index: 3 }, bodyOffset: [14, 0], faceOffset: [3, 0] },
        18: { texture: { kind: "slam", index: 4 }, bodyOffset: [12, 0], faceOffset: [1, 0] }
    },
    3: {
        // 宸︿晶 = 鍙充晶(2)鍏抽敭甯у€掓斁 + 姘村钩闀滃儚
        1: { texture: { kind: "slam", index: 4 }, bodyOffset: [12, 0], faceOffset: [1, 0] },
        5: { texture: { kind: "slam", index: 3 }, bodyOffset: [14, 0], faceOffset: [3, 0] },
        12: { texture: { kind: "body", index: 0 }, bodyOffset: [-3, 0], faceOffset: [-4, 0] },
        18: { texture: { kind: "body", index: 0 }, bodyOffset: [-2, 0], faceOffset: [-2, 0] }
    }
};

function toVector2(pair) {
    return new Vector2(pair[0], pair[1]);
}

function normalizeSlamDir(dir) {
    if (typeof dir === "number") {
        if (dir < 0) return 0;
        if (dir > 3) return 3;
        return Math.floor(dir);
    }
    const key = String(dir).trim().toLowerCase();
    return SLAM_DIR_MAP[key] ?? 0;
}

function applySlamFrame(enemy, frame) {
    if (frame.texture != null) {
        if (frame.texture.kind === "slam") {
            enemy.bodySprite.textures = enemy.slamBody[frame.texture.index];
        } else if (frame.texture.kind === "body") {
            enemy.bodySprite.textures = enemy.bodies[frame.texture.index];
        }
    }

    enemy.bodySprite.offset = toVector2(frame.bodyOffset);
    enemy.faceSprite.offset = toVector2(frame.faceOffset);
}

function ensureMenuBoneAnim(bone) {
    if (bone.menuAnim != null) {
        return bone.menuAnim;
    }
    bone.menuAnim = { ...MENU_BONE_ANIM_TEMPLATE };
    bone.globalPosition = new Vector2(bone.menuAnim.startX, bone.menuAnim.y);
    bone.menuAnim.speed = bone.menuAnim.startSpeed;
    return bone.menuAnim;
}

export function resolveSansNextTurn(enemy) {
    let result;

    if (enemy.spared) {
        return "turns/spare/spare-turn.js";
    }

    if (enemy.stage === 1) {
        if (enemy.attackCount >= 13) {
            return "turns/spare/empty-turn.js";
        }

        UTMX.debug.print(enemy.turnIndex);

        if (enemy.turnIndex > 12) {
            const RANDOM_TURNS = [
                1, 3, 8, 10
            ]
            const randomIndex = Math.floor(Math.random() * RANDOM_TURNS.length);
            return STAGE1_TURN_MAP[RANDOM_TURNS[randomIndex]];
        }
        if (enemy.turnIndex === 0) {
            return "turns/stage1/first-turn.js";
        }
        result = STAGE1_TURN_MAP[enemy.turnIndex];
    } else if (enemy.stage === 2) {
        if (enemy.attackCount === 0) {
            result = null;
        }

        if (enemy.turnIndex >= 9) {
            if (enemy.attackCount < 10)
            {
                return "turns/stage2/sans-turn11.js";
            }
            else
            {
                return "turns/stage2/sans-turn10.js"
            }
        }
        const stage2Turn = STAGE2_TURN_MAP[enemy.turnIndex];
        if (stage2Turn != null) {
            result = stage2Turn;
        }
    }

    return result;
}

export function updateSansIdle(enemy, delta) {
    if (enemy.anim === 0) {
        return;
    }

    // Preserve previous behavior: any non-zero value drifts back to anim=1.
    if (enemy.anim == 1)
    {
        enemy.anim = 1;
        enemy.timer += delta * 2.5;
        enemy.bodyOffset.x = Math.cos(enemy.timer);
        enemy.bodyOffset.y = Math.sin(enemy.timer * 2) * 0.75;
        enemy.bodySprite.position = new Vector2(enemy.bodyOffset.x - 1, -36 + enemy.bodyOffset.y);
        enemy.faceSprite.position = new Vector2(0, -22).add(
            new Vector2(0, enemy.bodyOffset.y).multiply(0.6)
        );
    }
    else if (enemy.anim == 2)
    {
        enemy.timer += delta * 1;
        enemy.bodyOffset.y = Math.sin(enemy.timer) * 0.75;
        enemy.bodySprite.position = new Vector2(0.0, -36 + enemy.bodyOffset.y);
        enemy.faceSprite.position = new Vector2(0, -22).add(
            new Vector2(0, enemy.bodyOffset.y).multiply(0.6)
        );
    }
}

export function enableSansMenuBone(enemy, enabled) {
    if (enabled) {
        if (enemy.menuBone != null) {
            return;
        }
        const bone = UTMX.BattleProjectile.new();
        bone.textures = "textures/sans/attack/spr_s_boneloop_0.png";
        bone.destroyOnTurnEnd = false;
        bone.globalPosition = new Vector2(-50, 280);
        bone.menuAnim = null;
        enemy.menuBone = bone;
        enemy.menuBoneUpdating = true;
        bone.onHit = () => {
            if (UTMX.player.hp > 1) {
                UTMX.player.hurt(1);
            }
        };
    }
    else {
        if (enemy.menuBone != null) {
            enemy.menuBone.destroy();
        }
        enemy.menuBone = null;
        enemy.menuBoneMoving = false;
    }
}

export function updateSansMenuBone(enemy) {
    const bone = enemy.menuBone;
    if (bone == null) {
        return;
    }
    const anim = ensureMenuBoneAnim(bone);
    if (anim.holdTimer > 0) {
        anim.holdTimer -= 1;
        if (anim.holdTimer <= 0 && enemy.menuBoneCanMoveIn) {
            anim.speed = anim.startSpeed;
            bone.globalPosition = new Vector2(anim.startX, anim.y);
        }
        return;
    }

    let x = bone.globalPosition != null ? bone.globalPosition.x : anim.startX;
    x += anim.speed;
    anim.speed -= 0.01 * (Math.abs(anim.speed) < 0.3 ? 1 : 4);
    anim.speed = Math.max(anim.speed, -anim.startSpeed);

    if (x < anim.hideX) {
        x = anim.hideX;
        anim.speed = 0;
        anim.holdTimer = anim.holdFrames;
    }
    
    bone.globalPosition = new Vector2(x, anim.y);
}

export function processSansSlam(enemy) {
    if (!enemy.slamming) {
        return;
    }

    enemy.slammingTimer += 1;
    const slamDir = normalizeSlamDir(enemy.slammingDir);
    const dirFrames = SLAM_FRAMES[slamDir];
    const frame = dirFrames != null ? dirFrames[enemy.slammingTimer] : null;
    if (frame != null) {
        applySlamFrame(enemy, frame);
    }

    if (enemy.slammingTimer === 30 + enemy.slammingTimerLength) {
        enemy.setBody(0);
        enemy.bodySprite.offset = Vector2.Zero;
        enemy.faceSprite.offset = Vector2.Zero;
        enemy.slamming = false;
    }
}

export function processSansBlueEyes(enemy) {
    if (!enemy.blueEyes) {
        return;
    }

    if (enemy.blueEyesTimer % 15 === 0) {
        enemy.faceSprite.textures = enemy.blueEyesTextures[enemy.blueEyesStatus ? 1 : 0];
        enemy.blueEyesStatus = !enemy.blueEyesStatus;
    }
    enemy.blueEyesTimer += 1;
}
