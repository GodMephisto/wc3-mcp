---
type: vendor-doc
source_repo: "https://github.com/FP-GAME-DEV-TEAM/LostWorldDotA"
source_path: "src/vscripts/abilities/heroes/crystal_maiden/ice_vortex.ts"
ref: HEAD
---

import { BaseAbility, registerAbility } from "../../../lib/dota_ts_adapter";
import { modifier_crystal_maiden_ice_vortex_thinker } from "./modifiers/modifier_crystal_maiden_ice_vortex_thinker";

@registerAbility()
export class ability_crystal_maiden_ice_vortex_ts extends BaseAbility {
    particle?: ParticleID;
    particleName: string = "particles/units/heroes/hero_crystalmaiden/maiden_freezing_field_explosion.vpcf";
    particleNameProj: string = "particles/units/heroes/hero_crystalmaiden/maiden_freezing_field_explosion.vpcf";
    particleNameGround: string = "particles/units/heroes/hero_ancient_apparition/ancient_ice_vortex.vpcf";
    soundName: string = "hero_Crystal.freezingField.explosion";

    Precache(context: CScriptPrecacheContext): void{
        PrecacheResource("particle", this.particleName, context);
        PrecacheResource("particle", this.particleNameProj, context);
        PrecacheResource("particle", this.particleNameGround, context);
    }

    GetAOERadius(): number {
        return this.GetSpecialValueFor("radius");
    }

    OnSpellStart(): void {
        const caster = this.GetCaster() as CDOTA_BaseNPC_Hero;
        const point = this.GetCursorPosition();
        const duration = this.GetSpecialValueFor("duration");
        
        EmitSoundOn(this.soundName, caster);
        if(!IsServer()) return;
        CreateModifierThinker(caster, this, modifier_crystal_maiden_ice_vortex_thinker.name, {duration: duration}, point, caster.GetTeamNumber(), false);
    }
}