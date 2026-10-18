---
type: vendor-doc
source_repo: "https://github.com/FP-GAME-DEV-TEAM/LostWorldDotA"
source_path: "src/vscripts/abilities/heroes/crystal_maiden/ice_nova.ts"
ref: HEAD
---

import { CreateCommonParticle, IsAliveUnit } from "../../../Core";
import { BaseAbility, registerAbility } from "../../../lib/dota_ts_adapter";
import { modifier_crystal_maiden_freeze_debuff } from "./modifiers/modifier_crystal_maiden_freeze_debuff";
import { modifier_crystal_maiden_ice_armor_buff } from "./modifiers/modifier_crystal_maiden_ice_armor_buff";


@registerAbility()
export class ability_crystal_maiden_ice_nova_ts extends BaseAbility {
    particle?: ParticleID;
    particleName: string = "particles/units/heroes/hero_crystalmaiden/maiden_crystal_nova.vpcf";
    soundName: string = "Hero_Crystal.CrystalNova";

    GetAOERadius(): number {
        return this.GetSpecialValueFor("radius");
    }

    Precache(context: CScriptPrecacheContext): void{
        PrecacheResource("particle", this.particleName, context);
    }

    OnSpellStart(): void {
        const caster = this.GetCaster() as CDOTA_BaseNPC_Hero;
        const point = this.GetCursorPosition();
        const lv = this.GetLevel();
        const radius = this.GetSpecialValueFor("radius");
        const freeze_duration = this.GetSpecialValueFor("freeze_duration");
        const base_dmg = this.GetSpecialValueFor("base_dmg");
        const int_bonus_ratio = this.GetSpecialValueFor("int_bonus_ratio");
        const dmg_bonus_threshold_hp_pct = this.GetSpecialValueFor("dmg_bonus_threshold_hp_pct");
        const dmg_bonus_pct = this.GetSpecialValueFor("dmg_bonus_pct");
        const buff_duration = this.GetSpecialValueFor("buff_duration");
        const buff_armor_bonus = this.GetSpecialValueFor("buff_armor_bonus");
        
        
        CreateCommonParticle(this.particleName, caster, (id) => {
            ParticleManager.SetParticleControl(id, 0, point);
            ParticleManager.SetParticleControl(id, 1, Vector(1,1,radius));
            ParticleManager.SetParticleControl(id, 2, point);
        }, ParticleAttachment.CUSTOMORIGIN);

        EmitSoundOn(this.soundName, caster);
        if(!IsServer()) return;

        const units = FindUnitsInRadius(
            caster.GetTeamNumber(),
            point,
            undefined,
            radius,
            UnitTargetTeam.BOTH,
            this.GetAbilityTargetType(),
            this.GetAbilityTargetFlags(),
            FindOrder.CLOSEST,
            false
        );

        const int = caster.GetIntellect();
        const dmg = base_dmg+int*int_bonus_ratio;
        for(let u of units){
            if(IsAliveUnit(u)){
                if(u.GetTeamNumber() != caster.GetTeamNumber()){
                    // 生命值百分比低于阈值会受到额外伤害
                    const bonus_dmg = u.GetHealthPercent() < dmg_bonus_threshold_hp_pct ? dmg*dmg_bonus_pct*0.01 : 0;
                    ApplyDamage({
                        victim: u,
                        attacker: caster,
                        damage: dmg + bonus_dmg,
                        damage_type: this.GetAbilityDamageType(),
                        ability: this,
                    });

                    u.AddNewModifier(caster, this, modifier_crystal_maiden_freeze_debuff.name, {duration: freeze_duration});
                }else{
                    u.AddNewModifier(caster, this, modifier_crystal_maiden_ice_armor_buff.name, {duration: buff_duration, armor: buff_armor_bonus});
                }
            }
        }
    }
}