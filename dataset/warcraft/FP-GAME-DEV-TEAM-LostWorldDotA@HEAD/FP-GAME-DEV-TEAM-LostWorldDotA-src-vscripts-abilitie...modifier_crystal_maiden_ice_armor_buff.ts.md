---
type: vendor-doc
source_repo: "https://github.com/FP-GAME-DEV-TEAM/LostWorldDotA"
source_path: "src/vscripts/abilities/heroes/crystal_maiden/modifiers/modifier_crystal_maiden_ice_armor_buff.ts"
ref: HEAD
---

import { BaseModifier, registerModifier } from "../../../../lib/dota_ts_adapter";

// 冰霜护甲效果
@registerModifier()
export class modifier_crystal_maiden_ice_armor_buff extends BaseModifier {
    armor: number = 0;

    DeclareFunctions(): ModifierFunction[] {
        return [
            ModifierFunction.PHYSICAL_ARMOR_BONUS,
        ];
    }

    GetEffectAttachType(): ParticleAttachment {
        return ParticleAttachment.POINT_FOLLOW;
    }
    
    GetEffectName(): string {
        return "particles/units/heroes/hero_crystalmaiden/maiden_shard_frostbite.vpcf";
    }

    IsPurgable(): boolean {
        return true;
    }

    IsDebuff(): boolean {
        return true;
    }

    OnCreated(kv: any): void {
        this.armor = kv.armor;
    }

    GetModifierPhysicalArmorBonus(event: ModifierAttackEvent): number {
        return this.armor;
    }
}
