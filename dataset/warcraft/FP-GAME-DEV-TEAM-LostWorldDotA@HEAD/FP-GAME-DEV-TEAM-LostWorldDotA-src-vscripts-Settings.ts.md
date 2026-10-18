---
type: vendor-doc
source_repo: "https://github.com/FP-GAME-DEV-TEAM/LostWorldDotA"
source_path: "src/vscripts/Settings.ts"
ref: HEAD
---

//================================
// 全局游戏设定参数
//================================

export class Settings{
    static _TestMode: boolean = false;

    // 游戏当前环境
    public static get Environment(): string {
        if(IsInToolsMode()) return "Alpha";
        if(this._TestMode == true) return "Beta";
        return "Stable";
    }

    // 开启测试模式
    public static OpenTestMode() : void {
        this._TestMode = true;
    }

    // 最大玩家人数
    public static get MaxPlayerCount(): number {
        return 8;
    }

    // 地图版本
    public static get Version(): number {
        if(IsInToolsMode()) return 1;
        return 20230620;
    }

    // 默认镜头高度
    public static get CameraHeight(): number {
        if(IsInToolsMode()) return 1388;
        return 1388;
    }

    // 玩家初始金钱
    public static get PlayerInitGold(): number{
        if(IsInToolsMode()) return 88888;
        return 1200;
    }
    
    // 英雄选择阶段限定时间
    public static get HeroPickTimeLimit() : number {
        if(IsInToolsMode()) return 1500;
        return 120;
    }

    // 英雄复活时间
    public static get HeroReviveTime() : number {
        if(IsInToolsMode()) return 15;
        return 20;
    }

}