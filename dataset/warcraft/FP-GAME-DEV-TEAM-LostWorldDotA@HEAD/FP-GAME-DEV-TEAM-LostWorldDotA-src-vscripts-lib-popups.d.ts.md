---
type: vendor-doc
source_repo: "https://github.com/FP-GAME-DEV-TEAM/LostWorldDotA"
source_path: "src/vscripts/lib/popups.d.ts"
ref: HEAD
---


declare interface Popup{
    Healing(target: CDOTA_BaseNPC, amount: number, player?: CDOTAPlayerController): void;
    Damage(target: CDOTA_BaseNPC, amount: number, player?: CDOTAPlayerController): void;
    DamageColored(target: CDOTA_BaseNPC, amount: number, color: Vector, player?: CDOTAPlayerController): void;
    CriticalDamage(target: CDOTA_BaseNPC, amount: number, player?: CDOTAPlayerController): void;
    CriticalDamageColored(target: CDOTA_BaseNPC, amount: number, color: Vector, player?: CDOTAPlayerController): void;
    DamageOverTime(target: CDOTA_BaseNPC, amount: number, player?: CDOTAPlayerController): void;
    DamageBlock(target: CDOTA_BaseNPC, amount: number, player?: CDOTAPlayerController): void;
    GoldGain(target: CDOTA_BaseNPC, amount: number, player?: CDOTAPlayerController): void;
    ManaGain(target: CDOTA_BaseNPC, amount: number, player?: CDOTAPlayerController): void;
    Miss(target: CDOTA_BaseNPC, player?: CDOTAPlayerController): void;
    DamageBig(target: CDOTA_BaseNPC, amount: number, player?: CDOTAPlayerController): void;
    AddGold(target: CDOTA_BaseNPC, amount: number, player?: CDOTAPlayerController): void;
}

declare var Popup: Popup;