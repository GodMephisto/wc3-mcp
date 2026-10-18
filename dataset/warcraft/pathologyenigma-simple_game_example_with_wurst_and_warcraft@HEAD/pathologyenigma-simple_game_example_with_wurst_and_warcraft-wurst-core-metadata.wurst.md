---
type: vendor-doc
source_repo: "https://github.com/pathologyenigma/simple_game_example_with_wurst_and_warcraft"
source_path: "wurst/core/metadata.wurst"
ref: HEAD
---

package metadata
import HashMap
import UnitIndexer
import ClosureTimers
import LinkedList
public tuple ProbilityRateBundle(string name, real probility, real rate)
public tuple ProbilityValueBundle(string name, real probility, real value)
enum TypeName
    String
    Real
    Int
    Timer
function TypeName.toString() returns string
    switch this
        case String
            return "String"
        case Real
            return "Real"
        case Int
            return "Int"
        case Timer
            return "Timer"
        default
            return ""
class Info
    string name
    string typeName
    construct(string name, string typeName)
        this.name = name
        this.typeName = typeName
    function toString() returns string
        return name +"(" + typeName + ")"
public interface CompositeDataType
    function typeName() returns string
    function showInfo(string name)
public enum MaxExpUpAlgorithmType
    AddOneConstantValueEveryTime
    AddOneSelfUpgradeValueEveryTime
    BasedOnCurrentValue
    LevelTimeOneConstantValue
    LevelTimeOneSelfUpgradeValue
public interface MaxExpUpAlgorithm
    // 获取当前的算法类型
    function getType() returns MaxExpUpAlgorithmType
    // 升一级, 并获取升级后的经验需求
    function up(int current, int nextLevel) returns int
public class AddOneSelfUpgradeValueEverTime implements MaxExpUpAlgorithm
    private int baseValue
    private bool upgradeByTimes
    private real upgrade
    // 获取当前的算法类型
    override function getType() returns MaxExpUpAlgorithmType
        return MaxExpUpAlgorithmType.AddOneSelfUpgradeValueEveryTime
    // 升一级, 并获取升级后的经验需求
    override function up(int current, int nextLevel) returns int
        if upgradeByTimes
            return current + (baseValue.toReal() * upgrade).toInt() * nextLevel
        else
            return current + baseValue + (upgrade.toInt() * nextLevel)
    construct(int baseValue, bool upgradeByTimes, real upgrade)
        this.baseValue = baseValue
        this.upgradeByTimes = upgradeByTimes
        this.upgrade = upgrade
public class AddOneConstantValueEveryTime implements MaxExpUpAlgorithm
    private int theConstant
    // 获取当前的算法类型
    override function getType() returns MaxExpUpAlgorithmType
        return MaxExpUpAlgorithmType.AddOneConstantValueEveryTime
    // 升一级, 并获取升级后的经验需求
    override function up(int current, int nextLevel) returns int
        return current + theConstant
    construct(int theConstant)
        this.theConstant = theConstant
public class BasedOnCurrentValue implements MaxExpUpAlgorithm
    private real upgrade
    // 获取当前的算法类型
    override function getType() returns MaxExpUpAlgorithmType
        return MaxExpUpAlgorithmType.BasedOnCurrentValue
    // 升一级, 并获取升级后的经验需求
    override function up(int current, int nextLevel) returns int
        return (current.toReal() * (1 + nextLevel * upgrade)).toInt()
    construct(real upgrade)
        this.upgrade = upgrade
public class LevelTimeOneConstantValue implements MaxExpUpAlgorithm
    private int theConstant
    // 获取当前的算法类型
    override function getType() returns MaxExpUpAlgorithmType
        return MaxExpUpAlgorithmType.LevelTimeOneConstantValue
    // 升一级, 并获取升级后的经验需求
    override function up(int current, int nextLevel) returns int
        return nextLevel * theConstant
    construct(int theConstant)
        this.theConstant = theConstant
public class LevelTimeOneSelfUpgradeValue implements MaxExpUpAlgorithm
    private int baseValue
    private bool upgradeByTimes
    private real upgrade
    // 获取当前的算法类型
    override function getType() returns MaxExpUpAlgorithmType
        return MaxExpUpAlgorithmType.LevelTimeOneSelfUpgradeValue
    // 升一级, 并获取升级后的经验需求
    override function up(int current, int nextLevel) returns int
        if upgradeByTimes
            return (baseValue.toReal() * upgrade).toInt() * nextLevel * nextLevel
        else
            return (baseValue + (upgrade.toInt() * nextLevel)) * nextLevel
    construct(int baseValue, bool upgradeByTimes, real upgrade)
        this.baseValue = baseValue
        this.upgradeByTimes = upgradeByTimes
        this.upgrade = upgrade
public interface AnyTypeInputClosure<T>
    function run(T eventSource, int lvl)
public abstract class OnLvlUp<T> implements AnyTypeInputClosure<T>
    override abstract function run(T eventSource, int lvl)
// 一个复合数据例子
public class Upgrader<T> implements CompositeDataType
    int level
    int exp_current
    int exp_max
    private T source
    private OnLvlUp<T> onLvlUp
    MaxExpUpAlgorithm max_exp_up_algorithm
    override function typeName() returns string
        return "upgrader"
    override function showInfo(string name)
        print(name + "(" + this.typeName() + "): {")
        print("    level(int): " + this.level.toString() + ",")
        print("    exp(int/int): " + this.exp_current.toString() + "/" + this.exp_max.toString())
        print("}")
    construct(T source, int start_lvl, int basic_exp_need, MaxExpUpAlgorithm max_exp_up_algorithm, OnLvlUp<T> onLvlUp)
        this.level = start_lvl
        this.exp_max = basic_exp_need
        this.exp_current = 0
        this.max_exp_up_algorithm = max_exp_up_algorithm
        this.onLvlUp = onLvlUp
        this.source = source
    function gain_exp(int exp)
        this.exp_current += exp
        if exp_current >= this.exp_max
            this.exp_current -= this.exp_max
            lvlUp()
            this.exp_max = this.max_exp_up_algorithm.up(this.exp_max, this.level)
    protected function lvlUp()
        this.level++
        onLvlUp.run(source, this.level)
    function upgradeLvlDirectly(int lvl)
        lvlUp()
        this.exp_max = this.max_exp_up_algorithm.up(this.exp_max, this.level)
public function unit.getData() returns UnitData
    return UnitData.of(this)

public class UnitData
    private static constant map = new HashMap<unit, thistype>

    protected unit u
    protected HashMap<string, real> realVal = new HashMap<string, real>()
    protected HashMap<string, int> intVal = new HashMap<string, int>()
    protected HashMap<string, string> strVal = new HashMap<string, string>()
    protected HashMap<string, CallbackPeriodic> timers = new HashMap<string, CallbackPeriodic>()
    protected HashMap<string, CompositeDataType> compositeDatas = new HashMap<string, CompositeDataType>()
    protected LinkedList<Info> info = new LinkedList<Info>()

    function hasRealVal(string name) returns bool
        return realVal.has(name)

    function hasIntVal(string name) returns bool
        return intVal.has(name)

    function hasStringVal(string name) returns bool
        return strVal.has(name)

    function getRealVal(string name) returns real
        return realVal.get(name)

    function getIntVal(string name) returns int
        return intVal.get(name)

    function getStringVal(string name) returns string
        return strVal.get(name)

    function getCompositeVal(string name) returns CompositeDataType
        return compositeDatas.get(name)

    function destroyTimer(string name)
        if timers.has(name)
            destroy timers.getAndRemove(name)
    
    function setRealVal(string name, real val)
        if not realVal.has(name)
            info.add(new Info(name, TypeName.Real.toString()))
        realVal.put(name, val)

    function setIntval(string name, int val)
        if not intVal.has(name)
            info.add(new Info(name, TypeName.Int.toString()))
        intVal.put(name, val)
    
    function setStringVal(string name, string val)
        if not strVal.has(name)
            info.add(new Info(name, TypeName.String.toString()))
        strVal.put(name, val)
        
    function registerNewTimer(string name, real duration, CallbackPeriodic callback)
        if not strVal.has(name)
            info.add(new Info(name, TypeName.Timer.toString()))
        else
            this.destroyTimer(name)
        timers.put(name, doPeriodically(duration, callback))

    function setCompositeData(string name, CompositeDataType data) 
        if not compositeDatas.has(name)
            info.add(new Info(name, data.typeName()))
        this.compositeDatas.put(name, data)
    function showInfo()
        for i in info
            switch i.typeName
                case "String"
                    print(i.toString() + ":" + this.getStringVal(i.name))
                case "Real"
                    print(i.toString() + ":" + this.getRealVal(i.name).toString())
                case "Int"
                    print(i.toString() + ":" + this.getIntVal(i.name).toString())
                case "Timer"
                    print(i.toString() + ":" + "timer not having info")
                default
                    this.getCompositeVal(i.name).showInfo(i.name)
    function getCriticalBundle() returns ProbilityRateBundle
        return ProbilityRateBundle("暴击数据组", getRealVal("暴击率"), getRealVal("暴击伤害"))
    function getDamageToneUp() returns real
        return getRealVal("增伤")
    function getDamageMitigate() returns real
        return getRealVal("减伤")        
    function getCumstomUpgrader<T>(string name) returns Upgrader<T>
        return getCompositeVal(name) castTo Upgrader<T>
    function setCriticalBundle()
        setCriticalBundle(5, 110)
    function setCriticalBundle(real probility, real rate)
        setRealVal("暴击率", probility)
        setRealVal("暴击倍率", rate)
    function setDamageToneUp()
        setDamageToneUp(0)
    function setDamageToneUp(real rate)
        setRealVal("增伤", rate)
    function setDamageMitigate()
        setDamageMitigate(0)
    function setDamageMitigate(real rate)
        setRealVal("减伤", rate)
    function setBlockBundle(real probility, real value)
        setRealVal("格挡率", probility)
        setRealVal("格挡值", value)
    function setBlockBundle()
        setBlockBundle(0, 0)
    function getBlockBundle() returns ProbilityValueBundle
        return ProbilityValueBundle("格挡数据组", getRealVal("格挡率"), getRealVal("格挡值"))
    construct(unit u)
        this.u = u
        map.put(u, this)
        // doPeriodically(0.2) cb ->
            
    static function of(unit u) returns thistype
        return map.get(u)
    ondestroy
        map.remove(u)
init
    onUnitIndex() -> 
        new UnitData(getIndexingUnit())
    onUnitDeindex() ->
        destroy UnitData.of(getIndexingUnit())