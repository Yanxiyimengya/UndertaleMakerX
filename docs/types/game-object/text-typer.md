# TextTyper

缁ф壙 [TransformableObject](types/game-object/transformable-object.md)

TextTyper 瀹炵幇浜嗕竴涓€愬瓧绗︽墦鍗版枃鏈殑澶勭悊绯荤粺锛屼笖鏀寔 [鏂囨湰鍛戒护](#鏂囨湰鍛戒护)銆?

閫氳繃 `UTMX.TextTyper` 璁块棶銆?

---

### BBCode

BBCode锛圔ulletin Board Code锛夋槸涓撻棬鐢ㄤ簬鍦ㄧ嚎璁哄潧銆佸叕鍛婃澘璁捐鐨勮交閲忕骇鏍囪璇█锛屽叾璇硶閫氳繃鏂规嫭鍙峰寘鍥存爣绛惧疄鐜版枃鏈牸寮忓寲銆?

UTMX 浣跨敤 BBCode 瀹炵幇 TextTyper 鐨勫瘜鏂囨湰鍔熻兘銆?

BBCode 鏈変袱绉嶆牸寮忥紝涓€绉嶄负 `[<name>=<value>]` 鐨勫舰寮忥紝鍙︿竴绉嶄负澶氬弬鏁板舰寮?`[<name> <key>=<value> <key>=<value> ...]`

### 鏂囨湰鍛戒护

UTMX 鎷撳睍浜?BBCode锛屽畠浠绉颁负 **鏂囨湰鍛戒护**锛屽綋鎵撳瓧鏈烘枃鏈墦鍗板埌鎸囧畾鏂囨湰鍛戒护鏃讹紝TextTyper 灏变細鎵ц瀹冦€?

鍙互閫氳繃 [processCmd](#processCmd) 瀹炵幇鑷畾涔夌殑鏂囨湰鍛戒护澶勭悊鍥炶皟鍑芥暟

<br>

<details>
<summary><b>鏂囨湰鍛戒护鍒楄〃</b></summary>

| 鍛戒护鍚?| 璇硶鏍煎紡 | 鍔熻兘璇存槑 |
|--------|----------|----------|
| waitfor | `[waitfor=<鎸夐敭/鍔ㄤ綔鍚?]` | 鏆傚仠鏂囨湰鎵撳嵃锛岀瓑寰呮寚瀹氭寜閿垨杈撳叆鍔ㄤ綔琚寜涓嬪悗鎭㈠ |
| wait | `[wait=<鏃堕棿鍊?]` | 鏆傚仠鏂囨湰鎵撳嵃鎸囧畾鏃堕暱锛堝崟浣嶏細绉掞級锛屾椂闀跨粨鏉熷悗鎭㈠鎵撳嵃 |
| blend | `[blend=<棰滆壊鍊?]` | 璁剧疆鏂囨湰鐨勬暣浣撴贩鍚堥鑹诧紙Modulate锛夛紝棰滆壊鍊兼敮鎸佸瓧绗︿覆鏍煎紡锛堝 "#FFFFFF"锛?|
| speed | `[speed=<鏁板€?]` | 淇敼鏂囨湰鎵撳嵃閫熷害锛堝崟浣嶏細绉?瀛楃锛夛紝鏁板€艰秺灏忔墦鍗拌秺蹇?|
| size | `[size=<鏁板€?]` | 璁剧疆鍚庣画鏂囨湰鐨勫瓧浣撳ぇ灏忥紙鍗曚綅锛氬儚绱狅級 |
| font | `[font=<瀛椾綋璺緞>]` | 鍔犺浇骞跺簲鐢ㄦ寚瀹氳矾寰勭殑瀛椾綋鍒板悗缁枃鏈?|
| instant | `[instant=<甯冨皵鍊?]` / `[instant]` | 璁剧疆鏄惁绔嬪嵆鎵撳嵃鍏ㄩ儴鏂囨湰锛氫紶鍏ュ竷灏斿€煎垯鎸夊€艰缃紝鏃犲弬鏁板垯鍒囨崲褰撳墠鐘舵€?|
| noskip | `[noskip=<甯冨皵鍊?]` / `[noskip]` | 璁剧疆鏄惁绂佹璺宠繃鎵撳嵃锛氫紶鍏ュ竷灏斿€煎垯鎸夊€艰缃紝鏃犲弬鏁板垯鍒囨崲褰撳墠鐘舵€?|
| clear | `[clear]` | 娓呯┖褰撳墠宸叉墦鍗扮殑鏂囨湰锛屽苟閲嶇疆瀛椾綋銆佸瓧鍙枫€侀鑹茬瓑鏍峰紡涓洪粯璁ゅ€?|
| end | `[end]` | 閿€姣佸綋鍓?TextTyper 瀹炰緥 |
| img | `[img path=<鍥剧墖璺緞> width=?<瀹藉害> height=?<楂樺害> color=?<棰滆壊鍊?]` | 鍦ㄦ枃鏈腑鎻掑叆鍥剧墖锛歱ath 涓哄浘鐗囪祫婧愯矾寰勶紝width/height 涓烘樉绀哄昂瀵革紝color 涓哄浘鐗囨贩鍚堥鑹?|
| shader | `[shader=<鐫€鑹插櫒璺緞>` | 璁剧疆鏂囨湰浣跨敤鐨勭潃鑹插櫒锛屽簲鐢ㄧ殑鐫€鑹插櫒鏁堟灉鏄叏灞€鐨?|
| voice | `[voice=<闊抽璺緞>]` / `[voice=null]` | 璁剧疆鎵撳嵃瀛楃鏃舵挱鏀剧殑璇煶闊抽锛氫紶鍏?null 鍒欐竻绌鸿闊?|
| play_sound | `[play_sound=<闊抽璺緞>]` | 鎾斁鎸囧畾璺緞鐨勯煶鏁堥煶棰戯紙涓€娆℃€ф挱鏀撅級 |
| play_bgm | `[play_bgm path=<闊抽璺緞> id=<鏍囪瘑> loop=?<甯冨皵鍊? pitch=?<闊宠皟> volume=?<闊抽噺> position=?<璧峰浣嶇疆>]` | 鎾斁鎸囧畾璺緞鐨勮儗鏅煶涔愶細<br>- id锛欱GM 鏍囪瘑锛堥粯璁?"_TYPER_BGM"锛?br>- loop锛氭槸鍚﹀惊鐜挱鏀?br>- pitch锛氶煶璋冿紙榛樿涓嶅彉锛?br>- volume锛氶煶閲忥紙榛樿涓嶅彉锛?br>- position锛氭挱鏀句綅缃紙榛樿锛?锛?|
| stop_bgm | `[stop_bgm=<鏍囪瘑>]` / `[stop_bgm id=<鏍囪瘑>]` | 鍋滄鎸囧畾鏍囪瘑鐨勮儗鏅煶涔愶細鏃犲弬鏁版椂鍋滄榛樿鏍囪瘑 "_TYPER_BGM" 鐨?BGM |
</details>

鍏充簬鍏朵粬 BBCode锛岃鍓嶅線 [RichTextLabel 涓殑 BBCode](https://docs.godotengine.org/zh-cn/4.x/tutorials/ui/bbcode_in_richtextlabel.html) 鏌ョ湅

## 鏍稿績灞炴€э紙Properties锛?

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| text     | string | ""      | 璇?`TextTyper` 鐨勬枃鏈?|
| instant  | boolean | false  | 鑻ヤ负 `true` 鍒欎娇鎵撳瓧鏈轰竴鐬棿鏄剧ず鎵€鏈夋枃鏈紝鐩村埌鏂囨湰鏈熬 |
| noskip   | boolean | false  | 鑻ヤ负 `true` 鍒欑姝㈠綋 `cancel` 瑙﹀彂鍚庤烦杩囨枃鏈?|
| processCmd  | function | undefined  | 鐢ㄤ簬澶勭悊鏂囨湰鍛戒护鐨勫洖璋冨嚱鏁?|
| shader | UTMX.Shader | null     | 璇?`DrawableObject` 浣跨敤鐨勭潃鑹插櫒 |

### processCmd

杩欐槸 TextTyper 涓殑涓€涓壒娈婂唴缃睘鎬э紝浣犲彲浠ヨ缃竴涓尶鍚嶅嚱鏁扮敤浜庢姄鍙栫壒瀹氱殑鏂囨湰鍛戒护锛屽苟鎵ц鐗瑰畾澶勭悊銆?

杩欎釜鍑芥暟鐨勫０鏄庢牸寮忓涓嬶細

```javascript
(cmdName, args) -> boolean
```

| Parameter | Type    | Description |
| --------- | ------- | ----------- |
| cmdName   | string | 鏂囨湰鍛戒护鐨勫悕绉?|
| args      | object | 鏂囨湰鍛戒护鍙傛暟  |

濡傛灉鎴愬姛鎶撳彇锛岃繖涓嚱鏁板簲璇ヨ繑鍥?`true`锛岃繖浼氳鐩?TextTyper 鍐呯疆鐨勫鐞嗘枃鏈懡浠ょ殑閫昏緫锛屽鏋滄病鏈夋垚鍔熸姄鍙栵紝杩欎釜鍑芥暟搴旇杩斿洖 `false`銆?

---

#### 浣跨敤绀轰緥

璇ョず渚嬪睍绀轰簡濡備綍閫氳繃 [processCmd](#processCmd)锛屽疄鐜拌嚜瀹氫箟鏂囨湰鍛戒护銆?

```javascript
import { UTMX } from "UTMX";

let typer = UTMX.TextTyper.new("[setFace=1][print message1='Hello, ' message2='UndertaleMakerX!']Test!");
let faceIndex = 0;
typer.processCmd = (cmdName, args) => {
    switch(cmdName)
    {
        case "setFace" : {
            faceIndex = Number(args.value);
            UTMX.debug.log(faceIndex);
            return true;
        }
        case "print" :  {
                UTMX.debug.log(args.message1, args.message2);
            return true;
        }
        default : {
            return false;
        }
    }
}
```

**鎺у埗鍙拌緭鍑?**

```
> 1
> Hello, UndertaleMakerX!
```

## 鏂规硶锛圡ethods锛?

### new

```javascript
new(text: string) -> TextTyper
```

闈欐€佹柟娉曪紝瀹炰緥鍖栬 TextTyper锛屽苟涓斾互 `text` 寮€濮嬫墦鍗版枃鏈€?

**Returns** `TextTyper`

---

### start

```javascript
start(text: string) -> void
```

娓呯┖鍏堝墠鐨勬枃鏈紝浠?`text` 寮€濮嬮噸鏂版墦鍗版枃鏈€?

**Returns** `void`

| Parameter   | Type   | Description |
| ----------- | ------ | ----------- |
| text | string | 鎵撳嵃鐨勭洰鏍囨枃鏈?  |

---

### getProgress

```javascript
getProgress() -> number
```

杩斿洖褰撳墠 TextTyper 鐨勬墦鍗拌繘搴︺€?

**Returns** `number`

---

### isFinished

```javascript
isFinished() -> boolean
```

妫€鏌ュ綋鍓?TextTyper 鐨勬墦鍗拌繘搴︽槸鍚﹀凡缁忓湪鏂囨湰鏈熬銆?

**Returns** `boolean`
---

## SpeechBubble 扩展命令

`UTMX.SpeechBubble` 使用的是 `SpeechBubbleTextTyper`，在标准 TextTyper 命令基础上新增：

| 命令 | 语法 | 说明 |
| --- | --- | --- |
| spikeVisible | `[spikeVisible=true]` / `[spikeVisible=false]` / `[spikeVisible]` | 控制尖角可见性；不传参数时切换 |
| dir | `[dir=top]` / `[dir=bottom]` / `[dir=left]` / `[dir=right]` / `[dir=0..3]` / `[dir]` | 设置尖角方向；不传参数时循环切换 |

更多见 [SpeechBubble](types/game-object/speeth-bubble.md)。
