---
type: vendor-doc
source_repo: "https://github.com/FP-GAME-DEV-TEAM/LostWorldDotA"
source_path: "src/vscripts/Core.ts"
ref: HEAD
---

//================================
// 顶层工具类
//================================


// 数组乱序
export function ArrayShuffle<T> (lt: Array<T>) : Array<T>{
    if(lt == undefined || lt.length == 0) return lt;

    for(let i=lt.length-1; i>=1; i--){
        const j = RandomInt(0, i);
        const t = lt[i];
        lt[i] = lt[j];
        lt[j] = t;
    }

    return lt;
}

// 格式化输出table，仅输出深度为1的属性
export function PrintTable(kv: any, key="", inner: (a : any) => string = ()=>""){
    if(!kv){
        print("nil");
        return;
    }
    print(key+"{");
    for(let k in kv){
        print("\t"+k + ":" + kv[k] + "\t" + inner(kv[k]));
    }
    print("}");
}

// 判断是否有魔晶
export function HasShard(u: CDOTA_BaseNPC | undefined) : boolean{
    if(!u) return false;
    return u.HasModifier("modifier_item_aghanims_shard");
}

// 判断单位是否存活
export function IsAliveUnit(u: CDOTA_BaseNPC | undefined) : u is CDOTA_BaseNPC{
    if(!u || u == undefined || u.IsNull()) return false;
    return u.IsAlive();
}

// 创建特效
export function CreateCommonParticle(particleName: string, target: CBaseEntity | undefined, callback?: (p:ParticleID)=>void, attach: ParticleAttachment = ParticleAttachment.ABSORIGIN_FOLLOW) : void{
    const particle = ParticleManager.CreateParticle(particleName, attach, target);
    if(callback) callback(particle);
    ParticleManager.ReleaseParticleIndex(particle);
}

// 创建延时特效
export function CreateDelayParticle(particleName: string, target: CBaseEntity | undefined, delay: number, callback?: (p:ParticleID)=>void, attach: ParticleAttachment = ParticleAttachment.ABSORIGIN_FOLLOW) : void{
    if(delay <= 0) delay = FrameTime();
    const particle = ParticleManager.CreateParticle(particleName, attach, target);
    if(callback) callback(particle);
    Timers.CreateTimer(delay, () => {
        ParticleManager.DestroyParticle(particle, true);
    });
}

// 吸血特效
export function CreateParticle_Lifesteal(target: CBaseEntity | undefined) : void{
    CreateCommonParticle("particles/items3_fx/octarine_core_lifesteal.vpcf", target);
}

// 客户端显示错误信息
export function DisplayError(playerID: PlayerID, message: string){
    if(playerID == -1){
        CustomGameEventManager.Send_ServerToAllClients("CreateIngameErrorMessage", {message:message})
        return;
    }
    const player = PlayerResource.GetPlayer(playerID);
    if(player){
        CustomGameEventManager.Send_ServerToPlayer(player, "CreateIngameErrorMessage", {message:message})
    }
}