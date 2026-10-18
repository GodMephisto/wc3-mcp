---
type: vendor-doc
source_repo: "https://github.com/FP-GAME-DEV-TEAM/LostWorldDotA"
source_path: "src/vscripts/abilities/heroes/crystal_maiden/modifiers/modifier_crystal_maiden_silence_debuff.ts"
ref: HEAD
---

import { BaseModifier, registerModifier } from "../../../../lib/dota_ts_adapter";

// 沉默效果
@registerModifier()
export class modifier_crystal_maiden_silence_debuff extends BaseModifier {
    CheckState(): Partial<Record<ModifierState, boolean>> {
        return {
            [ModifierState.SILENCED]: true,
        };
    }

    GetEffectAttachType(): ParticleAttachment {
        return ParticleAttachment.OVERHEAD_FOLLOW;
    }
    
    GetEffectName(): string {
        return "particles/generic_gameplay/generic_silence.vpcf";
    }

    IsPurgable(): boolean {
        return true;
    }

    IsDebuff(): boolean {
        return true;
    }
}
