# Color

Color 是包含四个 `number` 分量的 RGBA 颜色类型|可用于表示颜色或任意四元数值组合。

该类型为 Jint 引擎对 Godot `Color` 的包装对象|因此可以直接访问 Godot API 中的大部分属性与方法。
本文档仅列出 UTMX 层扩展或与 Godot 存在差异的部分。


---

?> 可以使用常量快速初始化特定值的 Color

<br>

<br>
<a href="https://raw.githubusercontent.com/godotengine/godot-docs/master/img/color_constants.png" target="_self">
<img src="/imgs/color_constants.png" alt="color_constants" title="点击此图片可查看源图片"></img>
</a>
<br><br>

<details>
<summary><b>颜色常量列表</b></summary>

| Name | Preview | Value | Description  |
| :--- | ------ | ----- | ---------- |
| Color.Transparent        | <span style="color:rgb(255,255,255,0)">■</span> | `Color(1, 1, 1, 0)`|透明色                                                   |
| Color.White              | <span style="color:rgb(255,255,255,255)">■</span> | `Color(1, 1, 1, 1)`|白色                                                      |
| Color.Black              | <span style="color:rgb(0,0,0,255)">■</span> | `Color(0, 0, 0, 1)`|黑色                                                      |
| Color.Red                | <span style="color:rgb(255,0,0,255)">■</span> | `Color(1, 0, 0, 1)`|红色                                                      |
| Color.Green              | <span style="color:rgb(0,255,0,255)">■</span> | `Color(0, 1, 0, 1)`|绿色                                                      |
| Color.Blue               | <span style="color:rgb(0,0,255,255)">■</span> | `Color(0, 0, 1, 1)`|蓝色                                                      |
| Color.Yellow             | <span style="color:rgb(255,255,0,255)">■</span> | `Color(1, 1, 0, 1)`|黄色                                                      |
| Color.Cyan               | <span style="color:rgb(0,255,255,255)">■</span> | `Color(0, 1, 1, 1)`|青色                                                      |
| Color.Magenta            | <span style="color:rgb(255,0,255,255)">■</span> | `Color(1, 0, 1, 1)`|洋红色                                                     |
| Color.Gray               | <span style="color:rgb(189,189,189,255)">■</span> | `Color(0.74509805, 0.74509805, 0.74509805, 1)`|灰色                             |
| Color.AliceBlue          | <span style="color:rgb(240,248,255,255)">■</span> | `Color(0.9411765, 0.972549, 1, 1)`|爱丽丝蓝                                       |
| Color.AntiqueWhite       | <span style="color:rgb(250,235,215,255)">■</span> | `Color(0.98039216, 0.92156863, 0.84313726, 1)`|古董白                               |
| Color.Aqua               | <span style="color:rgb(0,255,255,255)">■</span> | `Color(0, 1, 1, 1)`|水色                                                       |
| Color.Aquamarine         | <span style="color:rgb(127,255,212,255)">■</span> | `Color(0.49803922, 1, 0.83137256, 1)`|海蓝色                                      |
| Color.Azure              | <span style="color:rgb(240,255,255,255)">■</span> | `Color(0.9411765, 1, 1, 1)`|天蓝色                                               |
| Color.Beige              | <span style="color:rgb(245,245,220,255)">■</span> | `Color(0.9607843, 0.9607843, 0.8627451, 1)`|米黄色                               |
| Color.Bisque             | <span style="color:rgb(255,228,196,255)">■</span> | `Color(1, 0.89411765, 0.76862746, 1)`|橘黄色                                      |
| Color.BlanchedAlmond     | <span style="color:rgb(255,235,205,255)">■</span> | `Color(1, 0.92156863, 0.8039216, 1)`|杏仁白色                                     |
| Color.BlueViolet         | <span style="color:rgb(138,43,226,255)">■</span> | `Color(0.5411765, 0.16862746, 0.8862745, 1)`|蓝紫色                               |
| Color.Brown              | <span style="color:rgb(165,42,42,255)">■</span> | `Color(0.64705884, 0.16470589, 0.16470589, 1)`|棕色                               |
| Color.Burlywood          | <span style="color:rgb(222,184,135,255)">■</span> | `Color(0.87058824, 0.72156864, 0.5294118, 1)`|硬木色                               |
| Color.CadetBlue          | <span style="color:rgb(95,158,160,255)">■</span> | `Color(0.37254903, 0.61960787, 0.627451, 1)`|军服蓝                                |
| Color.Chartreuse         | <span style="color:rgb(127,255,0,255)">■</span> | `Color(0.49803922, 1, 0, 1)`|黄褐色                                              |
| Color.Chocolate          | <span style="color:rgb(210,105,30,255)">■</span> | `Color(0.8235294, 0.4117647, 0.11764706, 1)`|巧克力色                               |
| Color.Coral              | <span style="color:rgb(255,127,80,255)">■</span> | `Color(1, 0.49803922, 0.3137255, 1)`|珊瑚色                                       |
| Color.CornflowerBlue     | <span style="color:rgb(100,149,237,255)">■</span> | `Color(0.39215687, 0.58431375, 0.92941177, 1)`|矢车菊蓝色                            |
| Color.Cornsilk           | <span style="color:rgb(255,248,220,255)">■</span> | `Color(1, 0.972549, 0.8627451, 1)`|玉米须色                                       |
| Color.Crimson            | <span style="color:rgb(220,20,60,255)">■</span> | `Color(0.8627451, 0.078431375, 0.23529412, 1)`|绯红                                 |
| Color.DarkBlue           | <span style="color:rgb(0,0,139,255)">■</span> | `Color(0, 0, 0.54509807, 1)`|深蓝色                                              |
| Color.DarkCyan           | <span style="color:rgb(0,139,139,255)">■</span> | `Color(0, 0.54509807, 0.54509807, 1)`|深青色                                      |
| Color.DarkGoldenrod      | <span style="color:rgb(184,134,11,255)">■</span> | `Color(0.72156864, 0.5254902, 0.043137256, 1)`|深色菊科植物的颜色                       |
| Color.DarkGray           | <span style="color:rgb(169,169,169,255)">■</span> | `Color(0.6627451, 0.6627451, 0.6627451, 1)`|深灰色                                  |
| Color.DarkGreen          | <span style="color:rgb(0,100,0,255)">■</span> | `Color(0, 0.39215687, 0, 1)`|深绿色                                              |
| Color.DarkKhaki          | <span style="color:rgb(189,183,107,255)">■</span> | `Color(0.7411765, 0.7176471, 0.41960785, 1)`|深卡其色                                |
| Color.DarkMagenta        | <span style="color:rgb(139,0,139,255)">■</span> | `Color(0.54509807, 0, 0.54509807, 1)`|深洋红色                                      |
| Color.DarkOliveGreen     | <span style="color:rgb(85,107,47,255)">■</span> | `Color(0.33333334, 0.41960785, 0.18431373, 1)`|深橄榄绿色                             |
| Color.DarkOrange         | <span style="color:rgb(255,140,0,255)">■</span> | `Color(1, 0.54901963, 0, 1)`|深橙色                                              |
| Color.DarkOrchid         | <span style="color:rgb(153,50,204,255)">■</span> | `Color(0.6, 0.19607843, 0.8, 1)`|深色的兰花色                                        |
| Color.DarkRed            | <span style="color:rgb(139,0,0,255)">■</span> | `Color(0.54509807, 0, 0, 1)`|深红色                                              |
| Color.DarkSalmon         | <span style="color:rgb(233,150,122,255)">■</span> | `Color(0.9137255, 0.5882353, 0.47843137, 1)`|深鲑鱼色                                |
| Color.DarkSeaGreen       | <span style="color:rgb(143,188,143,255)">■</span> | `Color(0.56078434, 0.7372549, 0.56078434, 1)`|深海绿色                               |
| Color.DarkSlateBlue      | <span style="color:rgb(72,61,139,255)">■</span> | `Color(0.28235295, 0.23921569, 0.54509807, 1)`|深板蓝的颜色                            |
| Color.DarkSlateGray      | <span style="color:rgb(47,79,79,255)">■</span> | `Color(0.18431373, 0.30980393, 0.30980393, 1)`|暗石板灰色                             |
| Color.DarkTurquoise      | <span style="color:rgb(0,206,209,255)">■</span> | `Color(0, 0.80784315, 0.81960785, 1)`|深绿松石色                                     |
| Color.DarkViolet         | <span style="color:rgb(148,0,211,255)">■</span> | `Color(0.5803922, 0, 0.827451, 1)`|深紫罗兰色                                       |
| Color.DeepPink           | <span style="color:rgb(255,20,147,255)">■</span> | `Color(1, 0.078431375, 0.5764706, 1)`|深粉色                                       |
| Color.DeepSkyBlue        | <span style="color:rgb(0,191,255,255)">■</span> | `Color(0, 0.7490196, 1, 1)`|深邃的天蓝色                                             |
| Color.DimGray            | <span style="color:rgb(105,105,105,255)">■</span> | `Color(0.4117647, 0.4117647, 0.4117647, 1)`|暗灰色                                   |
| Color.DodgerBlue         | <span style="color:rgb(30,144,255,255)">■</span> | `Color(0.11764706, 0.5647059, 1, 1)`|道奇蓝色                                       |
| Color.Firebrick          | <span style="color:rgb(178,34,34,255)">■</span> | `Color(0.69803923, 0.13333334, 0.13333334, 1)`|耐火砖红色                               |
| Color.FloralWhite        | <span style="color:rgb(255,250,240,255)">■</span> | `Color(1, 0.98039216, 0.9411765, 1)`|花白色                                        |
| Color.ForestGreen        | <span style="color:rgb(34,139,34,255)">■</span> | `Color(0.13333334, 0.54509807, 0.13333334, 1)`|森林绿色                               |
| Color.Fuchsia            | <span style="color:rgb(255,0,255,255)">■</span> | `Color(1, 0, 1, 1)`|洋红色                                                       |
| Color.Gainsboro          | <span style="color:rgb(220,220,220,255)">■</span> | `Color(0.8627451, 0.8627451, 0.8627451, 1)`|庚斯伯勒灰色                               |
| Color.GhostWhite         | <span style="color:rgb(248,248,255,255)">■</span> | `Color(0.972549, 0.972549, 1, 1)`|幽灵白颜色                                           |
| Color.Gold               | <span style="color:rgb(255,215,0,255)">■</span> | `Color(1, 0.84313726, 0, 1)`|金色                                                |
| Color.Goldenrod          | <span style="color:rgb(218,165,32,255)">■</span> | `Color(0.85490197, 0.64705884, 0.1254902, 1)`|金菊色                                 |
| Color.GreenYellow        | <span style="color:rgb(173,255,47,255)">■</span> | `Color(0.6784314, 1, 0.18431373, 1)`|绿黄色                                       |
| Color.Honeydew           | <span style="color:rgb(240,255,240,255)">■</span> | `Color(0.9411765, 1, 0.9411765, 1)`|蜜露色                                        |
| Color.HotPink            | <span style="color:rgb(255,105,180,255)">■</span> | `Color(1, 0.4117647, 0.7058824, 1)`|亮粉色                                        |
| Color.IndianRed          | <span style="color:rgb(205,92,92,255)">■</span> | `Color(0.8039216, 0.36078432, 0.36078432, 1)`|印度红色                               |
| Color.Indigo             | <span style="color:rgb(75,0,130,255)">■</span> | `Color(0.29411766, 0, 0.50980395, 1)`|靛青色                                       |
| Color.Ivory              | <span style="color:rgb(255,255,240,255)">■</span> | `Color(1, 1, 0.9411765, 1)`|象牙色                                               |
| Color.Khaki              | <span style="color:rgb(240,230,140,255)">■</span> | `Color(0.9411765, 0.9019608, 0.54901963, 1)`|卡其色                                |
| Color.Lavender           | <span style="color:rgb(230,230,250,255)">■</span> | `Color(0.9019608, 0.9019608, 0.98039216, 1)`|薰衣草色                                |
| Color.LavenderBlush      | <span style="color:rgb(255,240,245,255)">■</span> | `Color(1, 0.9411765, 0.9607843, 1)`|薰衣草紫红色                                      |
| Color.LawnGreen          | <span style="color:rgb(124,252,0,255)">■</span> | `Color(0.4862745, 0.9882353, 0, 1)`|草坪绿色                                       |
| Color.LemonChiffon       | <span style="color:rgb(255,250,205,255)">■</span> | `Color(1, 0.98039216, 0.8039216, 1)`|柠檬雪纺色                                      |
| Color.LightBlue          | <span style="color:rgb(173,216,230,255)">■</span> | `Color(0.6784314, 0.84705883, 0.9019608, 1)`|浅蓝色                                 |
| Color.LightCoral         | <span style="color:rgb(240,128,128,255)">■</span> | `Color(0.9411765, 0.5019608, 0.5019608, 1)`|浅珊瑚色                                 |
| Color.LightCyan          | <span style="color:rgb(224,255,255,255)">■</span> | `Color(0.8784314, 1, 1, 1)`|淡青色                                               |
| Color.LightGoldenrod     | <span style="color:rgb(250,250,210,255)">■</span> | `Color(0.98039216, 0.98039216, 0.8235294, 1)`|亮金菊黄色                               |
| Color.LightGray          | <span style="color:rgb(211,211,211,255)">■</span> | `Color(0.827451, 0.827451, 0.827451, 1)`|浅灰色                                      |
| Color.LightGreen         | <span style="color:rgb(144,238,144,255)">■</span> | `Color(0.5647059, 0.93333334, 0.5647059, 1)`|浅绿色                                   |
| Color.LightPink          | <span style="color:rgb(255,182,193,255)">■</span> | `Color(1, 0.7137255, 0.75686276, 1)`|浅粉色                                        |
| Color.LightSalmon        | <span style="color:rgb(255,160,122,255)">■</span> | `Color(1, 0.627451, 0.47843137, 1)`|浅鲑鱼色                                        |
| Color.LightSeaGreen      | <span style="color:rgb(32,178,170,255)">■</span> | `Color(0.1254902, 0.69803923, 0.6666667, 1)`|浅海绿色                                 |
| Color.LightSkyBlue       | <span style="color:rgb(135,206,250,255)">■</span> | `Color(0.5294118, 0.80784315, 0.98039216, 1)`|浅天蓝色                                |
| Color.LightSlateGray     | <span style="color:rgb(119,136,153,255)">■</span> | `Color(0.46666667, 0.53333336, 0.6, 1)`|浅板岩灰色                                   |
| Color.LightSteelBlue     | <span style="color:rgb(176,196,222,255)">■</span> | `Color(0.6901961, 0.76862746, 0.87058824, 1)`|浅钢蓝色                                 |
| Color.LightYellow        | <span style="color:rgb(255,255,224,255)">■</span> | `Color(1, 1, 0.8784314, 1)`|浅黄色                                               |
| Color.Lime               | <span style="color:rgb(0,255,0,255)">■</span> | `Color(0, 1, 0, 1)`|青柠色                                                       |
| Color.LimeGreen          | <span style="color:rgb(50,205,50,255)">■</span> | `Color(0.19607843, 0.8039216, 0.19607843, 1)`|石灰绿色                                |
| Color.Linen              | <span style="color:rgb(250,240,230,255)">■</span> | `Color(0.98039216, 0.9411765, 0.9019608, 1)`|亚麻色                                |
| Color.Maroon             | <span style="color:rgb(176,48,96,255)">■</span> | `Color(0.6901961, 0.1882353, 0.3764706, 1)`|栗色                                   |
| Color.MediumAquamarine   | <span style="color:rgb(102,205,170,255)">■</span> | `Color(0.4, 0.8039216, 0.6666667, 1)`|中等海蓝宝石色                                   |
| Color.MediumBlue         | <span style="color:rgb(0,0,205,255)">■</span> | `Color(0, 0, 0.8039216, 1)`|中蓝色                                               |
| Color.MediumOrchid       | <span style="color:rgb(186,85,211,255)">■</span> | `Color(0.7294118, 0.33333334, 0.827451, 1)`|中等兰色                                 |
| Color.MediumPurple       | <span style="color:rgb(147,112,219,255)">■</span> | `Color(0.5764706, 0.4392157, 0.85882354, 1)`|中等紫色                                 |
| Color.MediumSeaGreen     | <span style="color:rgb(60,179,113,255)">■</span> | `Color(0.23529412, 0.7019608, 0.44313726, 1)`|中海绿色                                 |
| Color.MediumSlateBlue    | <span style="color:rgb(123,104,238,255)">■</span> | `Color(0.48235294, 0.40784314, 0.93333334, 1)`|中等板岩蓝色                              |
| Color.MediumSpringGreen  | <span style="color:rgb(0,250,154,255)">■</span> | `Color(0, 0.98039216, 0.6039216, 1)`|中等春天绿色                                     |
| Color.MediumTurquoise    | <span style="color:rgb(72,209,204,255)">■</span> | `Color(0.28235295, 0.81960785, 0.8, 1)`|中等绿松石色                                   |
| Color.MediumVioletRed    | <span style="color:rgb(199,21,133,255)">■</span> | `Color(0.78039217, 0.08235294, 0.52156866, 1)`|中等紫红色                               |
| Color.MidnightBlue       | <span style="color:rgb(25,25,112,255)">■</span> | `Color(0.09803922, 0.09803922, 0.4392157, 1)`|午夜蓝色                                 |
| Color.MintCream          | <span style="color:rgb(245,255,250,255)">■</span> | `Color(0.9607843, 1, 0.98039216, 1)`|薄荷奶油色                                       |
| Color.MistyRose          | <span style="color:rgb(255,228,225,255)">■</span> | `Color(1, 0.89411765, 0.88235295, 1)`|朦胧的玫瑰色                                       |
| Color.Moccasin           | <span style="color:rgb(255,228,181,255)">■</span> | `Color(1, 0.89411765, 0.70980394, 1)`|鹿皮鞋颜色                                       |
| Color.NavajoWhite        | <span style="color:rgb(255,222,173,255)">■</span> | `Color(1, 0.87058824, 0.6784314, 1)`|纳瓦白                                          |
| Color.NavyBlue           | <span style="color:rgb(0,0,128,255)">■</span> | `Color(0, 0, 0.5019608, 1)`|藏青色                                                |
| Color.OldLace            | <span style="color:rgb(253,245,230,255)">■</span> | `Color(0.99215686, 0.9607843, 0.9019608, 1)`|旧蕾丝色                                   |
| Color.Olive              | <span style="color:rgb(128,128,0,255)">■</span> | `Color(0.5019608, 0.5019608, 0, 1)`|橄榄色                                         |
| Color.OliveDrab          | <span style="color:rgb(107,142,35,255)">■</span> | `Color(0.41960785, 0.5568628, 0.13725491, 1)`|暗淡橄榄色                                 |
| Color.Orange             | <span style="color:rgb(255,165,0,255)">■</span> | `Color(1, 0.64705884, 0, 1)`|橙色                                                |
| Color.OrangeRed          | <span style="color:rgb(255,69,0,255)">■</span> | `Color(1, 0.27058825, 0, 1)`|橘红色                                               |
| Color.Orchid             | <span style="color:rgb(218,112,214,255)">■</span> | `Color(0.85490197, 0.4392157, 0.8392157, 1)`|兰花色                                   |
| Color.PaleGoldenrod      | <span style="color:rgb(238,232,170,255)">■</span> | `Color(0.93333334, 0.9098039, 0.6666667, 1)`|淡金色                                   |
| Color.PaleGreen          | <span style="color:rgb(152,251,152,255)">■</span> | `Color(0.59607846, 0.9843137, 0.59607846, 1)`|淡绿色                                   |
| Color.PaleTurquoise      | <span style="color:rgb(175,238,238,255)">■</span> | `Color(0.6862745, 0.93333334, 0.93333334, 1)`|淡绿松石色                                 |
| Color.PaleVioletRed      | <span style="color:rgb(219,112,147,255)">■</span> | `Color(0.85882354, 0.4392157, 0.5764706, 1)`|淡紫红色                                   |
| Color.PapayaWhip         | <span style="color:rgb(255,239,213,255)">■</span> | `Color(1, 0.9372549, 0.8352941, 1)`|木瓜鞭色                                          |
| Color.PeachPuff          | <span style="color:rgb(255,218,185,255)">■</span> | `Color(1, 0.85490197, 0.7254902, 1)`|桃花粉                                          |
| Color.Peru               | <span style="color:rgb(205,133,63,255)">■</span> | `Color(0.8039216, 0.52156866, 0.24705882, 1)`|秘鲁色                                   |
| Color.Pink               | <span style="color:rgb(255,192,203,255)">■</span> | `Color(1, 0.7529412, 0.79607844, 1)`|粉红色                                          |
| Color.Plum               | <span style="color:rgb(221,160,221,255)">■</span> | `Color(0.8666667, 0.627451, 0.8666667, 1)`|梅花色                                   |
| Color.PowderBlue         | <span style="color:rgb(176,224,230,255)">■</span> | `Color(0.6901961, 0.8784314, 0.9019608, 1)`|浅蓝色                                   |
| Color.Purple             | <span style="color:rgb(160,32,240,255)">■</span> | `Color(0.627451, 0.1254902, 0.9411765, 1)`|紫色                                       |
| Color.RebeccaPurple      | <span style="color:rgb(102,51,153,255)">■</span> | `Color(0.4, 0.2, 0.6, 1)`|丽贝卡紫色                                               |
| Color.RosyBrown          | <span style="color:rgb(188,143,143,255)">■</span> | `Color(0.7372549, 0.56078434, 0.56078434, 1)`|玫瑰棕                                   |
| Color.RoyalBlue          | <span style="color:rgb(65,105,225,255)">■</span> | `Color(0.25490198, 0.4117647, 0.88235295, 1)`|宝蓝色                                   |
| Color.SaddleBrown        | <span style="color:rgb(139,69,19,255)">■</span> | `Color(0.54509807, 0.27058825, 0.07450981, 1)`|鞍棕色                                   |
| Color.Salmon             | <span style="color:rgb(250,128,114,255)">■</span> | `Color(0.98039216, 0.5019608, 0.44705883, 1)`|鲑鱼色                                   |
| Color.SandyBrown         | <span style="color:rgb(244,164,96,255)">■</span> | `Color(0.95686275, 0.6431373, 0.3764706, 1)`|沙褐色                                   |
| Color.SeaGreen           | <span style="color:rgb(46,139,87,255)">■</span> | `Color(0.18039216, 0.54509807, 0.34117648, 1)`|海绿色                                   |
| Color.Seashell           | <span style="color:rgb(255,245,238,255)">■</span> | `Color(1, 0.9607843, 0.93333334, 1)`|贝壳色                                          |
| Color.Sienna             | <span style="color:rgb(160,82,45,255)">■</span> | `Color(0.627451, 0.32156864, 0.1764706, 1)`|西恩娜色                                   |
| Color.Silver             | <span style="color:rgb(192,192,192,255)">■</span> | `Color(0.7529412, 0.7529412, 0.7529412, 1)`|银色                                       |
| Color.SkyBlue            | <span style="color:rgb(135,206,235,255)">■</span> | `Color(0.5294118, 0.80784315, 0.92156863, 1)`|天蓝色                                   |
| Color.SlateBlue          | <span style="color:rgb(106,90,205,255)">■</span> | `Color(0.41568628, 0.3529412, 0.8039216, 1)`|石板蓝色                                   |
| Color.SlateGray          | <span style="color:rgb(112,128,144,255)">■</span> | `Color(0.4392157, 0.5019608, 0.5647059, 1)`|石板灰                                    |
| Color.Snow               | <span style="color:rgb(255,250,250,255)">■</span> | `Color(1, 0.98039216, 0.98039216, 1)`|雪白                                          |
| Color.SpringGreen        | <span style="color:rgb(0,255,127,255)">■</span> | `Color(0, 1, 0.49803922, 1)`|春绿                                                 |
| Color.SteelBlue          | <span style="color:rgb(70,130,180,255)">■</span> | `Color(0.27450982, 0.50980395, 0.7058824, 1)`|钢蓝色                                   |
| Color.Tan                | <span style="color:rgb(210,180,140,255)">■</span> | `Color(0.8235294, 0.7058824, 0.54901963, 1)`|棕褐色                                   |
| Color.Teal               | <span style="color:rgb(0,128,128,255)">■</span> | `Color(0, 0.5019608, 0.5019608, 1)`|青色                                          |
| Color.Thistle            | <span style="color:rgb(216,191,216,255)">■</span> | `Color(0.84705883, 0.7490196, 0.84705883, 1)`|蓟色                                    |
| Color.Tomato             | <span style="color:rgb(255,99,71,255)">■</span> | `Color(1, 0.3882353, 0.2784314, 1)`|番茄色                                          |
| Color.Turquoise          | <span style="color:rgb(64,224,208,255)">■</span> | `Color(0.2509804, 0.8784314, 0.8156863, 1)`|松石绿                                    |
| Color.Violet             | <span style="color:rgb(238,130,238,255)">■</span> | `Color(0.93333334, 0.50980395, 0.93333334, 1)`|紫罗兰色                                  |
| Color.WebGray            | <span style="color:rgb(128,128,128,255)">■</span> | `Color(0.5019608, 0.5019608, 0.5019608, 1)`|网格灰                                    |
| Color.WebGreen           | <span style="color:rgb(0,128,0,255)">■</span> | `Color(0, 0.5019608, 0, 1)`|网络绿                                                  |
| Color.WebMaroon          | <span style="color:rgb(128,0,0,255)">■</span> | `Color(0.5019608, 0, 0, 1)`|网络栗                                                  |
| Color.WebPurple          | <span style="color:rgb(128,0,128,255)">■</span> | `Color(0.5019608, 0, 0.5019608, 1)`|网络紫                                          |
| Color.Wheat              | <span style="color:rgb(245,222,179,255)">■</span> | `Color(0.9607843, 0.87058824, 0.7019608, 1)`|小麦色                                   |
| Color.WhiteSmoke         | <span style="color:rgb(245,245,245,255)">■</span> | `Color(0.9607843, 0.9607843, 0.9607843, 1)`|白烟色                                   |
| Color.YellowGreen        | <span style="color:rgb(154,205,50,255)">■</span> | `Color(0.6039216, 0.8039216, 0.19607843, 1)`|黄绿色                                   |

</details>

---

## 核心属性（Properties）

| Property | Type   | Default | Description |
| -------- | ------ | ------- | ----------- |
| r        | number | `0`     | 红色通道值       |
| g        | number | `0`     | 绿色通道值       |
| b        | number | `0`     | 蓝色通道值       |
| a        | number | `1`     | Alpha 通道值   |

---

## 方法（Methods）

### add

```javascript
add(value: number | Color) -> Color
```

替代 Godot 加法运算符重载。

* 传入 `number` 时：对所有通道执行数值相加
* 传入 `Color` 时：对所有通道执行对应相加

**Returns**
`Color` — 返回当前实例

| Parameter | Type           | Description |
| --------- | -------------- | ----------- |
| value     | number | Color | 加数          |

---

### subtract

```javascript
subtract(value: number | Color) -> Color
```

替代 Godot 减法运算符重载。

* 传入 `number` 时：对所有通道执行数值相减
* 传入 `Color` 时：对所有通道执行对应相减

**Returns**
`Color` — 返回当前实例

| Parameter | Type           | Description |
| --------- | -------------- | ----------- |
| value     | number | Color | 减数          |

---

### multiply

```javascript
multiply(value: number | Color) -> Color
```

替代 Godot 乘法运算符重载。

* 传入 `number` 时：对所有通道执行数值缩放
* 传入 `Color` 时：对所有通道执行对应相乘

**Returns**
`Color` — 返回当前实例

| Parameter | Type           | Description |
| --------- | -------------- | ----------- |
| value     | number | Color | 乘数          |

---

### divide

```javascript
divide(value: number | Color) -> Color
```

替代 Godot 除法运算符重载。

* 传入 `number` 时：对所有通道执行数值相除
* 传入 `Color` 时：对所有通道执行对应相除

**Returns**
`Color` — 返回当前实例

| Parameter | Type           | Description |
| --------- | -------------- | ----------- |
| value     | number | Color | 除数          |

---

## 其他 API

完整功能请参考 Godot 官方文档中的 `Color` 类说明。

[Godot 中的 Color](https://docs.godotengine.org/zh-cn/stable/classes/class_color.html)

---

#### 使用示例

```javascript
import { UTMX, Color } from "UTMX";

let c1 = Color.Red;
let c2 = Color.Red;

UTMX.debug.log(c1 === c2); // false

c1.r = 0.5;

UTMX.debug.log(c2.r); // 1
```
