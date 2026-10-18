---
type: vendor-doc
source_repo: "https://github.com/FP-GAME-DEV-TEAM/LostWorldDotA"
source_path: "src/vscripts/GameMode.ts"
ref: HEAD
---

import { OnNpcSpawned, OnPlayerChat } from "./EventHandler";
import { Precache } from "./Precache";
import { Settings } from "./Settings";
import { reloadable } from "./lib/tstl-utils";

declare global {
    interface CDOTAGameRules {
        Addon: GameMode;
    }
}

@reloadable
export class GameMode {
    public static Precache(this: void, context: CScriptPrecacheContext) {
        Precache(context);
    }

    public static Activate(this: void) {
        GameRules.Addon = new GameMode();
    }

    constructor() {
        this.configure();

        // Register event listeners for dota engine events
        ListenToGameEvent("game_rules_state_change", () => this.OnStateChange(), undefined);
        ListenToGameEvent("npc_spawned", event => OnNpcSpawned(event), undefined);
        ListenToGameEvent("player_chat", event => OnPlayerChat(event), undefined);
    }

    private configure(): void {
        // 一些游戏规则的设定
        GameRules.GetGameModeEntity().SetAnnouncerDisabled(true);       // 播音员
        GameRules.GetGameModeEntity().SetRemoveIllusionsOnDeath(true);   // 死亡时幻象消失
        GameRules.GetGameModeEntity().SetDaynightCycleDisabled(true);    // 日夜交替
        GameRules.GetGameModeEntity().SetStashPurchasingDisabled(false); // 储藏处
        GameRules.GetGameModeEntity().SetSendToStashEnabled(true);       // 传送到储藏处
        GameRules.GetGameModeEntity().SetRandomHeroBonusItemGrantDisabled(true); // 随机英雄奖励
        GameRules.GetGameModeEntity().SetForceRightClickAttackDisabled(true);   // 右键强制攻击
        GameRules.GetGameModeEntity().DisableClumpingBehaviorByDefault(true);
        GameRules.GetGameModeEntity().SetNeutralStashTeamViewOnlyEnabled(true);  // 中立物品仅同队可见？
        GameRules.GetGameModeEntity().SetNeutralItemHideUndiscoveredEnabled(true);
        GameRules.GetGameModeEntity().SetGiveFreeTPOnDeath(false) // 死亡时获得tp
        GameRules.GetGameModeEntity().SetBuybackEnabled(false)    // 买活
        GameRules.GetGameModeEntity().SetPlayerHeroAvailabilityFiltered(false)
        GameRules.GetGameModeEntity().SetLoseGoldOnDeath(false)   // 死亡掉钱
        GameRules.GetGameModeEntity().SetFriendlyBuildingMoveToEnabled(true)
        GameRules.GetGameModeEntity().SetHudCombatEventsDisabled(true)  // 战斗回放界面
        GameRules.GetGameModeEntity().SetWeatherEffectsDisabled(true)   // 天气效果
        GameRules.GetGameModeEntity().SetSelectionGoldPenaltyEnabled(false) // 选英雄超时扣钱
        GameRules.GetGameModeEntity().SetDefaultStickyItem("item_bottle") // 快速购买物品
        GameRules.GetGameModeEntity().SetTPScrollSlotItemOverride("item_bottle")  // 默认物品格的物品
        // GameRules.GetGameModeEntity().SetCustomGameForceHero("npc_dota_hero_jakiro"); // 强制选择英雄，自制选英雄UI时需要使用
        GameRules.SetCustomGameAllowHeroPickMusic(false)
        GameRules.SetCustomGameAllowBattleMusic(false)
        GameRules.SetCustomGameAllowMusicAtGameStart(true)
        GameRules.SetAllowOutpostBonuses(false);    // 前哨奖励
        GameRules.SetSameHeroSelectionEnabled(true);
        GameRules.SetUseUniversalShopMode(false);  // 全局商店
        GameRules.SetEnableAlternateHeroGrids(false);
        GameRules.SetHeroRespawnEnabled(true);

        // 设置队伍人数
        GameRules.SetCustomGameTeamMaxPlayers(DotaTeam.GOODGUYS, 4);
        GameRules.SetCustomGameTeamMaxPlayers(DotaTeam.BADGUYS, 4);
        GameRules.SetCustomGameTeamMaxPlayers(DotaTeam.CUSTOM_1, 4);
        GameRules.SetCustomGameTeamMaxPlayers(DotaTeam.CUSTOM_2, 4);
        GameRules.SetCustomGameTeamMaxPlayers(DotaTeam.CUSTOM_3, 4);
        GameRules.SetCustomGameTeamMaxPlayers(DotaTeam.CUSTOM_4, 4);
        GameRules.SetCustomGameTeamMaxPlayers(DotaTeam.CUSTOM_5, 4);
        GameRules.SetCustomGameTeamMaxPlayers(DotaTeam.CUSTOM_6, 4);
        GameRules.SetCustomGameTeamMaxPlayers(DotaTeam.CUSTOM_7, 4);
        GameRules.SetCustomGameTeamMaxPlayers(DotaTeam.CUSTOM_8, 4);

        // 数值类设置
        GameRules.SetTimeOfDay(0.251);
        GameRules.SetTreeRegrowTime(60.0); // 树木重生间隔
        GameRules.SetCustomGameSetupTimeout(0);
        GameRules.SetCustomGameSetupAutoLaunchDelay(0); // setup阶段
        GameRules.SetStrategyTime(0.0);   // 策略阶段时长
        GameRules.SetShowcaseTime(0.0);   // 进入游戏前动画过长时长
        GameRules.SetPreGameTime(3.0);    // ?
        GameRules.SetPostGameTime(45.0);  // 好像是出兵前的时长
        GameRules.SetHeroSelectionTime(Settings.HeroPickTimeLimit);   // 选英雄阶段时长
        GameRules.GetGameModeEntity().SetCameraSmoothCountOverride(2)
        GameRules.GetGameModeEntity().SetFixedRespawnTime(Settings.HeroReviveTime)
        GameRules.GetGameModeEntity().SetMinimumAttackSpeed(0.5)
        GameRules.GetGameModeEntity().SetCameraZRange(11, 3800)
        GameRules.GetGameModeEntity().SetCameraDistanceOverride(1388);


        // 初始金钱和工资设定
        GameRules.SetStartingGold(Settings.PlayerInitGold);
        GameRules.SetGoldTickTime(999999.0);
        GameRules.SetGoldPerTick(0);
        for (let i = 0; i < DOTA_MAX_TEAM_PLAYERS; i++) {
            if (PlayerResource.IsValidPlayerID(i)) {
                PlayerResource.SetGold(i, Settings.PlayerInitGold, true);
            }
        }

        // GameRules.GetGameModeEntity().SetDamageFilter((event) => ds_damage_filter(event), this);
        // GameRules.GetGameModeEntity().SetHealingFilter((event) => hs_heal_filter(event), this);
    }

    public OnStateChange(): void {
        const state = GameRules.State_Get();

        // Add 4 bots to lobby in tools
        if (IsInToolsMode() && state == GameState.CUSTOM_GAME_SETUP) {
            for (let i = 0; i < 3; i++) {
                Tutorial.AddBot("npc_dota_hero_lina", "", "", false);
            }
        }

        if (state === GameState.CUSTOM_GAME_SETUP) {
            // Automatically skip setup in tools
            if (IsInToolsMode()) {
                Timers.CreateTimer(3, () => {
                    GameRules.FinishCustomGameSetup();
                });
            }
        }

        // Start game once pregame hits
        if (state === GameState.PRE_GAME) {
            Timers.CreateTimer(0.2, () => this.StartGame());
        }
    }

    private StartGame(): void {
        print("Game starting!");

    }

    // Called on script_reload
    public Reload() {
        print("Script reloaded!");

    }

}
