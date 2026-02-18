# Weapon

继承 [Item](types/item/item.md)

Weapon 表示一个能够被玩家装备的武器物品对象。通过 [GameRegisterDB](modules/game-register-db.md) 将 Weapon 物品的脚本路径注册到数据库，然后通过 `UTMX.player.inventory.setWeapon` 方法为玩家添加指定注册Id的物品作为武器。

通过 `UTMX.Weapon` 访问

## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| attack | number | 0 | 武器的攻击力  |
| attackAnimation | string[] | 0 | 武器的攻击动画序列帧图片纹理数组  |
| attackAnimationSpeed | number | 1 | 武器攻击动画的播放速率 |
| attackSound | string | "" | 武器的攻击音效 |

## 方法（Methods）

### onAttack

```javascript
onAttack(value: number, targetEnemy: Enemy) -> number
```

当玩家装备此武器攻击怪物时触发的回调函数，用于计算对指定怪物造成的伤害，若玩家对怪物造成的伤害小于等于 `0`，则会显示Miss 文本，不会对怪物造成任何伤害。

* `value` 为玩家处于攻击状态按下确认键时，攻击条相对于中心处的距离，这个值越接近 `0` 表示越接近中心。
* `targetEnemy` 表示玩家攻击的目标敌人。

#### 使用示例

下面展示了实现一个来自 Undertale 的挥砍类武器默认的伤害计算方法。

```javascript
import { UTMX } from "UTMX";

export default class MyCustomWeapon extends UTMX.Weapon {
    onAttack(value, targetEnemy) {
        const atk = UTMX.player.attack + this.attack;
        const def = targetEnemy.defence;

        let damage = atk - def + (Math.random() * 2);

        if (value <= 12) {
            damage *= 2.2;
        } else {
            damage *= (1 - value / 545) * 2;
        }
        damage = Math.round(damage);
        damage = Math.Max(0, damage);

        return damage;
    }
}
```
