import { UTMX, Vector2, Color } from "UTMX";

export default class GasterBlaster extends UTMX.Sprite {
    static introTextures = [
        "libraries/gaster-blaster/spr_gasterblaster_0.png",
        "libraries/gaster-blaster/spr_gasterblaster_1.png",
        "libraries/gaster-blaster/spr_gasterblaster_2.png"
    ];
    static fireTextures = [
        "libraries/gaster-blaster/spr_gasterblaster_3.png",
        "libraries/gaster-blaster/spr_gasterblaster_4.png",
        "libraries/gaster-blaster/spr_gasterblaster_5.png"
    ];
    static fireKeepTextures = [
        "libraries/gaster-blaster/spr_gasterblaster_4.png",
        "libraries/gaster-blaster/spr_gasterblaster_5.png"
    ];
    static beamTexture = "libraries/gaster-blaster/spr_gb_fire_1.png";

    constructor() {
        super();
        this.status = 0; 
        this.timer = 0;
        this.targetPos = Vector2.Zero;
        this.targetRot = 0;
        this.shootDelay = 40;
        this.holdFire = 20;   
        this.builderSpd = 0;  
        this.beam = null;
        this.baseBeamWidth = 0;
        this.introSpeed = 0.15;
        this.lerp = (a, b, t) => a + (b - a) * t;
    }
    
    static create(pos, targetPos, rot, targetRot, scaleX = 1, scaleY = 1, holdFire = 20) {
        let gb = GasterBlaster.new();
        gb.position = pos;
        gb.targetPos = targetPos;
        gb.rotation = rot;
        gb.targetRot = targetRot;
        gb.holdFire = holdFire;
        
        if (gb.targetRot >= 180) gb.targetRot -= 360;
        
        gb.scale = new Vector2(2 * scaleX, 2 * scaleY);
        
        return gb;
    }

    start() {
        this.textures = GasterBlaster.introTextures;
        this.frame = 0;
        this.stop(); 
        this.loop = false;
        this.z = 300;
        this.frameSpeed = 3.0;
        let snd = UTMX.audio.playSound("libraries/gaster-blaster/mus_sfx_segapower.wav");
        UTMX.audio.setSoundPitch(snd, 1.2);
    }

    spawnBeam() {
        this.beam = UTMX.BattleProjectile.new();
        this.beam.textures = [GasterBlaster.beamTexture]; 
        
        let currentScale = this.scale;
        this.beam.offset = new Vector2(0, 2048); 
        
        this.baseBeamWidth = currentScale.x * 0.85;
        this.beam.scale = new Vector2(this.baseBeamWidth, 0);
        
        this.beam.color = new Color(1, 1, 1, 1);
        this.beam.z = 0;
        this.beam.onHit = () => {
            if (this.beam.color.a > 0.8) UTMX.player.hurt(this.beam.damage);
        }
        
        this.calculateBeamPosition();
        
        UTMX.audio.playSound("libraries/gaster-blaster/mus_sfx_rainbowbeam_1.wav");
        
        let camera = UTMX.scene.getCamera();
        if (camera != null) camera.startShake(0.35, new Vector2(14, 14), new Vector2(35, 35));
        
    }

    calculateBeamPosition() {
        if (!this.beam) return;
        this.beam.position = this.position;
        this.beam.rotation = this.rotation;
        
        if (this.status === 1) {
            // 通过 timer 控制频率 (0.5)，通过系数控制振幅 (0.15)
            // 基础宽度 + sin 波动
            let pulse = Math.sin(this.timer * 0.35) * 0.15;
            let currentWidth = this.baseBeamWidth + pulse;
            
            let shrinkStart = this.shootDelay + 8 + this.holdFire;
            if (this.timer < shrinkStart) {
                this.beam.scale = new Vector2(currentWidth, this.beam.scale.y);
            }
        }
    }

    update(delta) {
        this.timer += 1;
        let curPos = this.position;

        if (this.status === 0) {
            let nextX = this.lerp(curPos.x, this.targetPos.x, this.introSpeed);
            let nextY = this.lerp(curPos.y, this.targetPos.y, this.introSpeed);
            this.position = new Vector2(nextX, nextY);
            this.rotation = this.lerp(this.rotation, this.targetRot, this.introSpeed);
            
            if (this.timer === this.shootDelay - 12) this.frame = 1;
            else if (this.timer === this.shootDelay - 8) this.frame = 2;

            if (this.timer >= this.shootDelay) {
                this.status = 1;
                this.rotation = this.targetRot;
                this.spawnBeam();
                this.textures = GasterBlaster.fireTextures;
                this.play(); 
                this.frameSpeed = 4.0;
            }
        } else {
            this.builderSpd += 1.2; 
            let rad = (this.rotation + 90) * Math.PI / 180;
            let moveX = Math.cos(rad) * this.builderSpd;
            let moveY = Math.sin(rad) * this.builderSpd;
            this.position = new Vector2(curPos.x - moveX, curPos.y - moveY);
            this.calculateBeamPosition();
            
            if (this.frame == 2) {
                this.textures = GasterBlaster.fireKeepTextures;
                this.play();
                this.loop = true;
            }
        }

        if (this.beam) {
            let bScale = this.beam.scale;
            let shootStart = this.shootDelay;
            let fullSize = this.shootDelay + 8; 
            let shrinkStart = fullSize + this.holdFire;
            
            if (this.timer > shootStart && this.timer <= fullSize) {
                let nextSY = this.lerp(bScale.y, 1, 0.5); 
                this.beam.scale = new Vector2(bScale.x, nextSY);
            } 
            else if (this.timer > shrinkStart) {
                let nextSX = this.lerp(bScale.x, 0, 0.2);
                
                let curColor = this.beam.color;
                let nextAlpha = Math.max(0, curColor.a - 0.05);
                this.beam.color = new Color(curColor.r, curColor.g, curColor.b, nextAlpha);
                
                if (nextSX < 0.05 || nextAlpha <= 0) {
                    this.beam.destroy();
                    this.beam = null;
                } else {
                    this.beam.scale = new Vector2(nextSX, bScale.y);
                }
            }
        }
        
        if (this.status === 1 && !this.beam) {
            if (curPos.x < -400 || curPos.x > 1000 || curPos.y < -400 || curPos.y > 1000) {
                this.destroy();
            }
        }
    }

    onDestroy()
    {
        if (this.beam != null) 
        {
            this.beam.destroy();
            this.beam = null;
        }
    }
}