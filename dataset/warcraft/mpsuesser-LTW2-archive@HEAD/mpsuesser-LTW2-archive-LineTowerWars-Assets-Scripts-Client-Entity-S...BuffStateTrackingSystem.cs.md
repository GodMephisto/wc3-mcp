---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Entity/Systems/BuffStateTrackingSystem.cs"
ref: HEAD
---

﻿using System.Collections.Generic;
using System;

public class BuffStateTrackingSystem {
    public event Action<BuffType> OnBuffOfTypeApplied;
    public event Action<BuffType> OnBuffOfTypeRemoved;
    public event Action<ClientEntity> OnBuffsUpdated;
    
    private ClientEntity E { get; }
    
    private Dictionary<int, BuffState> BuffStatesByID { get; }
    private HashSet<BuffType> ActiveBuffTypes { get; }
    
    public BuffStateTrackingSystem(ClientEntity entity) {
        E = entity;

        BuffStatesByID = new Dictionary<int, BuffState>();
        ActiveBuffTypes = new HashSet<BuffType>();
    }

    public void Add(BuffState bs) {
        BuffStatesByID[bs.ID] = bs;
        ActiveBuffTypes.Add(bs.Type);
        OnBuffsUpdated?.Invoke(E);
        OnBuffOfTypeApplied?.Invoke(bs.Type);
    }

    public BuffState GetBuffStateByBuffID(int buffID) {
        AssertBuffStateWithBuffID(buffID);
        return BuffStatesByID[buffID];
    }

    public void RemoveBuffStateByBuffID(int buffID) {
        AssertBuffStateWithBuffID(buffID);
        BuffState bs = BuffStatesByID[buffID];
        BuffStatesByID.Remove(buffID);
        ActiveBuffTypes.Remove(bs.Type);
        OnBuffsUpdated?.Invoke(E);
        OnBuffOfTypeRemoved?.Invoke(bs.Type);
    }

    private void AssertBuffStateWithBuffID(int buffID) {
        if (!BuffStatesByID.ContainsKey(buffID)) {
            throw new BuffStateNotFoundException(buffID);
        }
    }

    public List<BuffState> GetAll() {
        return new List<BuffState>(BuffStatesByID.Values);
    }

    public bool ContainsBuffOfType(BuffType buffType) {
        return ActiveBuffTypes.Contains(buffType);
    }
}