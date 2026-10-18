---
type: vendor-doc
source_repo: "https://github.com/FP-GAME-DEV-TEAM/LostWorldDotA"
source_path: "src/vscripts/EventHandler.ts"
ref: HEAD
---

//================================
// 事件处理
//================================

import { Settings } from "./Settings";

export function OnNpcSpawned(event: NpcSpawnedEvent) {
    // After a hero unit spawns, apply modifier_panic for 8 seconds
    const unit = EntIndexToHScript(event.entindex) as CDOTA_BaseNPC;

    if (unit.IsRealHero()) {

    }
}

// 聊天事件
export function OnPlayerChat(event: PlayerChatEvent){
    if(!IsServer()) return;
    const pid = event.playerid;
    const msg = event.text;
    const pl = PlayerResource.GetPlayer(pid)!;
    const hero = pl.GetAssignedHero();

    // 开发阶段测试指令
    if(IsInToolsMode()){
        // 创建物品
        if(msg.startsWith("=item")){
            const itemname = msg.substring(6);
            const it = CreateItem(itemname, undefined, undefined)!; 
            it.SetPurchaseTime(0);
            it.SetPurchaser(hero);
            it.SetOwner(hero);
            const pos = (hero.GetAbsOrigin()) as Vector;
            CreateItemOnPositionForLaunch(pos, it);
            it.LaunchLoot(false, 300, 0.75, pos, null!);
        }else if (msg.startsWith("=ffa")){
            const teams = [DotaTeam.GOODGUYS,DotaTeam.BADGUYS,DotaTeam.CUSTOM_1,DotaTeam.CUSTOM_2,DotaTeam.CUSTOM_3,DotaTeam.CUSTOM_4,DotaTeam.CUSTOM_5,DotaTeam.CUSTOM_6];
            for(let i=0; i<Settings.MaxPlayerCount; i++){
                if(PlayerResource.IsValidPlayerID(i)){
                    print(i, PlayerResource.GetPlayer(i)?.GetAssignedHero().GetName())
                    PlayerResource.UpdateTeamSlot(i, teams[i], 1);
                    PlayerResource.GetPlayer(i)?.SetTeam(teams[i]);
                    PlayerResource.SetCustomTeamAssignment(i, teams[i]);
                    PlayerResource.GetPlayer(i)?.GetAssignedHero().RespawnHero(false, false);
                }
            }
        }else if (msg.startsWith("=testffa")){
            const teams = [DotaTeam.GOODGUYS,DotaTeam.BADGUYS,DotaTeam.CUSTOM_1,DotaTeam.CUSTOM_2,DotaTeam.CUSTOM_3,DotaTeam.CUSTOM_4,DotaTeam.CUSTOM_5,DotaTeam.CUSTOM_6];
            for(let i=0; i<Settings.MaxPlayerCount; i++){
                const u = CreateUnitByName("npc_dota_neutral_kobold_tunneler", RandomVector(500), true, undefined, undefined, teams[i]);
                
            }
        }
    }
}