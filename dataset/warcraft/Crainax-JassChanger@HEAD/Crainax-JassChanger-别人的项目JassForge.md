---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "别人的项目JassForge.md"
ref: HEAD
---

# JassForge 语言参考

基于当前编译器实现整理，时间点为 2026-04-27。

这份文档不是“计划中的语法”，而是按当前代码与测试反推出来的“已经实现并可使用的语法与语义”。主要依据来自：

- `Project/Parser.h`
- `Project/src/compiler/CompilerSemantics.cpp`
- `Project/src/compiler/CompilerResourceLeak.cpp`
- `Project/JassForgeStdLib.h`
- `Project/tests/*.cpp`

JassForge 本质上是一门面向 Warcraft 3 JASS 代码生成的新语言前端。它兼容传统 vJASS 风格，也支持更现代的 Zinc/C 风格语法，并在此基础上加入了泛型、强类型枚举、结构体反射、闭包、`new/delete/let`、静态局部变量、函数指针等能力。

## 1. 总体设计

JassForge 有两个语法模式：

- vJASS 风格
- Zinc 风格

同一个源文件里可以混用，两种模式通过注释切换：

```jass
//! zinc
library Demo {
    function A() {
    }
}
//! endzinc

library OldStyle
    function B takes nothing returns nothing
    endfunction
endlibrary
```

核心规则：

- Zinc 区域必须使用 `{}` 和 `;`
- vJASS 区域必须使用 `endfunction/endstruct/endif/...`
- Zinc 区域里不能写 vJASS 的 `takes/returns`、`call`、`endfunction`
- vJASS 区域里不要写 Zinc 的 `;`、`{}`、`->`

## 2. 顶层结构

当前顶层支持这些声明：

- `library`
- `scope`
- `struct`
- `enum`
- `type`
- `native`
- `globals ... endglobals`
- `function`
- 可见性修饰：`public`、`private`

`library` 和 `scope` 都支持：

- `initializer`
- 多 initializer
- `requires`
- `uses`
- `needs`
- `requires optional X`

示例：

```jass
library Damage requires Utils, Math initializer Init
endlibrary
```

```jass
//! zinc
library Damage requires Utils, Math initializer InitA, InitB {
}
//! endzinc
```

`library`/`scope` 体内还支持：

- `hook Target Handler`
- `public { ... }`
- `private { ... }`
- `interpreter "说明" { ... }`
- `interpreter "说明" ... endinterpreter`

注意：

- `public/private { ... }` 当前主要用于函数和全局变量声明块
- `hook` 是已实现的顶层语法
- `interpreter` 只允许出现在 `library`/`scope` 体内，且必须位于 `function` 外
- `interpreter` 不引入新作用域，不改变可见性、名称修饰或依赖关系，只是把一组声明标为同一个解释块
- interpreter block 允许继续嵌套 interpreter block；嵌套不会引入新作用域，只会形成一条完整解释路径
- 在 `function` 体内可以用调用语法糖 `interpreter.("说明").Func(...)` 或嵌套形式 `interpreter.("父").("子").Func(...)` 调用对应解释路径下的函数；当前只支持函数调用，不支持 interpreter 块内全局变量或其他成员访问
- 在需要 `code` 函数引用的地方，也可以写 `function interpreter.("说明").Func` 或嵌套形式 `function interpreter.("父").("子").Func`；这类写法会先按完整 interpreter 路径解析到唯一函数，再等价降级为普通 `function 真函数名`
- 嵌套调用路径必须精确匹配完整链，不支持只写叶子标签，也不支持父级自动下钻
- `interpreter.(...)...Func(...)` 和 `function interpreter.(...)...Func` 都会按完整解释路径在“当前调用点可见”的函数集合中做唯一匹配：同 library 可命中 private/protected，跨 library 只能命中 public/default；唯一命中则降级为普通函数调用或普通函数引用，零命中报“未找到函数”，多命中报“函数调用歧义/函数引用歧义”
- 最终 `war3map.j` 会输出 `// interpreter-start: ...` 和 `// interpreter-end: ...` 注释；由于代码生成分为 globals、struct support、functions 等阶段，同一个解释块可能在输出里被拆成多段注释
- 若 interpreter 存在嵌套，注释显示完整路径，例如 `D级装备 / 破布手套`

示例：

```jass
library Demo
    interpreter "基础声明"
        globals
            integer SharedValue = 1
        endglobals

        function Touch takes nothing returns nothing
        endfunction
    endinterpreter
endlibrary
```

```jass
library Demo
    interpreter "花括号形式" {
        function Touch takes nothing returns nothing
        endfunction
    }
endlibrary
```

```jass
//! zinc
library Demo {
    interpreter "Zinc 分组" {
        integer SharedValue = 1;

        function Touch() {
        }
    }
}
//! endzinc
```

```jass
library Demo
    interpreter "物品"
        private function CreateItem takes nothing returns nothing
        endfunction
    endinterpreter

    function Run takes nothing returns nothing
        call interpreter.("物品").CreateItem()
    endfunction
endlibrary
```

```jass
library Demo
    interpreter "D级装备"
        interpreter "破布手套"
            private function Install takes nothing returns nothing
            endfunction
        endinterpreter
    endinterpreter

    function Run takes nothing returns nothing
        call interpreter.("D级装备").("破布手套").Install()
    endfunction
endlibrary
```

```jass
library Demo
    interpreter "D级装备"
        interpreter "破布手套"
            private function Install takes nothing returns nothing
            endfunction
        endinterpreter
    endinterpreter

    function Run takes nothing returns nothing
        local code func = function interpreter.("D级装备").("破布手套").Install
    endfunction
endlibrary
```

## 3. 可见性规则

可见性关键字有：

- `public`
- `private`
- `protected`

当前实现中的默认可见性比较重要：

- vJASS 模式下，顶层函数/结构体/类型别名/枚举/方法默认是 `public`
- Zinc 模式下，顶层函数/结构体/类型别名/枚举/方法默认是 `private`
- 结构体字段在两种模式下默认都是 `public`

例子：

```jass
//! zinc
library Demo {
    function Hidden() { }        // 默认 private

    struct Foo {
        integer x;               // 默认 public 字段
        method Run() { }         // 默认 private 方法
        public method Open() { }
    }
}
//! endzinc
```

关于 `protected`：

- 语法上可写
- 但当前实现里，很多可见性检查对 `protected` 的处理更接近 `private/file-private`
- 不建议现在依赖“严格的 C++/Java 式 protected 继承可见性语义”

## 4. 基本类型系统

### 4.1 核心内建类型

已确认支持：

- `integer`
- `real`
- `boolean`
- `string`
- `handle`
- `code`
- `nothing`
- `hashtable`
- `thistype`

### 4.2 Warcraft 3 常用内建类型

已确认编译器语义层认识这些类型名：

- `unit`
- `timer`
- `trigger`
- `effect`
- `rect`
- `item`
- `destructable`
- `player`
- `force`
- `group`
- `region`
- `sound`
- `widget`
- `image`
- `eventid`
- `dialog`
- `button`
- `boolexpr`
- `location`
- `weathereffect`

### 4.3 用户类型

支持：

- `type` 定义的类型别名
- `struct` 类型
- `enum` 类型
- 泛型实例类型

## 5. 变量声明

### 5.1 全局变量

vJASS：

```jass
globals
    integer Count = 0
    string array Names
endglobals
```

Zinc：

```jass
//! zinc
library Demo {
    public integer Count = 0;
    private string Names[];
    private string Labels[3] = { "A", "B", "C" };
    private TaskInfo Tasks[4] = new TaskInfo(Count)[4];
}
//! endzinc
```

规则：

- 支持初始化表达式
- 支持数组
- Zinc 全局数组支持初始化列表：`string labels[] = { "A", "B" };`
- Zinc 全局结构体数组支持批量构造：`TaskInfo tasks[4] = new TaskInfo(arg)[4];`
- `new Type(args)[N]` 会在 `InitGlobals` 中生成循环，每个元素独立调用一次 `new`/构造函数
- 全局数组长度只能写整数字面量或留空 `[]`，不支持 `MAXPLAYERCOUNT` 这类符号长度
- `new Type(args)[N]` 的 `N` 当前必须是整数字面量；若左侧也写了固定长度，两边大小必须一致
- 全局数组不能用单个表达式初始化；`TaskInfo tasks[4] = new TaskInfo();` 非法，必须写成 `new TaskInfo()[4]`
- `constant` 全局变量必须在定义时初始化
- `constant` 全局变量不能是数组
- `hashtable` 不能声明为数组

### 5.2 局部变量

vJASS：

```jass
local integer i
local string name
```

Zinc：

```jass
integer i = 1;
string name = "A";
integer a, b = 2, c;
string arr[];
string buf[64];
string values[] = { "1", "2", "3" };
string empty[] = {};
```

规则：

- vJASS 仍遵循传统 JASS 规则：`local` 必须出现在首个可执行语句之前
- Zinc 支持块内声明
- Zinc 支持逗号分隔的多变量声明
- Zinc 局部数组支持初始化列表：`string arr[] = { "a", "b" };`
- 空初始化列表合法：`string arr[] = {};`
- 局部数组的 `new Type(args)[N]` 批量构造语法当前不支持
- `static` 局部数组当前不支持初始化列表
- `hashtable` 不能声明为数组

编译后会降级成普通 JASS 数组声明加多条赋值：

```jass
local string array values
set values[0] = "1"
set values[1] = "2"
set values[2] = "3"
```

### 5.3 静态局部变量

JassForge 支持函数内静态局部变量。

vJASS：

```jass
local static integer count = 0
```

Zinc：

```jass
static integer count = 0;
```

语义：

- 会被降级成编译器生成的隐藏全局变量
- 有初始化值时会生成初始化 guard
- 无初始化值时使用默认零值
- 闭包可直接引用外层静态局部变量

## 6. 函数

### 6.1 普通函数

vJASS：

```jass
function Add takes integer a, integer b returns integer
    return a + b
endfunction
```

Zinc：

```jass
//! zinc
function Add(integer a, integer b) -> integer {
    return a + b;
}
//! endzinc
```

规则：

- Zinc 中返回类型写在 `->` 后
- Zinc 中如果省略返回类型，默认是 `nothing`
- vJASS 仍然使用 `takes ... returns ...`

### 6.2 默认参数

只有 Zinc 函数/方法支持默认参数：

```jass
//! zinc
function Log(string msg, boolean force = false) {
}
//! endzinc
```

规则：

- 省略实参时由编译器补齐默认值
- 该能力也会影响函数指针调用、`sys::function` 绑定调用等代码生成
- 泛型结构体方法在单态化后仍会保留默认参数语义

### 6.3 `&` 引用参数

当前仅 Zinc 函数和普通命名 `method` 支持 `&` 引用参数：

```jass
//! zinc
function Bump(integer &x) {
    x = x + 1;
}
//! endzinc
```

```jass
//! zinc
struct Counter {
    method Add(integer &x) {
        x = x + 1;
    }
}
//! endzinc
```

规则：

- 调用点不写 `&`，直接写 `Bump(value)`；是否按引用传递由被调函数/方法签名决定
- 当前只支持“简单变量名”实参，例如 `Bump(x)`；不支持 `obj.field`、`arr[i]`、函数名等
- `const &` 暂不支持
- `&` 参数不能使用默认值
- vJASS `takes/returns` 签名、`native`、`operator`、构造/析构、hook 目标/handler、函数指针绑定目标都不支持 `&`
- 同一调用里不能把两个 `&` 形参绑定到同一个变量，例如 `Swap(x, x)`

### 6.4 `native`

支持 `native`，但当前 `native` 仍按传统 JASS 签名解析：

```jass
native UnitAddAbility takes unit whichUnit, integer abilityId returns boolean
```

注意：

- 即使在 Zinc 区域里，`native` 也不要写成 `native Foo(integer a) -> integer`

## 7. 结构体

### 7.1 定义形式

vJASS：

```jass
struct Hero
    integer hp
endstruct
```

Zinc：

```jass
//! zinc
struct Hero {
    integer hp = 100;
}
//! endzinc
```

### 7.2 支持的结构体特性

支持：

- 成员字段
- 成员默认值
- `extends`
- 泛型参数
- 方法
- 静态方法
- 操作符方法
- 构造函数
- 析构函数
- `readonly`
- `constant`
- `static`
- `stub`
- `implements` 语法解析

示例：

```jass
//! zinc
struct Box<T> extends Base implements IReadable {
    public T value;
    private static integer Count = 0;
    readonly real weight = 1.0;
}
//! endzinc
```

注意：

- `implements` 目前已被解析并存入 AST
- 但完整的 `interface/module` 体系没有看到完整的语义与代码生成链路
- 因此不要把它当成“已成熟的接口系统”

### 7.3 字段规则

字段支持：

- 普通字段
- 带默认值字段
- `readonly` 字段
- `static` 字段
- 固定长度数组字段

数组字段例子：

```jass
//! zinc
struct PlayerInfo {
    string name[32];
}
//! endzinc
```

规则：

- 结构体数组字段必须给出整数常量长度
- 结构体数组字段不能带默认初始化；如需填充数组，请在构造函数或显式方法中逐项赋值
- 结构体字段默认可见性是 `public`
- `readonly` 字段不能被赋值，包括不能通过反射赋值
- `constant` 结构体成员当前必须同时声明为 `static`
- `constant` 成员不能是数组
- `constant` 成员必须在定义时初始化

### 7.4 方法

vJASS：

```jass
method Move takes real x, real y returns nothing
endmethod
```

Zinc：

```jass
//! zinc
method Move(real x, real y) {
}

static method Create() -> Hero {
    return Hero.create();
}
//! endzinc
```

注意：

- Zinc 中结构体方法默认是 `private`
- vJASS 中结构体方法默认是 `public`
- 结构体内部不能用 `function`，只能用 `method`

### 7.5 操作符重载

已确认支持：

- `operator []`
- `operator []=`

例子：

```jass
//! zinc
struct Buffer {
    method operator [](integer index) -> integer {
        return 0;
    }

    method operator []=(integer index, integer value) {
    }
}
//! endzinc
```

### 7.5.1 const常量关键字的支持

const 修饰的局部变量/全局变量不可以被修改

### 7.6 构造与析构

JassForge 使用 C++ 风格构造/析构语法，不使用 `method` 关键字：

```jass
//! zinc
struct Foo {
    Foo(integer x) {
    }

    ~Foo() {
    }
}
//! endzinc
```

规则：

- 构造函数写成 `StructName(...) { ... }`
- 析构函数写成 `~StructName() { ... }`
- 不能写成 `method Foo(...)`
- 不能写成 `method ~Foo()`
- 构造/析构当前不能显式写 `public/private`
- 析构函数不能有参数
- 析构函数必须返回 `nothing`
- 构造/析构函数体当前必须使用 Zinc 的 `{ ... }`

### 7.7 `create/allocate/deallocate/destroy`

结构体生命周期相关的传统接口仍可用：

- `create`
- `allocate`
- `deallocate`
- `destroy`

`new`/`delete` 只是更高层的语法糖，底层仍会落到这些能力上。

## 8. `new`、`delete` 与 `let`

### 8.1 `new`

支持：

```jass
//! zinc
Foo a = new Foo();
Foo b = new Foo(10);
//! endzinc
```

语义：

- 若结构体没有构造函数且参数个数为 0，`new Foo()` 会降级成 `Foo.create()`
- 若存在匹配参数个数的构造函数，会调用编译器生成的 `__new_N`
- 若参数不匹配，不会静默回退到 `create()`，而是直接报错

### 8.2 `delete`

支持：

```jass
//! zinc
delete obj;
//! endzinc
```

语义：

- 若结构体定义了析构函数，先调用析构，再调用 `__deallocate`
- 若没有析构函数，直接 `__deallocate`

### 8.3 `let`

`let` 是带自动清理的作用域对象。

```jass
//! zinc
function Run() {
    let Foo a;
    let Bar b(10);
}
//! endzinc
```

语义：

- `let Foo a;` 会自动构造对象
- `let Foo a(10);` 会自动调用对应构造函数
- 离开作用域时自动析构并释放
- 多个 `let` 对象按逆序清理
- `return` 前会先清理 `let`
- `break` 前会先清理当前循环作用域里的 `let`

限制：

- `let` 只适合结构体对象
- `let Foo a()` 这种零参写法是错误的，应写 `let Foo a;`
- `let` 不允许显式 `delete`
- `let` 不允许整变量重绑定，例如 `a = other`
- `let` 当前不支持数组形式
- `let` 当前不支持逗号分隔多声明
- `let + continue` 当前不支持
- `let + exitwhen` 当前不支持

重要区别：

- 普通声明 `Foo f;` 仍然不会自动构造/析构
- 只有 `new` 和 `let` 会触发生命周期语义

## 9. 枚举

JassForge 支持强类型枚举。

vJASS：

```jass
enum Mode: integer
    Idle = 0
    Run = 1
endenum
```

Zinc：

```jass
//! zinc
enum HeroAttr: string {
    STR = "STR",
    AGI = "AGI"
}
//! endzinc
```

规则：

- 必须显式写底层类型
- 当前确认支持的底层类型：`integer`、`real`、`string`、`boolean`
- 每个枚举成员都必须显式初始化
- 枚举成员值必须是对应底层类型的字面量
- 使用时必须写限定名：`Mode.Run`
- 裸写 `Run` 会报错
- 枚举值是只读常量
- 枚举变量、全局、结构体字段若不是数组，必须显式初始化
- 不允许把底层基础类型值直接隐式赋给枚举
- 不允许不同枚举类型之间互相赋值
- 枚举比较只允许和同类型枚举用 `==`、`!=`

枚举转换：

- `static_cast<EnumType>(常量)` 有条件支持
- 只有编译器能在编译期验证该常量确实对应某个成员时才允许
- 从动态值或未知值强转到枚举会报错

代码生成：

- 枚举在语义层保留强类型
- 最终代码生成时会落到底层基础类型

## 10. 类型别名与函数指针

### 10.1 普通类型别名

```jass
type HeroId extends integer
```

### 10.2 函数指针类型

JassForge 支持比 `code` 更强的函数指针类型：

```jass
//! zinc
type StringCallback extends function(string, integer) -> string;
//! endzinc
```

规则：

- 参数列表里只能写类型，不能写参数名
- 返回类型省略时默认为 `nothing`
- 不能出现空参数项
- 不能漏逗号

使用方式：

```jass
//! zinc
type StringCallback extends function(string, string) -> string;

function Join(string a, string b) -> string {
    return a + b;
}

function Test() {
    StringCallback cb = Join;
    string s = cb.evaluate("A", "B");
}
//! endzinc
```

### 10.3 `code`

传统 `code` 仍然支持：

```jass
code c = function Foo
```

规则：

- `code` 引用的目标函数必须满足 JASS 回调规则：无参数、返回 `nothing`
- lambda 赋给 `code` 时也必须满足无参数、返回 `nothing`，因此通常写成 `function() { ... }`

### 10.4 `sys::function<Foo>`

这是 JassForge 的特殊语法糖，用来表达“精确绑定到某个全局函数”的可调用类型：

```jass
//! zinc
function Join(string a, string b) -> string {
    return a + b;
}

function Test() {
    sys::function<Join> cb = Join;
    string s = cb("A", "B");
}
//! endzinc
```

规则：

- 只能绑定全局函数
- 不是普通“按签名兼容”的函数指针，而是“按目标函数精确绑定”
- 绑定错函数名会报错
- 同一 library 内可以绑定 private 函数
- 跨 library 访问 private 函数会被拒绝
- 当前不要把它当成 struct 方法指针系统

`sys::function<Foo>` 和普通函数指针的区别：

- 普通函数指针是“按签名匹配”
- `sys::function<Foo>` 是“按具体函数实体匹配”

## 11. 泛型

### 11.1 用户泛型结构体

支持泛型结构体，并在编译时单态化：

```jass
//! zinc
struct Box<T> {
    T value;

    method assign(T v) {
        this.value = v;
    }

    method get() -> T {
        return this.value;
    }
}
//! endzinc
```

使用：

```jass
Box<integer> a = Box<integer>.create();
Box<real> b = Box<real>.create();
```

语义：

- 会生成具体实例类型，例如 `Box__integer`
- 支持多个类型参数，例如 `Pair<K, V>`
- 会检查实参数量是否匹配
- 类型实参可以是基础类型、用户类型、枚举类型等

### 11.2 内置泛型标准库

编译器会自动注入 `sys::` 标准容器模板：

- `sys::vector<T>`
- `sys::stack<T>`
- `sys::queue<T>`
- `sys::map<K, V>`

注意：

- 内置 STL 泛型必须写 `sys::` 前缀
- 用户自定义泛型不需要 `sys::`

### 11.3 标准容器 API

`sys::vector<T>`：

- `create()`
- `destroy()`
- `size()`
- `empty()`
- `at(index)`
- `operator []`
- `operator []=`
- `front()`
- `back()`
- `push_back(value)`
- `pop_back()`
- `insert(index, value)`
- `erase(index)`
- `clear()`

此外，编译器还提供专门的 `sys::sort`：

- `sys::sort(vec)`：仅支持 `sys::vector<integer>` 和 `sys::vector<real>`
- `sys::sort(vec, comparator)`：仅支持 `sys::vector<T>`，`comparator` 必须兼容签名 `(T, T) -> boolean`
- 严格只认 `sys::sort`，裸 `sort(...)` 不会被当作内置排序

例子：

```jass
//! zinc
sys::vector<integer> values = sys::vector<integer>.create();
sys::sort(values);

sys::vector<Player> players = sys::vector<Player>.create();
sys::sort(players, function(Player a, Player b) -> boolean {
    return a.level < b.level;
});
//! endzinc
```

`sys::stack<T>`：

- `create()`
- `destroy()`
- `size()`
- `empty()`
- `top()`
- `push(value)`
- `pop()`
- `clear()`

`sys::queue<T>`：

- `create()`
- `destroy()`
- `size()`
- `empty()`
- `front()`
- `back()`
- `push(value)`
- `pop()`
- `clear()`

`sys::map<K, V>`：

- `create()`
- `destroy()`
- `size()`
- `empty()`
- `at(index)`，按迭代顺序返回 key
- `operator []`
- `operator []=`
- `count(key)`
- `contains(key)`
- `insert(key, value)`
- `erase(key)`
- `clear()`

## 12. 语句与控制流

### 12.1 vJASS 语句

已确认支持：

- `local`
- `set`
- `call`
- `return`
- `if / elseif / else / endif`
- `loop / endloop`
- `exitwhen`
- `debug`

### 12.2 Zinc 语句

已确认支持：

- 局部变量声明
- 表达式语句
- 赋值
- `+=`
- `-=`
- `*=`
- `/=`
- `++`
- `return`
- `if / else if / else`
- `while`
- `loop`
- `exitwhen`
- `break`
- `continue`
- `for`
- range-based `for`
- `let`
- `delete`

### 12.3 `if`

Zinc：

```jass
if (ok) {
} else if (other) {
} else {
}
```

规则：

- 条件必须是 `boolean`，或 nullable reference-like（`string`、`code`、handle-like 及其别名）；nullable 条件按“非 `null`”判断
- 条件表达式里不允许赋值或复合赋值

### 12.4 `while`

```jass
while (i < 10) {
    i += 1;
}
```

规则：

- 条件必须是 `boolean`，或 nullable reference-like（`string`、`code`、handle-like 及其别名）；nullable 条件按“非 `null`”判断

### 12.5 `loop` 和 `exitwhen`

Zinc：

```jass
loop {
    exitwhen i > 10;
}
```

vJASS：

```jass
loop
    exitwhen i > 10
endloop
```

### 12.6 `for`

支持 C 风格 `for`：

```jass
//! zinc
for (integer i = 0; i < 10; i += 1) {
}
//! endzinc
```

支持 range-based `for`：

```jass
//! zinc
for (integer value : values) {
}
//! endzinc
```

规则：

- range-based `for` 当前支持 `sys::vector<T>` 和 `sys::map<K, V>`
- 遍历 `sys::map<K, V>` 时，循环变量类型是 key 类型 `K`
- `sys::stack` 和 `sys::queue` 当前不支持 range-based `for`

## 13. 表达式

### 13.1 字面量

已确认支持：

- 整数字面量
- 十六进制整数字面量
- 实数字面量
- 字符串字面量
- 原始字符串字面量 `R"(...)"`
- `true`
- `false`
- `null`
- FourCC / rawcode 字面量

FourCC 例子：

```jass
'A'
'ABCD'
```

十六进制例子：

```jass
0x1F
```

原始字符串例子：

```jass
R"(image("../image/console/turn_pan.webp"))"

R"(line1
line2)"
```

规则：

- 十六进制整数字面量使用 `0x` 前缀；`0x` 后不能为空
- 只接受 1 字符或 4 字符形式
- 不合法长度、空内容、非 ASCII、未闭合都会报错
- 普通字符串字面量不能跨行
- 原始字符串字面量使用固定语法 `R"(...)"`，当前只支持空分隔符形式
- 原始字符串内部不解析 `\n`、`\t`、`\"`、`\\` 这类转义，按字面内容保留
- 原始字符串可以跨行，但内容中不能再出现终止序列 `)"`
- 原始字符串最终仍会生成为普通 JASS 字符串，编译器会自动完成必要转义

### 13.2 算术与逻辑运算

支持：

- `+`
- `-`
- `*`
- `/`
- `%`
- `==`
- `!=`
- `<`
- `>`
- `<=`
- `>=`
- `and`
- `or`
- `not`
- `&&`
- `||`
- `!`
- 三目运算 `a ? b : c`

关于 `%`：

- 整数 `%` 会降级成 `ModuloInteger`
- 只要涉及 real，就会降级成 `ModuloReal`
- 若除数是字面量 0，编译时报错

### 13.3 赋值表达式

Zinc 中赋值也是表达式体系的一部分，但在条件里会受到限制。

支持：

- `=`
- `+=`
- `-=`
- `*=`
- `/=`

### 13.4 成员访问与下标访问

支持：

- `obj.member`
- `obj.method(...)`
- `obj[index]`

### 13.5 类型转换

JassForge 同时支持两类转换：

传统 cast：

```jass
(integer)str
(real)i
(string)r
```

安全 cast：

```jass
static_cast<real>(i)
```

规则：

- `static_cast` 只允许编译器能安全确定的转换
- 某些旧 JASS 风格转换仍然必须用 `(type)expr`
- 例如 `static_cast<string>(r)` 当前不允许，应继续用 `(string)r`
- 对 hashtable 读取、宽松转换、枚举受限转换，通常要比 `static_cast` 更谨慎

### 13.6 `sizeof`

支持：

```jass
sizeof(MyStruct)
```

当前语义：

- 对结构体返回字段数量

## 14. 强类型与比较规则

JassForge 的比较规则明显比原始 JASS 更严格。

已确认规则：

- `if` / `while` / `exitwhen` / 三元表达式条件必须是 `boolean`，或 nullable reference-like（`string`、`code`、handle-like 及其别名）
- `boolean` 不能和 `integer` 直接比较
- `string` 不能参与排序比较 `< > <= >=`
- handle-like 类型可以和 `null` 比较
- handle-like 类型不能做排序比较
- `integer` 不能和 `null` 比较
- 枚举只能和同枚举类型比较

这意味着很多“原始 JASS 里凑合能跑的写法”，在 JassForge 里会被直接拒绝。

## 15. 闭包与 lambda

JassForge 支持 lambda。

常见写法：

```jass
//! zinc
code cb = function() {
}

sys::function<boolean(integer, integer)> cmp = function(integer a, integer b) -> boolean {
    return a < b;
};
//! endzinc
```

规则：

- 语法形态为 `function(...) { ... }`
- 支持参数列表
- 支持显式返回类型，例如 `-> boolean`
- 不写返回类型时按 `nothing` 处理
- 可以自动捕获外层局部变量
- 可以捕获外层静态局部变量
- 已确认可直接用于 `code` 类型的默认参数、全局变量初始化和结构体成员默认值
- lambda 体内不允许隐式访问实例成员
- lambda 体内不允许隐式调用实例方法
- 若要访问成员，请显式通过对象引用访问
- lambda 体内当前不支持声明 `let`
- lambda 体内当前不支持声明 `static` 局部变量
- 默认参数中的 lambda 当前不支持捕获外部变量
- 成员默认值中的 lambda 当前不支持捕获外部变量或同结构体成员
- `constant` 全局初始化和 `static constant` 成员默认值当前不支持 lambda
- lambda 赋值仍然参与普通类型检查，不能赋给 `integer` 之类的非可调用类型

例子：

```jass
//! zinc
struct Foo {
    integer x;

    method Run() {
        Foo self = this;
        code cb = function() {
            self.x += 1; // 允许
        };
    }
}
//! endzinc
```

`code` 相关规则：

- 赋给 `code` 的 lambda 必须符合无参无返回
- `function Foo` 形式的函数引用也会做相同校验

额外例子：

```jass
//! zinc
library Demo {
    function Run(code cb = function() {
    }) {
    }

    code globalCb = function() {
    };

    struct Box {
        public code onClick = function() {
        };
    }
}
//! endzinc
```

特殊语法糖：

- `TriggerAddCondition(t, function() { ... })` 会自动包装成 `Condition(...)`
- 传入命名函数引用到 `TriggerAddCondition` 时也会自动包装

## 16. 结构体反射

这是 JassForge 很有辨识度的一组特性。

当前支持这些伪成员：

- `obj.at(index)`
- `obj.at(index, type)`
- `obj.at("fieldName")`
- `obj.at(nameVar, type)`
- `Struct.size()`
- `Struct.size(type)`
- `Struct.type(index)`

例子：

```jass
//! zinc
struct Stats {
    integer hp;
    real speed;
    string name;
}

function Test(Stats s, string key) {
    integer fieldCount = Stats.size();
    integer intCount = Stats.size(integer);
    string typeName = Stats.type(1);

    integer hp = s.at("hp");
    real spd = s.at("speed", real);
    s.at("hp") = 200;
    s.at(key, real) = 1.5;
}
//! endzinc
```

规则：

- 若索引或字段名是编译期常量，编译器会尽量直接降级为具体字段访问
- 若字段名/索引是运行时变量，会生成运行时分发代码
- 继承来的字段也会参与反射
- 若结构体字段类型混杂，运行时 `obj.at(var)` 可能必须显式给出类型过滤
- `readonly` 字段不能通过反射写入
- `Struct.type(i)` 返回字段类型名字符串
- `sizeof(Struct)` 返回字段总数

重要注意：

- 这些反射伪成员只在结构体本身没有同名用户方法时才作为内建语义生效
- 如果你自己定义了 `at/size/type` 方法，会屏蔽对应反射糖

## 17. Hashtable 语法糖

JassForge 对 hashtable 做了较强的语法糖支持。

### 17.1 双下标读写

```jass
ht[k1][k2] = value;
integer x = ht[k1][k2];
```

编译器会根据值类型和 key 类型，自动选择正确的 `SaveXxx/LoadXxx` 和 key 转换方式。

规则：

- `integer` key 直接使用
- `string` key 自动走 `StringHash`
- handle key 自动走 `GetHandleId`
- 编译器会尽量从上下文推断读取类型

限制：

- 若读取结果没有足够类型上下文，可能报“无法推断 hashtable 读取类型”
- hashtable 读取结果不能隐式当作枚举值使用

### 17.2 `_G()`

`_G()` 是 JassForge 的全局变量反射访问内建：

```jass
set _G("GlobalScore") = 123
return _G("PlayerData")[0]
```

规则：

- 必须且只能有一个参数
- 参数必须是字符串字面量
- 编译时会检查目标全局变量是否存在
- 访问数组全局时仍然必须继续写下标
- 不能写入常量全局
- 最终代码生成后 `_G()` 会被完全消除，不会残留在输出里

用途：

- 很适合配合混淆后的全局名访问

## 18. 预处理器

当前预处理器已确认支持：

- `#define NAME value`
- `#define FUNC(a, b) ...`
- 反斜杠续行宏
- `#undef`
- `#ifdef`
- `#ifndef`
- `#if`
- `#elif`
- `#else`
- `#endif`
- `defined(X)`
- `defined X`
- `!`
- `&&`
- `||`

例子：

```c
#define DEBUG 1
#define ADD(a, b) ((a) + (b))

#if defined(DEBUG) && DEBUG
    // ...
#endif
```

额外特性：

- 预处理器会保护枚举类型名和枚举成员名，避免被对象宏误替换
- 对函数式宏调用缺少右括号、不合法宏定义等情况会给出诊断
- 宏展开存在深度上限，当前实现为 512 层；超过会报“宏展开深度超过限制”
- 原始字符串内部的 `//`、`#endif`、宏名、逗号和括号不会参与注释截断、条件编译匹配或函数宏参数拆分

当前未确认/不要依赖：

- `#include` 当前没有看到完整实现链路，不建议写进项目规范里当作已支持能力

## 19. 编译器注解

JassForge 支持一批以 `//@` 开头的源级注解。

通用形式：

```jass
//@ annotation-name
//@ annotation-name 3
```

数字参数通常表示“影响后面若干条声明/函数”。

### 19.1 `//@ nodiscard`

用途：

- 不是 C++ 那种“声明返回值必须使用”的含义
- 在当前编译器里更像 warning 白名单/抑制标记
- 用于部分返回值未使用警告、生成输出警告、某些全局初始化警告的抑制与标记

### 19.2 `//@ desync ...`

已确认支持：

- `//@ desync danger`
- `//@ desync danger-scope`
- `//@ desync handle-op`
- `//@ desync safe`
- 兼容旧写法：`//@ desync danger-execstring`

用途：

- 参与编译器的 desync 静态分析
- 可标注函数，也能影响 lambda

### 19.3 `//@ execstring-func`

用途：

- 标记“通过字符串执行函数”的入口函数
- 参与混淆与全局初始化分析

### 19.4 `//@ data-encode`

用途：

- 给 integer/real 全局变量、结构体字段启用仿射编码

注意：

- 不是所有变量形式都可用
- 数组、某些常量场景会被拒绝

### 19.5 其他已确认注解

- `//@ print-log`
- `//@ print-log-all`
- `//@ fire-image`
- `//@ fire-image-all`
- `//@ no-obfuscate`

## 20. 编译器特殊检查

### 20.1 `sprintf`

编译器会对 `sprintf` 做额外校验。

当前要求：

- 第一个参数必须是字符串字面量
- 参数数量必须和格式符数量匹配
- 当前支持的格式符：`%d`、`%i`、`%s`、`%f`、`%r`、`%b`、`%%`

### 20.2 关键字保留

JassForge 对关键字和部分上下文关键字做了保留检查，避免拿关键字当：

- 枚举名
- 变量名
- `foreach` 变量名
- 声明名

例如 `initializer`、`hook` 在特定声明上下文里是保留的。

### 20.3 资源泄漏警告

编译器可通过 `--warn-resource-leak` 对最终 `war3map.j` 做资源泄漏静态检查。

当前已确认覆盖：

- `effect`
- `group`
- `timer`
- `location`

当前行为：

- 直接创建后未释放的局部资源会报警
- 忽略拥有型返回值或直接忽略 `Location()`、`CreateTimer()` 这类创建表达式时，会以“临时资源未被释放”形式报警
- 警告通常会附带来源表达式，例如 `来源: MakePoint()` 或 `来源: Location(0.00, 0.00)`
- 资源写入全局变量或全局数组时，当前按“已逃逸/已转交”处理，通常不会在当前函数直接报警
- 能识别常见的 `SaveXxxHandle/LoadXxxHandle` + `TimerStart` 回调清理链
- 也能跟踪通过 `code` 参数、`C2I/I2C` 以及 `ForForce/ForGroup` 传递的同步回调清理路径

注意：

- 这是一套静态告警系统，不是完整的所有权证明器；复杂封装函数仍建议结合生成代码与实际生命周期人工复核

### 20.4 数组语法

Zinc 数组可以写固定长度或省略长度：

```jass
string a[];
string b[64];
```

局部数组和全局数组都支持初始化列表：

```jass
string localNames[] = { "A", "B" };
string GlobalNames[] = { "A", "B" };
```

全局结构体数组还支持批量构造初始化：

```jass
TaskInfo Tasks[4] = new TaskInfo(seed)[4];
```

该写法会生成 `InitGlobals` 循环，逐项执行构造并写入数组。当前只支持全局数组，局部数组尚不支持 `new Type(args)[N]`。

严格限制：

- 左侧数组长度只能是整数字面量或留空 `[]`
- 右侧批量构造必须写成 `new Type(args)[整数大小]`
- `TaskInfo Tasks[MAXPLAYERCOUNT] = new TaskInfo();` 非法，因为左侧使用了符号长度，右侧也缺少 `[整数大小]`
- `TaskInfo Tasks[4] = new TaskInfo();` 非法，因为单个 `new` 不能初始化整个数组

## 21. 当前未稳定或暂不建议依赖的部分

以下内容不要当成“已经完整可用”：

- `interface`
- `module`
- `delegate`
- `super`
- 完整的 `implements` 接口系统
- `#include`

这些词法/语法入口有的已经保留，有的已部分入 AST，但目前没有看到完整、稳定、成体系的语义和代码生成闭环。

## 22. 推荐写法

如果你准备长期把 JassForge 当主语言用，当前最稳的写法组合是：

- 以 Zinc 风格为主
- 用 `library` 管理模块
- 用 `public/private` 显式写清可见性
- 结构体方法显式标 `public`，不要依赖 Zinc 默认可见性
- 枚举一律显式限定名访问
- 容器统一用 `sys::vector/sys::map`
- 需要自动析构时用 `let`
- 需要堆对象时用 `new/delete`
- 需要弱反射时优先用 `at/size/type/_G`
- 需要类型安全回调时优先用函数指针类型或 `sys::function`

## 23. 一页速记

最值得记住的 JassForge 差异点：

- 同文件可混用 vJASS 与 Zinc，靠 `//! zinc` / `//! endzinc` 切换
- Zinc 默认顶层/方法是 `private`
- 结构体字段默认 `public`
- 支持泛型结构体与自动注入的 `sys::vector/stack/queue/map`
- 支持强类型枚举，不再是裸整数常量
- 支持 `new/delete/let`
- 支持静态局部变量
- 支持 lambda，可写参数列表和显式返回类型，也可用于 `code` 默认参数/全局初始化/成员默认值
- 支持函数指针类型和 `sys::function<Foo>`
- 支持结构体反射 `at/size/type`
- 支持 hashtable 双下标语法糖和 `_G()`
- 预处理器支持宏与条件编译，并对宏递归和缺失右括号给出更严格诊断
- 可选开启 `--warn-resource-leak` 对 `effect/group/timer/location` 做静态资源泄漏警告
- 类型检查、比较检查、条件检查比传统 JASS 严格很多

如果你后续继续扩语言，建议把新增特性都先补测试，再直接把这一份文档当主手册持续更新。
