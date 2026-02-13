import { __Color } from "__UTMX";

class _Color extends __Color {}

__Color.prototype.add = function (value) {
    if (value instanceof __Color) {
        this.r += value.r;
        this.g += value.g;
        this.b += value.b;
        this.a += value.a;
    } else if (typeof value === "number") {
        this.r += value;
        this.g += value;
        this.b += value;
        this.a += value;
    }
    return this;
};

__Color.prototype.subtract = function (value) {
    if (value instanceof __Color) {
        this.r -= value.r;
        this.g -= value.g;
        this.b -= value.b;
        this.a -= value.a;
    } else if (typeof value === "number") {
        this.r -= value;
        this.g -= value;
        this.b -= value;
        this.a -= value;
    }
    return this;
};

__Color.prototype.multiply = function (value) {
    if (value instanceof __Color) {
        this.r *= value.r;
        this.g *= value.g;
        this.b *= value.b;
        this.a *= value.a;
    } else if (typeof value === "number") {
        this.r *= value;
        this.g *= value;
        this.b *= value;
        this.a *= value;
    }
    return this;
};

__Color.prototype.divide = function (value) {
    if (value instanceof __Color) {
        this.r /= value.r;
        this.g /= value.g;
        this.b /= value.b;
        this.a /= value.a;
    } else if (typeof value === "number") {
        this.r /= value;
        this.g /= value;
        this.b /= value;
        this.a /= value;
    }
    return this;
};

function defineColor(name, r, g, b, a = 1) {
    Object.defineProperty(_Color, name, {
        get() {
            return new __Color(r, g, b, a);
        },
        enumerable: true
    });
}

defineColor("AliceBlue", 0.9411765, 0.972549, 1, 1); // 爱丽丝蓝
defineColor("AntiqueWhite", 0.98039216, 0.92156863, 0.84313726, 1); // 古董白
defineColor("Aqua", 0, 1, 1, 1); // 水色
defineColor("Aquamarine", 0.49803922, 1, 0.83137256, 1); // 海蓝色
defineColor("Azure", 0.9411765, 1, 1, 1); // 天蓝色
defineColor("Beige", 0.9607843, 0.9607843, 0.8627451, 1); // 米黄色
defineColor("Bisque", 1, 0.89411765, 0.76862746, 1); // 橘黄色
defineColor("Black", 0, 0, 0, 1); // 黑色（GDScript默认值）
defineColor("BlanchedAlmond", 1, 0.92156863, 0.8039216, 1); // 杏仁白色
defineColor("Blue", 0, 0, 1, 1); // 蓝色
defineColor("BlueViolet", 0.5411765, 0.16862746, 0.8862745, 1); // 蓝紫色
defineColor("Brown", 0.64705884, 0.16470589, 0.16470589, 1); // 棕色
defineColor("Burlywood", 0.87058824, 0.72156864, 0.5294118, 1); // 硬木色
defineColor("CadetBlue", 0.37254903, 0.61960787, 0.627451, 1); // 军服蓝
defineColor("Chartreuse", 0.49803922, 1, 0, 1); // 黄褐色
defineColor("Chocolate", 0.8235294, 0.4117647, 0.11764706, 1); // 巧克力色
defineColor("Coral", 1, 0.49803922, 0.3137255, 1); // 珊瑚色
defineColor("CornflowerBlue", 0.39215687, 0.58431375, 0.92941177, 1); // 矢车菊蓝色
defineColor("Cornsilk", 1, 0.972549, 0.8627451, 1); // 玉米须色
defineColor("Crimson", 0.8627451, 0.078431375, 0.23529412, 1); // 绯红
defineColor("Cyan", 0, 1, 1, 1); // 青色
defineColor("DarkBlue", 0, 0, 0.54509807, 1); // 深蓝色
defineColor("DarkCyan", 0, 0.54509807, 0.54509807, 1); // 深青色
defineColor("DarkGoldenrod", 0.72156864, 0.5254902, 0.043137256, 1); // 深色菊科植物色
defineColor("DarkGray", 0.6627451, 0.6627451, 0.6627451, 1); // 深灰色
defineColor("DarkGreen", 0, 0.39215687, 0, 1); // 深绿色
defineColor("DarkKhaki", 0.7411765, 0.7176471, 0.41960785, 1); // 深卡其色
defineColor("DarkMagenta", 0.54509807, 0, 0.54509807, 1); // 深洋红色
defineColor("DarkOliveGreen", 0.33333334, 0.41960785, 0.18431373, 1); // 深橄榄绿色
defineColor("DarkOrange", 1, 0.54901963, 0, 1); // 深橙色
defineColor("DarkOrchid", 0.6, 0.19607843, 0.8, 1); // 深色兰花色
defineColor("DarkRed", 0.54509807, 0, 0, 1); // 深红色
defineColor("DarkSalmon", 0.9137255, 0.5882353, 0.47843137, 1); // 深鲑鱼色
defineColor("DarkSeaGreen", 0.56078434, 0.7372549, 0.56078434, 1); // 深海绿色
defineColor("DarkSlateBlue", 0.28235295, 0.23921569, 0.54509807, 1); // 深板蓝
defineColor("DarkSlateGray", 0.18431373, 0.30980393, 0.30980393, 1); // 暗石板灰色
defineColor("DarkTurquoise", 0, 0.80784315, 0.81960785, 1); // 深绿松石色
defineColor("DarkViolet", 0.5803922, 0, 0.827451, 1); // 深紫罗兰色
defineColor("DeepPink", 1, 0.078431375, 0.5764706, 1); // 深粉色
defineColor("DeepSkyBlue", 0, 0.7490196, 1, 1); // 深邃天蓝色
defineColor("DimGray", 0.4117647, 0.4117647, 0.4117647, 1); // 暗灰色
defineColor("DodgerBlue", 0.11764706, 0.5647059, 1, 1); // 道奇蓝色
defineColor("Firebrick", 0.69803923, 0.13333334, 0.13333334, 1); // 耐火砖红色
defineColor("FloralWhite", 1, 0.98039216, 0.9411765, 1); // 花白色
defineColor("ForestGreen", 0.13333334, 0.54509807, 0.13333334, 1); // 森林绿色
defineColor("Fuchsia", 1, 0, 1, 1); // 洋红色
defineColor("Gainsboro", 0.8627451, 0.8627451, 0.8627451, 1); // 庚斯伯勒灰色
defineColor("GhostWhite", 0.972549, 0.972549, 1, 1); // 幽灵白
defineColor("Gold", 1, 0.84313726, 0, 1); // 金色
defineColor("Goldenrod", 0.85490197, 0.64705884, 0.1254902, 1); // 金菊色
defineColor("Gray", 0.74509805, 0.74509805, 0.74509805, 1); // 灰色
defineColor("Green", 0, 1, 0, 1); // 绿色
defineColor("GreenYellow", 0.6784314, 1, 0.18431373, 1); // 绿黄色
defineColor("Honeydew", 0.9411765, 1, 0.9411765, 1); // 蜜露色
defineColor("HotPink", 1, 0.4117647, 0.7058824, 1); // 亮粉色
defineColor("IndianRed", 0.8039216, 0.36078432, 0.36078432, 1); // 印度红色
defineColor("Indigo", 0.29411766, 0, 0.50980395, 1); // 靛青色
defineColor("Ivory", 1, 1, 0.9411765, 1); // 象牙色
defineColor("Khaki", 0.9411765, 0.9019608, 0.54901963, 1); // 卡其色
defineColor("Lavender", 0.9019608, 0.9019608, 0.98039216, 1); // 薰衣草色
defineColor("LavenderBlush", 1, 0.9411765, 0.9607843, 1); // 薰衣草紫红色
defineColor("LawnGreen", 0.4862745, 0.9882353, 0, 1); // 草坪绿色
defineColor("LemonChiffon", 1, 0.98039216, 0.8039216, 1); // 柠檬雪纺色
defineColor("LightBlue", 0.6784314, 0.84705883, 0.9019608, 1); // 浅蓝色
defineColor("LightCoral", 0.9411765, 0.5019608, 0.5019608, 1); // 浅珊瑚色
defineColor("LightCyan", 0.8784314, 1, 1, 1); // 淡青色
defineColor("LightGoldenrod", 0.98039216, 0.98039216, 0.8235294, 1); // 亮金菊黄色
defineColor("LightGray", 0.827451, 0.827451, 0.827451, 1); // 浅灰色
defineColor("LightGreen", 0.5647059, 0.93333334, 0.5647059, 1); // 浅绿色
defineColor("LightPink", 1, 0.7137255, 0.75686276, 1); // 浅粉色
defineColor("LightSalmon", 1, 0.627451, 0.47843137, 1); // 浅鲑鱼色
defineColor("LightSeaGreen", 0.1254902, 0.69803923, 0.6666667, 1); // 浅海绿色
defineColor("LightSkyBlue", 0.5294118, 0.80784315, 0.98039216, 1); // 浅天蓝色
defineColor("LightSlateGray", 0.46666667, 0.53333336, 0.6, 1); // 浅板岩灰色
defineColor("LightSteelBlue", 0.6901961, 0.76862746, 0.87058824, 1); // 浅钢蓝色
defineColor("LightYellow", 1, 1, 0.8784314, 1); // 浅黄色
defineColor("Lime", 0, 1, 0, 1); // 青柠色
defineColor("LimeGreen", 0.19607843, 0.8039216, 0.19607843, 1); // 石灰绿色
defineColor("Linen", 0.98039216, 0.9411765, 0.9019608, 1); // 亚麻色
defineColor("Magenta", 1, 0, 1, 1); // 洋红色
defineColor("Maroon", 0.6901961, 0.1882353, 0.3764706, 1); // 栗色
defineColor("MediumAquamarine", 0.4, 0.8039216, 0.6666667, 1); // 中等海蓝宝石色
defineColor("MediumBlue", 0, 0, 0.8039216, 1); // 中蓝色
defineColor("MediumOrchid", 0.7294118, 0.33333334, 0.827451, 1); // 中等兰色
defineColor("MediumPurple", 0.5764706, 0.4392157, 0.85882354, 1); // 中等紫色
defineColor("MediumSeaGreen", 0.23529412, 0.7019608, 0.44313726, 1); // 中海绿色
defineColor("MediumSlateBlue", 0.48235294, 0.40784314, 0.93333334, 1); // 中等板岩蓝色
defineColor("MediumSpringGreen", 0, 0.98039216, 0.6039216, 1); // 中等春天绿色
defineColor("MediumTurquoise", 0.28235295, 0.81960785, 0.8, 1); // 中等绿松石色
defineColor("MediumVioletRed", 0.78039217, 0.08235294, 0.52156866, 1); // 中等紫红色
defineColor("MidnightBlue", 0.09803922, 0.09803922, 0.4392157, 1); // 午夜蓝色
defineColor("MintCream", 0.9607843, 1, 0.98039216, 1); // 薄荷奶油色
defineColor("MistyRose", 1, 0.89411765, 0.88235295, 1); // 朦胧玫瑰色
defineColor("Moccasin", 1, 0.89411765, 0.70980394, 1); // 鹿皮鞋色
defineColor("NavajoWhite", 1, 0.87058824, 0.6784314, 1); // 纳瓦白
defineColor("NavyBlue", 0, 0, 0.5019608, 1); // 藏青色
defineColor("OldLace", 0.99215686, 0.9607843, 0.9019608, 1); // 旧蕾丝色
defineColor("Olive", 0.5019608, 0.5019608, 0, 1); // 橄榄色
defineColor("OliveDrab", 0.41960785, 0.5568628, 0.13725491, 1); // 暗淡橄榄色
defineColor("Orange", 1, 0.64705884, 0, 1); // 橙色
defineColor("OrangeRed", 1, 0.27058825, 0, 1); // 橘红色
defineColor("Orchid", 0.85490197, 0.4392157, 0.8392157, 1); // 兰花色
defineColor("PaleGoldenrod", 0.93333334, 0.9098039, 0.6666667, 1); // 淡金色
defineColor("PaleGreen", 0.59607846, 0.9843137, 0.59607846, 1); // 淡绿色
defineColor("PaleTurquoise", 0.6862745, 0.93333334, 0.93333334, 1); // 淡绿松石色
defineColor("PaleVioletRed", 0.85882354, 0.4392157, 0.5764706, 1); // 淡紫红色
defineColor("PapayaWhip", 1, 0.9372549, 0.8352941, 1); // 木瓜鞭色
defineColor("PeachPuff", 1, 0.85490197, 0.7254902, 1); // 桃花粉
defineColor("Peru", 0.8039216, 0.52156866, 0.24705882, 1); // 秘鲁色
defineColor("Pink", 1, 0.7529412, 0.79607844, 1); // 粉红色
defineColor("Plum", 0.8666667, 0.627451, 0.8666667, 1); // 梅花色
defineColor("PowderBlue", 0.6901961, 0.8784314, 0.9019608, 1); // 浅蓝色
defineColor("Purple", 0.627451, 0.1254902, 0.9411765, 1); // 紫色
defineColor("RebeccaPurple", 0.4, 0.2, 0.6, 1); // 丽贝卡紫色
defineColor("Red", 1, 0, 0, 1); // 红色
defineColor("RosyBrown", 0.7372549, 0.56078434, 0.56078434, 1); // 玫瑰棕
defineColor("RoyalBlue", 0.25490198, 0.4117647, 0.88235295, 1); // 宝蓝色
defineColor("SaddleBrown", 0.54509807, 0.27058825, 0.07450981, 1); // 鞍棕色
defineColor("Salmon", 0.98039216, 0.5019608, 0.44705883, 1); // 鲑鱼色
defineColor("SandyBrown", 0.95686275, 0.6431373, 0.3764706, 1); // 沙褐色
defineColor("SeaGreen", 0.18039216, 0.54509807, 0.34117648, 1); // 海绿色
defineColor("Seashell", 1, 0.9607843, 0.93333334, 1); // 贝壳色
defineColor("Sienna", 0.627451, 0.32156864, 0.1764706, 1); // 西恩娜色
defineColor("Silver", 0.7529412, 0.7529412, 0.7529412, 1); // 银色
defineColor("SkyBlue", 0.5294118, 0.80784315, 0.92156863, 1); // 天蓝色
defineColor("SlateBlue", 0.41568628, 0.3529412, 0.8039216, 1); // 石板蓝色
defineColor("SlateGray", 0.4392157, 0.5019608, 0.5647059, 1); // 石板灰
defineColor("Snow", 1, 0.98039216, 0.98039216, 1); // 雪白
defineColor("SpringGreen", 0, 1, 0.49803922, 1); // 春绿
defineColor("SteelBlue", 0.27450982, 0.50980395, 0.7058824, 1); // 钢蓝色
defineColor("Tan", 0.8235294, 0.7058824, 0.54901963, 1); // 棕褐色
defineColor("Teal", 0, 0.5019608, 0.5019608, 1); // 青色
defineColor("Thistle", 0.84705883, 0.7490196, 0.84705883, 1); // 蓟色
defineColor("Tomato", 1, 0.3882353, 0.2784314, 1); // 番茄色
defineColor("Transparent", 1, 1, 1, 0); // 透明色（Alpha为零的白色）
defineColor("Turquoise", 0.2509804, 0.8784314, 0.8156863, 1); // 松石绿
defineColor("Violet", 0.93333334, 0.50980395, 0.93333334, 1); // 紫罗兰色
defineColor("WebGray", 0.5019608, 0.5019608, 0.5019608, 1); // 网格灰
defineColor("WebGreen", 0, 0.5019608, 0, 1); // 网络绿
defineColor("WebMaroon", 0.5019608, 0, 0, 1); // 网络栗
defineColor("WebPurple", 0.5019608, 0, 0.5019608, 1); // 网络紫
defineColor("Wheat", 0.9607843, 0.87058824, 0.7019608, 1); // 小麦色
defineColor("White", 1, 1, 1, 1); // 白色
defineColor("WhiteSmoke", 0.9607843, 0.9607843, 0.9607843, 1); // 白烟色
defineColor("Yellow", 1, 1, 0, 1); // 黄色
defineColor("YellowGreen", 0.6039216, 0.8039216, 0.19607843, 1); // 黄绿色

export { _Color as __Color };