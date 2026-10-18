---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/TargetSystem/TargetSystem.cs"
ref: HEAD
---

using System;
using System.Collections.Generic;
using UnityEngine;

public class TargetSystem : SingletonBehaviour<TargetSystem>,
                            IHandleKeyDownInput,
                            IHandleKeyUpInput,
                            IHandleKeyContinuousInput
{
    private bool TargetExists => Targets.Count > 0;
    public List<ClientEntity> Targets { get; private set; }

    private Dictionary<ClientEntity, GameObject> selectionSprites;
    private Dictionary<ClientEntity, GameObject> hoverSprites;
    public HashSet<ClientEntity> HoverUnits =>
        new HashSet<ClientEntity>(hoverSprites.Keys);

    private readonly LayerMask EntityLayerMask =
        LayerMaskConstants.EntityLayerMask;
    private LayerMask GroundLayerMask = 
        LayerMaskConstants.EnvironmentLayerMask | LayerMaskConstants.LaneLayerMask;
    private Camera cam;

    // Box select
    private bool dragSelect;
    private bool dragEligible;
    private Vector3 dragStartPos;
    private MeshCollider selectionBox;
    private HashSet<ClientEntity> currentlyInBox;

    // Double click LM tracker
    private (ClientEntity entity, float time) lastSelect;
    
    // For tower upgrade forward-replacement in current target selection
    private HashSet<int> entityIDsToTargetUponCreation;

    private void Awake() {
        InitializeSingleton(this);

        Targets = new List<ClientEntity>();

        selectionSprites = new Dictionary<ClientEntity, GameObject>();
        hoverSprites = new Dictionary<ClientEntity, GameObject>();
        currentlyInBox = new HashSet<ClientEntity>();
        
        entityIDsToTargetUponCreation = new HashSet<int>();
        
        cam = Camera.main;

        dragSelect = false;
        selectionBox = null;
        
        EventBus.OnTowerSaleFinished += RemoveEntityIfTargeted;
        EventBus.OnTowerUpgradeFinished += ReplaceEntityIfTargeted;
        EventBus.OnTowerSpawnPost += AddTowerToTargetsIfInCreationQueue;
        EventBus.OnSetActiveInterfaceState += HandleInterfaceActiveInterfaceChanged;
    }

    private void OnDestroy() {
        EventBus.OnTowerSaleFinished -= RemoveEntityIfTargeted;
        EventBus.OnTowerUpgradeFinished -= ReplaceEntityIfTargeted;
        EventBus.OnTowerSpawnPost -= AddTowerToTargetsIfInCreationQueue;
        EventBus.OnSetActiveInterfaceState -= HandleInterfaceActiveInterfaceChanged;
    }

    private void HandleInterfaceActiveInterfaceChanged(InterfaceState activeInterface) {
        dragSelect = false;
        ClearSelectionBox();
        if (!InGameInterfaceStateSystem.Singleton.IsTargetSelectedInterfaceInActiveStack()) {
            DeselectAll();
        }
    }
    
    private void RemoveEntityIfTargeted(ClientEntity e) {
        if (!Targets.Contains(e)) {
            return;
        }
        
        DeselectEntity(e);
    }

    private void ReplaceEntityIfTargeted(ClientEntity e, int newEntityID) {
        if (!Targets.Contains(e)) {
            return;
        }

        DeselectEntity(e);
        
        try {
            // Add the new entity to our targets list if it exists now...
            ClientEntity alreadyExistingEntity = ClientEntityStorageSystem.Singleton.GetEntityByID(newEntityID);
            SelectEntity(alreadyExistingEntity);
        }
        catch (NotFoundException) {
            // It doesn't exist at the moment, so add it to the list when it's created in the near future
            entityIDsToTargetUponCreation.Add(newEntityID);
        }
    }

    private void AddTowerToTargetsIfInCreationQueue(ClientTower tower) {
        if (!entityIDsToTargetUponCreation.Contains(tower.ID)) return;
        
        SelectEntity(tower);
        entityIDsToTargetUponCreation.Remove(tower.ID);
    }

    private void Update() {
        ClearSelectionBox();
        CheckForHover();
    }

    private void ClearSelectionBox() {
        if (selectionBox == null || dragSelect) {
            return;
        }
        
        Destroy(selectionBox);
        selectionBox = null;
    }

    #region Input Handling

    private static readonly HashSet<KeyCode> keyDownSubscriptions =
        new HashSet<KeyCode>() {
            KeyCode.Mouse0,
        };
    public HashSet<KeyCode> KeyDownSubscriptions => keyDownSubscriptions;
    public bool HandleInputKeyDown(KeyCode kc) {
        return kc switch {
            KeyCode.Mouse0 => HandleLeftMouseDown(),
            _ => false
        };
    }
    
    private static readonly HashSet<KeyCode> keyUpSubscriptions =
        new HashSet<KeyCode>() {
            KeyCode.Mouse0,
        };
    public HashSet<KeyCode> KeyUpSubscriptions => keyUpSubscriptions;
    public bool HandleInputKeyUp(KeyCode kc) {
        return kc switch {
            KeyCode.Mouse0 => HandleLeftMouseUp(),
            _ => false
        };
    }
    
    private static readonly HashSet<KeyCode> keyContinuousSubscriptions =
        new HashSet<KeyCode>() {
            KeyCode.Mouse0
        };
    public HashSet<KeyCode> KeyContinuousSubscriptions => keyContinuousSubscriptions;
    public bool HandleInputKeyContinuous(KeyCode kc) {
        return kc switch {
            KeyCode.Mouse0 => HandleLeftMouseClickContinuous(),
            _ => false
        };
    }

    private bool HandleLeftMouseDown() {
        dragEligible = true;
        dragStartPos = Input.mousePosition;
        return true;
    }

    private bool HandleLeftMouseClickContinuous() {
        if (!dragEligible || Vector3.Distance(dragStartPos, Input.mousePosition) < 30f) {
            return false;
        }
        
        dragSelect = true;
        GenerateBoxSelectArea(Input.mousePosition);
        return true;
    }

    private bool HandleLeftMouseUp() {
        if (!dragSelect) {
            ClickSelect(Input.GetKey(KeyCode.LeftShift));
        } else {
            dragSelect = false;

            // shift to add onto existing
            if (!Input.GetKey(KeyCode.LeftShift)) {
                DeselectAll();
            }

            HashSet<ClientEntity> entitiesToSelect = new HashSet<ClientEntity>();
            ClientEntity builder = null;
            foreach (ClientEntity entity in hoverSprites.Keys) {
                if (entity == null) continue;
                
                // If a builder is selected, we ignore all others and select the builder
                if (entity is ClientBuilder) {
                    builder = entity;
                    break;
                }
                
                entitiesToSelect.Add(entity);
            }

            ClearHovers();
            currentlyInBox.Clear();

            if (builder != null) {
                DeselectAll();
                SelectEntity(builder);
            }
            else {
                foreach (ClientEntity entity in entitiesToSelect) {
                    SelectEntity(entity);
                }
            }
        }

        dragEligible = false;
        return true;
    }
    #endregion

    #region Box Select
    private void GenerateBoxSelectArea(Vector3 currentMousePos) {
        if (selectionBox == null) {
            selectionBox = gameObject.AddComponent<MeshCollider>();
        }

        // corners of 2D selection box
        Vector2[] corners = getBoundingBox(dragStartPos, currentMousePos);

        // vertices of meshcollider
        Vector3[] vertices = new Vector3[4];

        RaycastHit hit;

        for (int i = 0; i < 4; i++) {
            Ray ray = cam.ScreenPointToRay(corners[i]);

            if (Physics.Raycast(ray, out hit, 1000f, GroundLayerMask)) {
                vertices[i] = new Vector3(hit.point.x, 0, hit.point.z);
                Debug.DrawLine(cam.ScreenToWorldPoint(corners[i]), hit.point, Color.red, .1f);
            }
        }

        generateSelectionMesh(vertices);
        selectionBox.convex = true;
        selectionBox.isTrigger = true;
    }

    private void OnGUI()
    {
        if (!dragEligible || !dragSelect) {
            return;
        }
        
        Rect rect = SelectionBox.GetScreenRect(dragStartPos, Input.mousePosition);
        SelectionBox.DrawScreenRectBorder(rect, 2, new Color(1f, 1f, 1f));
    }

    // https://www.youtube.com/watch?v=OL1QgwaDsqo&t=398s
    private Vector2[] getBoundingBox(Vector2 _p1, Vector2 _p2) {
        Vector2 newP1;
        Vector2 newP2;
        Vector2 newP3;
        Vector2 newP4;

        if (_p1.x < _p2.x) { // if _p1 is to the left of _p2
            if (_p1.y > _p2.y) { // if _p1 is above _p2
                newP1 = _p1;
                newP2 = new Vector2(_p2.x, _p1.y);
                newP3 = new Vector2(_p1.x, _p2.y);
                newP4 = _p2;
            } else { // if _p1 is below _p2
                newP1 = new Vector2(_p1.x, _p2.y);
                newP2 = _p2;
                newP3 = _p1;
                newP4 = new Vector2(_p2.x, _p1.y);
            }
        } else { // if _p1 is to the right of _p2
            if (_p1.y > _p2.y) { // if _p1 is above _p2
                newP1 = new Vector2(_p2.x, _p1.y);
                newP2 = _p1;
                newP3 = _p2;
                newP4 = new Vector2(_p1.x, _p2.y);
            } else { // if _p1 is below _p2
                newP1 = _p2;
                newP2 = new Vector2(_p1.x, _p2.y);
                newP3 = new Vector2(_p2.x, _p1.y);
                newP4 = _p1;
            }
        }

        Vector2[] boxCorners = { newP1, newP2, newP3, newP4 };
        return boxCorners;
    }

    private void generateSelectionMesh(Vector3[] corners) {
        Vector3[] verts = new Vector3[8];
        int[] tris = { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 };

        // bottom rectangle
        for (int i = 0; i < 4; i++) {
            verts[i] = corners[i];
        }

        // top rectangle
        for (int i = 4; i < 8; i++) {
            verts[i] = corners[i - 4] + Vector3.up * 10f;
        }

        for (int i = 0; i < 8; i++) {
            for (int j = 1; j < 8; j++) {
                Debug.DrawLine(verts[i], verts[j], Color.blue, .1f);
            }
        }

        Mesh generatedMesh = new Mesh();
        generatedMesh.vertices = verts;
        generatedMesh.triangles = tris;
        selectionBox.sharedMesh = generatedMesh;
    }

    private void OnTriggerStay(Collider other) {
        ClientEntity entity = other.gameObject.GetComponent<ClientEntity>();
        if (entity != null) {
            currentlyInBox.Add(entity); // see FixedUpdate() for explanation
            CreateHover(entity);
        }
    }

    private void FixedUpdate() {
        // this is necessary because the OnTriggerExit isn't firing as expected when we adjust the mesh
        // FixedUpdate is always called before OnTriggerStay, so we update every entity's collision status with the selection box in OnTriggerStay

        if (dragSelect) {
            Dictionary<ClientEntity, GameObject> hoverSpritesCopy = new Dictionary<ClientEntity, GameObject>(hoverSprites);
            foreach (ClientEntity entity in hoverSpritesCopy.Keys) {
                if (!currentlyInBox.Contains(entity)) {
                    RemoveHover(entity);
                }
            }

            currentlyInBox.Clear();
        }
    }
    #endregion

    #region Raycast Checks

    private const float HoverSensitivity = 2f;
    private void CheckForHover() {
        Vector3 aimPoint;
        try {
            aimPoint = CameraController.Singleton.GetAimPoint();
        } catch (Exception) {
            return;
        }

        Collider[] colliders = Physics.OverlapBox(
            aimPoint, 
            Vector3.one * HoverSensitivity,
            Quaternion.identity, 
            EntityLayerMask
        );
        
        if (colliders.Length > 0) {
            Collider closestCollider = ClientUtil.GetColliderClosestToPoint(
                colliders,
                aimPoint
            );
            ClientEntity closest =
                closestCollider.gameObject.GetComponent<ClientEntity>();

            if (!hoverSprites.ContainsKey(closest)) {
                ClearHovers();
                CreateHover(closest);
            }
        } else if (!dragSelect) {
            ClearHovers();
        }
    }

    private void ClickSelect(bool shiftHeld) {
        if (hoverSprites.Count < 1) {
            return;
        }

        foreach (ClientEntity entity in hoverSprites.Keys) {
            if (entity == null) continue;
            
            if (shiftHeld) {
                if (Targets.Contains(entity)) {
                    DeselectEntity(entity);
                } else {
                    SelectEntity(entity);
                }
            } else {
                DeselectAll();
                SelectEntity(entity);

                lastSelect = (entity, Time.time); // strictly for double click tower selection purposes
            }
        }
    }
    #endregion

    #region Selection and Deselection

    // TODO: Hacky workaround, see TargetSelectedInterface comment for more
    public void SelectEntities(
        List<ClientEntity> entities
    ) {
        foreach (ClientEntity entity in entities) {
            SelectEntity(entity);
        }
    }
    
    private void SelectEntity(ClientEntity entity) {
        if (entity is ClientBuilder) {
            DeselectAll();
        }
        
        // Select our entity
        if (!Targets.Contains(entity)) {
            AddTarget(entity);
        }

        if (
            lastSelect.entity == entity
            && (Time.time - lastSelect.time < 0.5)
            // && entity.playerSlotNumber == ConnectionManager.instance.MySlot tower.lane == mylane
            && entity.GetComponent<ClientEntity>() != null) {
            lastSelect.entity = null;
            SelectAllEntitiesLike(entity);
        }

        // Create the selection sprite
        CreateSelectionSprite(entity);

        // Destroy hover sprite if exists
        if (hoverSprites.ContainsKey(entity)) {
            Destroy(hoverSprites[entity].gameObject);
        }
    }

    private void SelectAllEntitiesLike(ClientEntity entity) {
        if (entity is ClientTower t) {
            try {
                HashSet<ClientTower> towersLike =
                    ClientEntityStorageSystem.Singleton.GetTowersOfTypeByLaneID(
                        t.Type,
                        t.ActiveLane.ID
                    );
            
                foreach (ClientTower likeTower in towersLike) {
                    SelectEntity(likeTower);
                }
            }
            catch (NotFoundException e) {
                LTWLogger.Log(e.Message);
            }
        } else if (entity is ClientEnemy creep) {
            try {
                HashSet<ClientEnemy> creepsLike =
                    ClientEntityStorageSystem.Singleton.GetCreepsOfTypeByLaneID(
                        creep.Type,
                        creep.ActiveLane.ID
                    );
            
                foreach (ClientEnemy likeCreep in creepsLike) {
                    SelectEntity(likeCreep);
                }
            }
            catch (NotFoundException e) {
                LTWLogger.Log(e.Message);
            }
        }
    }

    private void DeselectEntity(ClientEntity entity) {
        // Remove from list
        RemoveTarget(entity);
    }

    private void DeselectAll() {
        if (selectionSprites.Count > 0) {
            foreach (GameObject sprite in selectionSprites.Values) {
                Destroy(sprite.gameObject);
            }

            selectionSprites.Clear();
        }

        if (Targets.Count == 0) return;
        
        // Targets.Clear() would be convenient here, but we should make sure each target goes through the proper removal flow
        foreach (ClientEntity e in new HashSet<ClientEntity>(Targets)) { // make a new set because we're going to be removing elements on each iteration
            RemoveTarget(e, false);
        }

        FireTargetUpdatedEvent();
    }

    private void FireTargetUpdatedEvent() {
        // TODO: Get rid of this, move the single/multiple distinction to TargetSelectedInterface
        if (Targets.Count == 1) {
            EventBus.SingleTargetSelected(Targets[0]);
        }
        
        EventBus.TargetsUpdated(Targets);
    }

    private void AddTarget(ClientEntity target) {
        Targets.Add(target);
        target.OnDestroyed += TargetDestroyed;
        
        FireTargetUpdatedEvent();
    }

    private void RemoveTarget(ClientEntity target, bool invokeUpdateEvent = true) {
        if (Targets.Remove(target)) {
            target.OnDestroyed -= TargetDestroyed;

            if (invokeUpdateEvent) {
                FireTargetUpdatedEvent();
            }
        }

        RemoveSelectionSprite(target);
    }

    private void TargetDestroyed(ClientEntity target) {
        RemoveTarget(target);
    }
    #endregion

    #region Hovering Helpers

    private void CreateSelectionSprite(ClientEntity entity) {
        if (selectionSprites.ContainsKey(entity)) {
            return;
        }

        SelectionSpriteIndicator indicator = Instantiate(
            ClientPrefabs.Singleton.pfSelectedSpritePrefab,
            entity.transform, 
            false
        );
        
        selectionSprites.Add(entity, indicator.gameObject);
    }

    private void RemoveSelectionSprite(ClientEntity entity) {
        if (!selectionSprites.ContainsKey(entity)) {
            return;
        }

        GameObject selectionSprite = selectionSprites[entity];
        selectionSprites.Remove(entity);
        
        if (selectionSprite == null) {
            return;
        }
        Destroy(selectionSprite);
    }
    
    private void CreateHover(ClientEntity entity) {
        if (hoverSprites.ContainsKey(entity)) {
            return;
        }

        SelectionSpriteIndicator indicator = Instantiate(
            ClientPrefabs.Singleton.pfHoveringSpritePrefab,
            entity.transform, 
            false
        );
        hoverSprites.Add(entity, indicator.gameObject);
    }

    private void RemoveHover(ClientEntity entity) {
        if (!hoverSprites.ContainsKey(entity)) {
            return;
        }

        Destroy(hoverSprites[entity].gameObject);
        hoverSprites.Remove(entity);
    }

    private void ClearHovers() {
        if (hoverSprites.Count < 1) {
            return;
        }

        Dictionary<ClientEntity, GameObject> hoverSpritesCopy =
            new Dictionary<ClientEntity, GameObject>(hoverSprites);
        foreach (ClientEntity entity in hoverSpritesCopy.Keys) {
            RemoveHover(entity);
        }
    }
    #endregion

    #region Event Triggering Helper
    public void RefreshEntity(ClientEntity e) {
        if (Targets.Contains(e)) {
            FireTargetUpdatedEvent();
        }
    }
    #endregion

    public HashSet<ClientEntity> GetTargetsOfMatchingType(ClientEntity match) {
        HashSet<ClientEntity> refinedTargets = new HashSet<ClientEntity>();
        foreach (ClientEntity target in Targets) {
            if (target.GetType() == match.GetType()) {
                refinedTargets.Add(target);
            }
        }
        return refinedTargets;
    }

}
