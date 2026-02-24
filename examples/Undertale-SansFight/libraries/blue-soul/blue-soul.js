import { UTMX, Vector2, Color } from "UTMX";

const DEG_TO_RAD = Math.PI / 180;
const PLATFORM_SNAP_LOCAL_Y = -10;
const PLATFORM_SIDE_PADDING = 8;
const PLATFORM_EDGE_STICK_MARGIN = 2;
const PLATFORM_LANDING_EDGE_INSET = 1;
const PLATFORM_SUPPORT_TOLERANCE_UP = 2;
const PLATFORM_SUPPORT_TOLERANCE_DOWN = 6;
const PLATFORM_PENETRATION_BUFFER = 12;
const PLATFORM_FACING_DOT_MIN = 0.5;
const PLATFORM_LANDING_MIN_LOCAL_DY = 0.001;

class PlatformGreen extends UTMX.BattleProjectile {
    prevPosition = new Vector2(0, 0);
    targetController = null;
    speed = new Vector2(0, 0);
    leftSideSprite = null;
    rightSideSprite = null;
    length = 30;
    _destroyed = false;

    ensureSideSprites() {
        if (!this.leftSideSprite) {
            this.leftSideSprite = UTMX.Sprite.new();
            this.leftSideSprite.z = 1;
            this.leftSideSprite.textures = "libraries/blue-soul/platform_side.png";
            this.addChild(this.leftSideSprite);
        }

        if (!this.rightSideSprite) {
            this.rightSideSprite = UTMX.Sprite.new();
            this.rightSideSprite.z = 1;
            this.rightSideSprite.textures = "libraries/blue-soul/platform_side.png";
            this.addChild(this.rightSideSprite);
        }
    }

    start() {
        this.textures = "libraries/blue-soul/platform-content.png";
        this.canCollideWithSoul = false;
        this._destroyed = false;
        this.ensureSideSprites();
    }

    update(delta) {
        if (this._destroyed) return;
        this.ensureSideSprites();

        const p = this.position;
        if (!p) return;
        this.prevPosition = new Vector2(p.x, p.y);

        if (this.speed.x !== 0 || this.speed.y !== 0) {
            this.position = new Vector2(p.x + this.speed.x, p.y + this.speed.y);
        }

        const length = this.length;
        if (!this.leftSideSprite || !this.rightSideSprite) return;
        this.scale = new Vector2(length, 1);
        const invLength = 1 / length;

        this.leftSideSprite.position = new Vector2(-1, 0);
        this.leftSideSprite.scale = new Vector2(invLength, 1);

        this.rightSideSprite.position = new Vector2(1, 0);
        this.rightSideSprite.scale = new Vector2(invLength, 1);
    }

    getHalfWidth() {
        return this.length + PLATFORM_SIDE_PADDING;
    }

    getSupportHalfWidth() {
        // Shrink the support area near side corners to avoid edge sticking.
        return Math.max(0, this.getHalfWidth() - PLATFORM_EDGE_STICK_MARGIN);
    }

    getLandingHalfWidth() {
        // Keep landing snap slightly inside platform bounds to avoid edge lock.
        return Math.max(0, this.getSupportHalfWidth() - PLATFORM_LANDING_EDGE_INSET);
    }

    getMotionDelta() {
        return new Vector2(this.position.x - this.prevPosition.x, this.position.y - this.prevPosition.y);
    }

    getDownDirectionWorld() {
        const angleRad = this.rotation * DEG_TO_RAD;
        return new Vector2(Math.sin(angleRad), Math.cos(angleRad));
    }

    supportsFallDirection(fallDirection) {
        const downDir = this.getDownDirectionWorld();
        const dot = downDir.x * fallDirection.x + downDir.y * fallDirection.y;
        return dot > PLATFORM_FACING_DOT_MIN;
    }

    worldToLocal(worldPos, usePrevPosition = false) {
        const basePos = usePrevPosition ? this.prevPosition : this.position;
        const offsetX = worldPos.x - basePos.x;
        const offsetY = worldPos.y - basePos.y;

        const angleRad = this.rotation * DEG_TO_RAD;
        const cos = Math.cos(angleRad);
        const sin = Math.sin(angleRad);

        return new Vector2(
            offsetX * cos - offsetY * sin,
            offsetX * sin + offsetY * cos
        );
    }

    localToWorld(localPos, usePrevPosition = false) {
        const basePos = usePrevPosition ? this.prevPosition : this.position;

        const angleRad = this.rotation * DEG_TO_RAD;
        const cos = Math.cos(angleRad);
        const sin = Math.sin(angleRad);

        return new Vector2(
            basePos.x + localPos.x * cos + localPos.y * sin,
            basePos.y - localPos.x * sin + localPos.y * cos
        );
    }

    isPositionOnTop(worldPos) {
        const localPos = this.worldToLocal(worldPos);
        const halfWidth = this.getSupportHalfWidth();
        if (halfWidth <= 0 || Math.abs(localPos.x) >= halfWidth) return false;

        const minY = PLATFORM_SNAP_LOCAL_Y - PLATFORM_SUPPORT_TOLERANCE_UP;
        const maxY = PLATFORM_SNAP_LOCAL_Y + PLATFORM_SUPPORT_TOLERANCE_DOWN;
        return localPos.y >= minY && localPos.y <= maxY;
    }

    tryResolveLanding(prevSoulPos, nextSoulPos, fallDirection) {
        if (!this.supportsFallDirection(fallDirection)) return null;

        const prevLocal = this.worldToLocal(prevSoulPos, true);
        const nextLocal = this.worldToLocal(nextSoulPos, false);
        const localDy = nextLocal.y - prevLocal.y;

        const movingTowardSurface = localDy > PLATFORM_LANDING_MIN_LOCAL_DY;
        if (!movingTowardSurface) return null;

        const crossedSurface = prevLocal.y <= PLATFORM_SNAP_LOCAL_Y && nextLocal.y >= PLATFORM_SNAP_LOCAL_Y;
        const enteredSurfaceBand =
            nextLocal.y >= PLATFORM_SNAP_LOCAL_Y &&
            nextLocal.y <= PLATFORM_SNAP_LOCAL_Y + PLATFORM_PENETRATION_BUFFER &&
            prevLocal.y <= PLATFORM_SNAP_LOCAL_Y + PLATFORM_SUPPORT_TOLERANCE_DOWN;

        if (!crossedSurface && !enteredSurfaceBand) return null;

        let hitRatio = 1;
        if (Math.abs(localDy) > 0.0001) {
            hitRatio = (PLATFORM_SNAP_LOCAL_Y - prevLocal.y) / localDy;
            if (hitRatio < 0) hitRatio = 0;
            if (hitRatio > 1) hitRatio = 1;
        }

        const hitLocalX = prevLocal.x + (nextLocal.x - prevLocal.x) * hitRatio;
        const landingHalfWidth = this.getLandingHalfWidth();
        if (landingHalfWidth <= 0 || Math.abs(hitLocalX) > landingHalfWidth) return null;

        const snapLocalX = Math.max(-landingHalfWidth, Math.min(landingHalfWidth, hitLocalX));
        return {
            platform: this,
            hitRatio: hitRatio,
            position: this.localToWorld(new Vector2(snapLocalX, PLATFORM_SNAP_LOCAL_Y))
        };
    }

    onDestroy() {
        this._destroyed = true;
        if (this.targetController) {
            this.targetController.unregisterPlatform(this);
        }

        if (this.leftSideSprite) {
            this.leftSideSprite.destroy();
            this.leftSideSprite = null;
        }
        if (this.rightSideSprite) {
            this.rightSideSprite.destroy();
            this.rightSideSprite = null;
        }
    }
}

export default class BlueSoulController {
    constructor() {
        this._enabled = false;
        this.jumping = false;
        this.moveSpeed = 140.0;
        this.gravity = 300.0;
        this.jumpSpeed = 0.0;
        this.jumpStartSpeed = -200.0;
        this.slamGravityScale = 4.0;
        this.slamming = false;
        this.onPlatform = false;
        this._supportPlatform = null;
        this._platforms = new Set();

        this._cachedDirectionIndex = -1;
        this._inputMap = { upKey: "up", hAxisNeg: "left", hAxisPos: "right" };
    }

    get enabled() {
        return this._enabled;
    }

    set enabled(value) {
        const soul = UTMX.battle.soul;
        if (value) {
            soul.movable = false;
            soul.sprite.color = Color.Blue;
        } else {
            soul.movable = true;
            soul.sprite.color = Color.Red
        }
        this.reset();
        this._enabled = value;
    }
    
    reset()
    {
        this.jumping = false;
        this.jumpSpeed = 0;
        this.slamming = false;
        this.onPlatform = false;
        this._supportPlatform = null;
    }

    createPlatform(len = 30) {
        const platform = PlatformGreen.new();
        platform.targetController = this;
        platform.length = len;
        this.registerPlatform(platform);
        return platform;
    }

    createPlateform(len = 30) {
        return this.createPlatform(len);
    }

    registerPlatform(platform) {
        if (!platform) return;
        this._platforms.add(platform);
    }

    unregisterPlatform(platform) {
        if (!platform) return;
        this._platforms.delete(platform);
        if (this._supportPlatform === platform) {
            this._supportPlatform = null;
            this.onPlatform = false;
        }
    }

    normalizeRotation(rotationDeg) {
        let rot = rotationDeg % 360;
        if (rot < 0) rot += 360;
        return rot;
    }

    getDirectionIndexFromRotation(rotationDeg) {
        const rot = this.normalizeRotation(rotationDeg);
        return Math.floor((rot + 45) / 90) % 4;
    }

    updateInputMap(directionIndex) {
        if (directionIndex === this._cachedDirectionIndex) return this._inputMap;

        this._cachedDirectionIndex = directionIndex;
        const map = this._inputMap;
        if (directionIndex === 0) {
            map.upKey = "up";
            map.hAxisNeg = "left";
            map.hAxisPos = "right";
        } else if (directionIndex === 1) {
            map.upKey = "right";
            map.hAxisNeg = "up";
            map.hAxisPos = "down";
        } else if (directionIndex === 2) {
            map.upKey = "down";
            map.hAxisNeg = "right";
            map.hAxisPos = "left";
        } else {
            map.upKey = "left";
            map.hAxisNeg = "down";
            map.hAxisPos = "up";
        }
        return map;
    }

    getFallDirectionForDirectionIndex(directionIndex) {
        if (directionIndex === 0) return new Vector2(0, 1);
        if (directionIndex === 1) return new Vector2(-1, 0);
        if (directionIndex === 2) return new Vector2(0, -1);
        return new Vector2(1, 0);
    }

    findSupportPlatform(worldPos, fallDirection) {
        let bestPlatform = null;
        let bestDistance = Number.POSITIVE_INFINITY;

        for (const platform of this._platforms) {
            if (!platform) continue;
            if (!platform.supportsFallDirection(fallDirection)) continue;
            const localPos = platform.worldToLocal(worldPos);
            const halfWidth = platform.getSupportHalfWidth();
            if (halfWidth <= 0 || Math.abs(localPos.x) >= halfWidth) continue;

            const minY = PLATFORM_SNAP_LOCAL_Y - PLATFORM_SUPPORT_TOLERANCE_UP;
            const maxY = PLATFORM_SNAP_LOCAL_Y + PLATFORM_SUPPORT_TOLERANCE_DOWN;
            if (localPos.y < minY || localPos.y > maxY) continue;

            const distance = Math.abs(localPos.y - PLATFORM_SNAP_LOCAL_Y);
            if (distance < bestDistance) {
                bestDistance = distance;
                bestPlatform = platform;
            }
        }

        return bestPlatform;
    }

    resolvePlatformLanding(prevPos, nextPos, fallDirection) {
        let bestHit = null;

        for (const platform of this._platforms) {
            if (!platform) continue;
            const hit = platform.tryResolveLanding(prevPos, nextPos, fallDirection);
            if (!hit) continue;

            if (!bestHit || hit.hitRatio < bestHit.hitRatio) {
                bestHit = hit;
            }
        }

        return bestHit;
    }

    update(delta) {
        if (!this._enabled) return;
        const soul = UTMX.battle.soul;
        const directionIndex = this.getDirectionIndexFromRotation(soul.rotation);
        const inputMap = this.updateInputMap(directionIndex);
        const fallDirection = this.getFallDirectionForDirectionIndex(directionIndex);
        const snappedRotation = directionIndex * 90;

        let startPos = soul.position;
        this._supportPlatform = this.findSupportPlatform(startPos, fallDirection);
        this.onPlatform = this._supportPlatform !== null;

        if (this._supportPlatform && !this.jumping && !this.slamming) {
            const carry = this._supportPlatform.getMotionDelta();
            if (carry.x !== 0 || carry.y !== 0) {
                soul.tryMoveTo(new Vector2(startPos.x + carry.x, startPos.y + carry.y));
                startPos = soul.position;
            }
        }

        const grounded = soul.isOnArenaFloor() || this.onPlatform;
        if (grounded && this.slamming) {
            this.stopSlam();
        }

        let vx = 0;
        let vy = 0;

        if (grounded) {
            if (UTMX.input.isActionHeld(inputMap.upKey) && !this.jumping) {
                this.jumping = true;
                this.jumpSpeed = this.jumpStartSpeed;
                this.onPlatform = false;
                this._supportPlatform = null;
                vy = this.jumpSpeed;
            } else if (this.jumping && this.jumpSpeed < 0) {
                vy = this.jumpSpeed;
            } else {
                this.jumpSpeed = 0.0;
                vy = 0;
                this.jumping = false;
            }
        } else if (this.slamming) {
            this.jumping = false;
            this.jumpSpeed = this.gravity * this.slamGravityScale;
            vy = this.jumpSpeed;
        } else {
            let g = Math.abs(this.jumpSpeed) < 50 ? this.gravity * 0.5 : this.gravity;
            if (this.jumpSpeed > 0) g *= 1.25;
            this.jumpSpeed += g * delta;
            vy = this.jumpSpeed;
        }

        if (this.jumping && vy < 0) {
            if (UTMX.input.isActionReleased(inputMap.upKey) || soul.isOnArenaCeiling()) {
                this.jumping = false;
                this.jumpSpeed = 0.0;
                vy = 0;
            }
        }

        vx = UTMX.input.getActionAxis(inputMap.hAxisNeg, inputMap.hAxisPos) * this.moveSpeed * (UTMX.input.isActionHeld("cancel") ? 0.5 : 1.0);

        const rotationRad = snappedRotation * DEG_TO_RAD;
        const cos = Math.cos(rotationRad);
        const sin = Math.sin(rotationRad);

        const dx = vx * delta;
        const dy = vy * delta;
        const worldMoveX = dx * cos - dy * sin;
        const worldMoveY = dx * sin + dy * cos;

        let targetPos = new Vector2(startPos.x + worldMoveX, startPos.y + worldMoveY);
        const landingHit = this.resolvePlatformLanding(startPos, targetPos, fallDirection);

        if (landingHit) {
            targetPos = landingHit.position;
            this._supportPlatform = landingHit.platform;
            this.onPlatform = true;
            this.jumping = false;
            this.jumpSpeed = 0;
            if (this.slamming) {
                this.stopSlam();
            }
        } else {
            this.onPlatform = false;
            this._supportPlatform = null;
        }

        soul.tryMoveTo(targetPos);

        if (!landingHit) {
            const supportAfterMove = this.findSupportPlatform(soul.position, fallDirection);
            this._supportPlatform = supportAfterMove;
            this.onPlatform = supportAfterMove !== null;
            if (this.onPlatform) {
                this.jumping = false;
                this.jumpSpeed = 0;
                if (this.slamming) {
                    this.stopSlam();
                }
            }
        }
    }

    isOnFloor() {
        const soul = UTMX.battle.soul;
        const directionIndex = this.getDirectionIndexFromRotation(soul.rotation);
        const fallDirection = this.getFallDirectionForDirectionIndex(directionIndex);
        return soul.isOnArenaFloor() || this.findSupportPlatform(soul.position, fallDirection) !== null;
    }

    slam() {
        if (!this._enabled) return;
        this.jumping = false;
        this.jumpSpeed = 0;
        this.onPlatform = false;
        this._supportPlatform = null;
        this.slamming = true;
    }

    stopSlam() {
        if (!this.slamming) return;
        this.slamming = false;
        UTMX.audio.playSound("libraries/blue-soul/snd_impact.wav");
        const camera = UTMX.scene.getCamera();
        if (camera) camera.startShake(0.30, new Vector2(15, 15), new Vector2(30, 30));
    }

    setEnabled(enabled) {
        this.enabled = enabled;
        UTMX.audio.playSound("libraries/blue-soul/snd_tempbell.wav");
    }
}
