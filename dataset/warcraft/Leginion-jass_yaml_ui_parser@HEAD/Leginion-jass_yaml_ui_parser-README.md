---
type: vendor-doc
source_repo: "https://github.com/Leginion/jass_yaml_ui_parser"
source_path: "README.md"
ref: HEAD
---

一个用于读取YAML格式储存的Jass动作、函数的触发器UI配置数据的工具，可以自动读取function名字和返回值，并对应生成到action.txt、call.txt中。

将这个工具引入你的编译流程可以让你不需要再单独维护一个与你的jass匹配的mpq配置文件，提高魔兽触发编辑器的DIY易用性和易维护性。

（本工具目前暂不稳定，随时大改，开源仅供参考。）

---

Node使用示例：

```javascript
const parser = require("./../Utilities/jass_yaml_ui_parser/jass_yaml_ui_parser");
const fs = require('fs');

console.log(parser);

let msg = parser.makeTriggerInfoFromFiles([
    "./Source/jass/xywe/XYMath.j",
    "./test.j",
]);
console.log("============== action =================");
console.log(msg.action);
console.log("============== call =================");
console.log(msg.call);

let pathToAction = "./Source/mpq/generate/action.txt";
let pathToCall = "./Source/mpq/generate/call.txt";

if (fs.existsSync(pathToAction)) fs.unlinkSync(pathToAction);
if (fs.existsSync(pathToAction)) fs.unlinkSync(pathToCall);

fs.writeFileSync(pathToAction, msg.action);
fs.writeFileSync(pathToCall, msg.call);

console.log('finished generate');
```

Jass填写示例：

```jass
library SampleLibrary
{
    // <ui>
    // title: 获取两点进度偏移点
    // desc: 获取从起点 ${A} 到终点 ${B} 进度为 ${progress} / 1.00 的中间点
    // cate: TC_LOCATION
    // args:
    // - (location) GetUnitLoc
    // - (location) GetUnitLoc
    // - (real) 0.50
    // </ui>
    public function XYGetProgressLocation(location a, location b, real progress) -> location
    {
        real ax, ay, bx, by, dx, dy, px, py, rx, ry;
        ax = GetLocationX(a); ay = GetLocationY(a);
        bx = GetLocationX(b); by = GetLocationY(b);
        dx = bx - ax;
        dy = by - ay;
        px = dx * progress;
        py = dy * progress;
        rx = ax + px;
        ry = ay + py;
        return Location(rx, ry);
    }
}
```
