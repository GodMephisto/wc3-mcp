---
type: vendor-doc
source_repo: "https://github.com/pathologyenigma/simple_game_example_with_wurst_and_warcraft"
source_path: "wurst/core/inventory/InventoryItem.wurst"
ref: HEAD
---

package InventoryItem

import HashMap
import LinkedList
import ErrorHandling
public interface Movable
public abstract class InventoryEvent
public abstract class InventoryItem
public interface InventoryEventExecutor
    function run(LinkedList<InventoryEvent> eventArgs)


public class InventoryItemBuilder
    bool isMovable = false
    bool isUsable = false
    bool isCountable = false
    int maxCount = 1
    IterableMap<string,InventoryEventExecutor> executors = new IterableMap<string,InventoryEventExecutor>()
    function setMovable(InventoryEventExecutor exec)
        this.isMovable = true
        this.executors.put("Move", exec)
    function setUsable(InventoryEventExecutor exec)
        this.isUsable = true
        this.executors.put("Use", exec)
    function setCountable(InventoryEventExecutor exec, int maxCount)
        this.isCountable = true
        this.executors.put("Count", exec)
        this.maxCount = maxCount

public class Inventory
    LinkedList<LinkedList<InventoryItem>> storage = new LinkedList<LinkedList<InventoryItem>>()
    int page = 0
    int pageSize = 100
    
    function switchPage(int page)
        if(storage.size() < page)
            error("try to switch to a empty page")
        this.page = page
        this.refresh()

    function move(int index, int target)
        if target > this.pageSize
            error("out of pageSize")
        
    private function refresh()
