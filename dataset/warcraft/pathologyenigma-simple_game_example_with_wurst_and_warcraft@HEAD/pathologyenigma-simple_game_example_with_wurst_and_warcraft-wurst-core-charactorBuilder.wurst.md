---
type: vendor-doc
source_repo: "https://github.com/pathologyenigma/simple_game_example_with_wurst_and_warcraft"
source_path: "wurst/core/charactorBuilder.wurst"
ref: HEAD
---

package charactorBuilder

import public metadata

public class CharacterBuilder
    private UnitData data
    private unit source
    construct(unit source)
        this.data = source.getData()
        this.source = source
        setCriticalBundle()
        setDamageToneUp()
        setDamageMitigate()
        setBlockBundle()
        OnLvlUp<unit> onLvlUp = (unit source, int lvl) -> 
            source.setMaxHP((source.getMaxHP() * (1 + lvl.toReal() * 0.02)).toInt())
            source.setHP(source.getMaxHP())
        setCumstomUpgrader<unit>("lvl_upgrader",
         new Upgrader<unit>(this.source, 0, 100, new AddOneConstantValueEveryTime(100),onLvlUp)
         )
    private function setCompositeData(string name, CompositeDataType data)
        this.data.setCompositeData(name, data)
    // private function setRealData(string name, real data)
    //     this.data.setRealVal(name, data)
    function setCriticalBundle()
        this.data.setCriticalBundle()
    function setCriticalBundle(real probility, real rate)
        this.data.setCriticalBundle(probility, rate)
    function setDamageToneUp()
        this.data.setDamageToneUp()
    function setDamageToneUp(real rate)
        this.data.setDamageToneUp(rate)
    function setDamageMitigate()
        this.data.setDamageMitigate()
    function setDamageMitigate(real rate)
        this.data.setDamageMitigate(rate)
    
    function setBlockBundle(real probility, real value)
        this.data.setBlockBundle(probility, value)
    function setBlockBundle()
        this.data.setBlockBundle()
   
    // 看似有点难理解，实际上就是可以绑定到任意类型的升级组件
    // 举个例子，你有一个技能，这个技能的伤害数值和技能本身相互独立
    // 现在你需要有一个技能熟练度系统，你可以考虑使用这个组件来操作
    // 此时这里的T就是那个伤害数值的类型（real或者int，如果是持续伤害可能是一组数据）
    function setCumstomUpgrader<T>(string name, Upgrader<T> upgrader)
        setCompositeData(name, upgrader)
    
public function unit.builder() returns CharacterBuilder
    return new CharacterBuilder(this)