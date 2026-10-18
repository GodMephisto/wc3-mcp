---
type: vendor-doc
source_repo: "https://github.com/FP-GAME-DEV-TEAM/LostWorldDotA"
source_path: "src/vscripts/abilities/heroes/crystal_maiden/ice_missile.ts"
ref: HEAD
---


import { CreateCommonParticle, IsAliveUnit } from "../../../Core";
import { BaseAbility, registerAbility } from "../../../lib/dota_ts_adapter";
import { modifier_crystal_maiden_freeze_debuff } from "./modifiers/modifier_crystal_maiden_freeze_debuff";
import { modifier_crystal_maiden_silence_debuff } from "./modifiers/modifier_crystal_maiden_silence_debuff";

//向目标发射带有冰霜能量的魔法飞弹，对目标单位造成(智力*3)点伤害和短暂的昏迷，并对目标及其周围的敌人造成100点伤害和5秒的霜冻减速效果。|n若目标带有霜冻效果，则还会对目标造成6秒的沉默。|n|n|
@registerAbility()
export class ability_crystal_maiden_ice_missile_ts extends BaseAbility {
    particle?: ParticleID;
    particleName: string = "particles/units/heroes/hero_ancient_apparition/ancient_apparition_chilling_touch_projectile.vpcf";
    particleImpactName: string = "particles/units/heroes/hero_ancient_apparition/ancient_apparition_chilling_touch_projectile_hit.vpcf";
    soundName: string = "Hero_Ancient_Apparition.ChillingTouch.Cast";
    soundNameImpact: string = "Hero_Ancient_Apparition.ChillingTouch.Target";
        
    Precache(context: CScriptPrecacheContext): void{
        PrecacheResource("particle", this.particleName, context);
    }

    GetAOERadius(): number {
        return this.GetSpecialValueFor("radius");
    }

    OnSpellStart(): void {
        const caster = this.GetCaster() as CDOTA_BaseNPC_Hero;
        const target = this.GetCursorTarget()!;
        

        EmitSoundOn(this.soundName, caster);
        if(!IsServer()) return;

        ProjectileManager.CreateTrackingProjectile({
            EffectName: this.particleName,
            Target: target,
            Ability: this,
            Source: caster,
            bProvidesVision: false,
            iMoveSpeed: 1000,
            bDodgeable: false,
            bIsAttack: false,
            iSourceAttachment: ProjectileAttachment.HITLOCATION,
            bDrawsOnMinimap: false,
        });
    }

    // 检测并执行沉默目标
    CheckAndSilenceTarget(target: CDOTA_BaseNPC){
        const caster = this.GetCaster() as CDOTA_BaseNPC_Hero;
        const silence_duration_for_unit = this.GetSpecialValueFor("silence_duration_for_unit");
        const silence_duration_for_hero = this.GetSpecialValueFor("silence_duration_for_hero");
        if(target.HasModifier(modifier_crystal_maiden_freeze_debuff.name)){
            const duration = target.IsHero() ? silence_duration_for_hero : silence_duration_for_unit;
            target.AddNewModifier(caster, this, modifier_crystal_maiden_silence_debuff.name, {duration: duration}); 
        }
    }

    OnProjectileHit(target: CDOTA_BaseNPC | undefined, location: Vector): boolean | void {
        const caster = this.GetCaster() as CDOTA_BaseNPC_Hero;
        const base_dmg = this.GetSpecialValueFor("base_heal");
        const int_bonus_ratio = this.GetSpecialValueFor("int_bonus_ratio");
        const freeze_duration = this.GetSpecialValueFor("freeze_duration");
        const stun_duration = this.GetSpecialValueFor("stun_duration");
        const radius = this.GetSpecialValueFor("radius");
        

        EmitSoundOn(this.soundNameImpact, caster);

        const int = caster.GetIntellect();
        const dmg = base_dmg+int*int_bonus_ratio;

        // 对技能目标的伤害和效果
        if(IsAliveUnit(target)){
            this.CheckAndSilenceTarget(target);
            target.AddNewModifier(caster, this, "modifier_stunned", {duration: stun_duration});
            target.AddNewModifier(caster, this, modifier_crystal_maiden_freeze_debuff.name, {duration: freeze_duration});
            ApplyDamage({
                victim: target,
                attacker: caster,
                damage: dmg,
                damage_type: this.GetAbilityDamageType(),
                ability: this,
            });
        }
        
        // 对周围单位的伤害和效果，即使目标死亡，这部分依然生效
        const units = FindUnitsInRadius(
            caster.GetTeamNumber(),
            location,
            undefined,
            radius,
            this.GetAbilityTargetTeam(),
            this.GetAbilityTargetType(),
            this.GetAbilityTargetFlags(),
            FindOrder.CLOSEST,
            false
        );

        for(let u of units){
            if(IsAliveUnit(u) && u != target){
                CreateCommonParticle(this.particleImpactName, u, (id)=>{
                    ParticleManager.SetParticleControl(id, 1, u.GetAbsOrigin());
                });
                this.CheckAndSilenceTarget(u);
                u.AddNewModifier(caster, this, modifier_crystal_maiden_freeze_debuff.name, {duration: freeze_duration});
                ApplyDamage({
                    victim: u,
                    attacker: caster,
                    damage: dmg,
                    damage_type: this.GetAbilityDamageType(),
                    ability: this,
                });
            }
        }
        

        return true;
    }
}