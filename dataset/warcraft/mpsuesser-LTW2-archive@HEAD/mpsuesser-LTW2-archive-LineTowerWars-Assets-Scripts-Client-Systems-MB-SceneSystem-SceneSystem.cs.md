---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/SceneSystem/SceneSystem.cs"
ref: HEAD
---

using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

public class SceneSystem : SingletonBehaviour<SceneSystem>
{
    private void Awake() {
        InitializeSingleton(this);
    }

    enum Scene {
        ClientPreload = 0,
        shared,
        Map,
        ClientMainMenu,
        ClientLobby,
        ClientInGame,
    }

    private HashSet<Scene> MutuallyExclusiveScenes = new HashSet<Scene>() {
        Scene.ClientMainMenu,
        Scene.ClientLobby,
        Scene.ClientInGame,
    };

    public void LoadSharedScene() {
        LoadSceneAdditively(Scene.shared);
        LoadSceneAdditively(Scene.Map);
    }

    public void LoadLobby() {
        LoadSceneAdditively(Scene.ClientLobby);
    }

    public void LoadGame() {
        // TODO: Bit hacky, could use some better placement
        MapEnvironment.Singleton.Enable();
        
        LoadSceneAdditively(Scene.ClientInGame);
    }

    public void LoadMainMenu() {
        LoadSceneAdditively(Scene.ClientMainMenu);
        LoadSharedScene();
    }

    private void LoadSceneAdditively(Scene scene) {
        if (MutuallyExclusiveScenes.Contains(scene)) {
            foreach (Scene mutuallyExclusiveScene in MutuallyExclusiveScenes) {
                if (mutuallyExclusiveScene == scene) {
                    continue;
                }

                UnloadScene(mutuallyExclusiveScene);
            }
        }

        SceneManager.LoadScene((int)scene, LoadSceneMode.Additive);
    }

    public void CleanupFromLobby() {
        UnloadScene(Scene.Map);
        UnloadScene(Scene.shared);
        UnloadScene(Scene.ClientLobby);
        UnloadScene(Scene.ClientInGame);
    }

    private static void UnloadScene(Scene scene) {
        if (!SceneManager.GetSceneByBuildIndex((int)scene).isLoaded) {
            return;
        }

        if (scene == Scene.ClientInGame) {
            EventBus.GameSceneUnloaded();
        }

        SceneManager.UnloadSceneAsync((int)scene);
    }
}
