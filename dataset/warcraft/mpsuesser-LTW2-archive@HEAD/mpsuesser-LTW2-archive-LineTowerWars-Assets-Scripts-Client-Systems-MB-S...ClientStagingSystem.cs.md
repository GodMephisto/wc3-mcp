---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/StagingSystem/ClientStagingSystem.cs"
ref: HEAD
---

public class ClientStagingSystem : SingletonBehaviour<ClientStagingSystem>
{
    private void Awake() {
        InitializeSingleton(this);
        
        EventBus.OnStagingStarted += InitiateStaging;
    }

    private void OnDestroy() {
        EventBus.OnStagingStarted -= InitiateStaging;
    }

    private void InitiateStaging() {
        LTWLogger.Log("Staging initiated...");

        LaneSystem.Singleton.InitializeLanes(LobbySystem.Singleton.PlayersByClientID.Count);
        SceneSystem.Singleton.LoadGame();
        
        // TODO: Make this dynamic e.g. have specific systems let this system know they're loaded
        // Wait for everything to load, then trigger MyLane sync
        Invoke(nameof(SyncLaneAndFinish), 0.5f);
    }

    private void SyncLaneAndFinish() {
        try {
            PlayerInfo myPlayer = LobbySystem.Singleton.GetPlayerByClientID(
                ClientNetworkManager.Singleton.Client.Id
            );

            // TODO: Will need to update this if slot->lane mapping ever becomes non-1-to-1, such as in a team LTW mode
            EventBus.MyLaneUpdated(myPlayer.Slot);
        }
        catch (NotFoundException e) {
            LTWLogger.LogError($"Could not find own PlayerInfo! Message: {e.Message}");
            FailStaging();
            return;
        }
        
        FinishStaging();
    }

    private static void FailStaging() {
        LTWLogger.LogError("TODO: Send the server a failed staging message!");
    }
    
    private static void FinishStaging() {
        EventBus.StagingCompleted();
    }
}
