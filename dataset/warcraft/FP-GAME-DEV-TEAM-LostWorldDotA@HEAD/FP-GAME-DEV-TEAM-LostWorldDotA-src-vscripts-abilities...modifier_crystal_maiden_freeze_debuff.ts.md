---
type: vendor-doc
source_repo: "https://github.com/FP-GAME-DEV-TEAM/LostWorldDotA"
source_path: "src/vscripts/abilities/heroes/crystal_maiden/modifiers/modifier_crystal_maiden_freeze_debuff.ts"
ref: HEAD
---

import { BaseModifier, registerModifier } from "../../../../lib/dota_ts_adapter";

// 霜冻减速效果
@registerModifier()
export class modifier_crystal_maiden_freeze_debuff extends BaseModifier {
    DeclareFunctions(): ModifierFunction[] {
        return [
            ModifierFunction.MOVESPEED_BONUS_PERCENTAGE,
            ModifierFunction.ATTACKSPEED_PERCENTAGE,
        ];
    }

    GetEffectAttachType(): ParticleAttachment {
        return ParticleAttachment.POINT_FOLLOW;
    }
    
    GetEffectName(): string {
        return "particles/generic_gameplay/generic_slowed_cold.vpcf";
    }

    IsPurgable(): boolean {
        return true;
    }

    IsDebuff(): boolean {
        return true;
    }

    GetModifierAttackSpeedPercentage(): number {
        return -25;
    }

    GetModifierMoveSpeedBonus_Percentage(): number {
        return -50;
    }
}
