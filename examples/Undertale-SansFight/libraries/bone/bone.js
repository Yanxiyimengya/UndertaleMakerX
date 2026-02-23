import { UTMX, Vector2, Color } from "UTMX";

export default class Bone extends UTMX.BattleProjectile {
    static MODE_CENTER = 0;
    static MODE_VERTEX = 1;

    length = 0;
    lifeTimer = -1;
    headTop = null;
    headBottom = null;
    speed = Vector2.Zero;
    status = 0;
    mode = Bone.MODE_CENTER;
    vertexTop = null;
    vertexBottom = null;
    
    static create(len = 20, lifeTime = -1)
    {
        let b = Bone.new();
        b.textures = "libraries/bone/bone-body.png";
        
        b.headTop = UTMX.Sprite.new();
        b.headTop.z = 0;
        b.headTop.textures = "libraries/bone/bone-head.png";
        b.headTop.rotation = 0;
        
        b.headBottom = UTMX.Sprite.new();
        b.headBottom.z = 0;
        b.addChild(b.headTop);
        b.addChild(b.headBottom);
        b.headBottom.textures = "libraries/bone/bone-head.png";
        b.headBottom.rotation = 180;
        
        b.length = len;
        b.lifeTimer = lifeTime;
        
        b.onHit = () => {
            if (b.status == 1)
            {
                if (UTMX.battle.soul.isMoving())
                    UTMX.player.hurt(b.damage);
            }
            else 
            {
                UTMX.player.hurt(b.damage);
            }
        }
        return b;
    }

    setCenterMode()
    {
        this.mode = Bone.MODE_CENTER;
        this.vertexTop = null;
        this.vertexBottom = null;
        return this;
    }

    setVertexMode(pointA, pointB)
    {
        if (pointA == null || pointB == null) return this;
        this.mode = Bone.MODE_VERTEX;
        this.vertexTop = new Vector2(pointA.x, pointA.y);
        this.vertexBottom = new Vector2(pointB.x, pointB.y);
        this.applyVertexTransform();
        return this;
    }

    applyVertexTransform()
    {
        if (this.vertexTop == null || this.vertexBottom == null) return;

        const top = this.vertexTop;
        const bottom = this.vertexBottom;

        const center = new Vector2(
            (top.x + bottom.x) * 0.5,
            (top.y + bottom.y) * 0.5
        );
        this.globalPosition = center;

        const dx = top.x - center.x;
        const dy = top.y - center.y;
        const halfDistance = Math.sqrt(dx * dx + dy * dy);
        this.length = Math.max(halfDistance + 3, 3);
        this.rotation = (Math.atan2(dy, dx) + Math.PI / 2) * 180 / Math.PI;
    }
    
    timer = 0;
    update(delta)
    {
        this.timer += 1;

        let pos;
        if (this.mode == Bone.MODE_VERTEX && this.vertexTop != null && this.vertexBottom != null)
        {
            this.vertexTop = this.vertexTop.add(this.speed);
            this.vertexBottom = this.vertexBottom.add(this.speed);
            this.applyVertexTransform();
            pos = new Vector2(this.globalPosition.x, this.globalPosition.y);
        }
        else
        {
            pos = new Vector2(this.globalPosition.x, this.globalPosition.y);
            pos = pos.add(this.speed);
            this.globalPosition = pos;
        }
        
        this.scale = new Vector2(1, this.length - 2);
        let len = this.length - 3;
        if (this.headTop != null)
        {
            this.headTop.globalPosition = pos.add(new Vector2(0, -len).rotated(this.rotation * Math.PI / 180));
            this.headTop.globalScale = Vector2.One;
        }
        if (this.headBottom != null)
        {
            this.headBottom.globalPosition = pos.add(new Vector2(0, len).rotated(this.rotation * Math.PI / 180));
            this.headBottom.globalScale = Vector2.One;
        }
        
        if (this.lifeTimer > 0)
            if (this.timer > this.lifeTimer) this.destroy();
    }
    
    onDestroy()
    {
        this.headTop.destroy();
        this.headBottom.destroy();
    }
    
    setStatus(status)
    {
        this.status = status;
        if (status == 0) this.color = Color.White;
        else if (status == 1) this.color = Color.Color8(0, 160, 255);
    }
}





