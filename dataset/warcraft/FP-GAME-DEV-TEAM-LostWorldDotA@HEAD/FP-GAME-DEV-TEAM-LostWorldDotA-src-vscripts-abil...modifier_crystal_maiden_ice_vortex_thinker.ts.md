---
type: vendor-doc
source_repo: "https://github.com/FP-GAME-DEV-TEAM/LostWorldDotA"
source_path: "src/vscripts/abilities/heroes/crystal_maiden/modifiers/modifier_crystal_maiden_ice_vortex_thinker.ts"
ref: HEAD
---


import { CreateCommonParticle, IsAliveUnit } from "../../../../Core";
import { BaseModifier, registerModifier } from "../../../../lib/dota_ts_adapter";
import { ability_crystal_maiden_ice_vortex_ts } from "../ice_vortex";
import { modifier_crystal_maiden_freeze_debuff } from "./modifier_crystal_maiden_freeze_debuff";


@registerModifier()
export class modifier_crystal_maiden_ice_vortex_thinker extends BaseModifier {
    timer?: string;

    DoProcess(){
        const sk = this.GetAbility() as ability_crystal_maiden_ice_vortex_ts;
        const caster = this.GetCaster() as CDOTA_BaseNPC_Hero;
        const owner = this.GetParent();
        if(sk == null || caster == null || !IsServer()) return;
        const lv = sk.GetLevel();
        const radius = sk.GetSpecialValueFor("radius");
        const base_dmg = sk.GetSpecialValueFor("base_dmg");
        const int_bonus_ratio = sk.GetSpecialValueFor("int_bonus_ratio");

        if(IsServer()){
            const units = FindUnitsInRadius(
                owner.GetTeamNumber(),
                owner.GetAbsOrigin(),
                undefined,
                radius,
                sk.GetAbilityTargetTeam(),
                sk.GetAbilityTargetType(),
                sk.GetAbilityTargetFlags(),
                FindOrder.CLOSEST,
                false
            );
    
            const attr = caster.GetIntellect();
            const dmg = base_dmg+attr*int_bonus_ratio;
            for(let u of units){
                if(IsAliveUnit(u)){
                    ApplyDamage({
                        victim: u,
                        attacker: caster,
                        damage: dmg,
                        damage_type: sk.GetAbilityDamageType(),
                        ability: sk,
                    });

                    u.AddNewModifier(caster, sk, modifier_crystal_maiden_freeze_debuff.name, {duration: 2.5});
                }
            }
        }
    }

    OnCreated(params: object): void {
        const sk = this.GetAbility() as ability_crystal_maiden_ice_vortex_ts;
        const dmg_interval = sk.GetSpecialValueFor("dmg_interval");
        const radius = sk.GetSpecialValueFor("radius");
        const owner = this.GetParent();

        const pcf = ParticleManager.CreateParticle(sk.particleNameGround, ParticleAttachment.WORLDORIGIN, undefined);
        ParticleManager.SetParticleControl(pcf, 0, owner.GetAbsOrigin());
        ParticleManager.SetParticleControl(pcf, 5, Vector(radius, radius, 0));
        this.AddParticle(pcf, false, false, 0, false, false);
        this.StartIntervalThink(dmg_interval);
        this.DoProcess();

        if(!IsServer()) return;
        
        Timers.CreateTimer(()=>{
            const origin = owner.GetAbsOrigin();
            const offset = RandomVector(radius-50);
            const pos = (origin+offset) as Vector;
            CreateCommonParticle(sk.particleName, owner, (id) => {
                ParticleManager.SetParticleControl(id, 0, pos);
                ParticleManager.SetParticleControl(id, 1, pos);
            }, ParticleAttachment.CUSTOMORIGIN);
            EmitSoundOnLocationWithCaster(pos, sk.soundName, owner);
            return 0.33;
        });
    }

    OnDestroy(): void {
        if(this.timer) Timers.RemoveTimer(this.timer);
    }
    
    OnIntervalThink(): void {
        this.DoProcess();
    }
}
