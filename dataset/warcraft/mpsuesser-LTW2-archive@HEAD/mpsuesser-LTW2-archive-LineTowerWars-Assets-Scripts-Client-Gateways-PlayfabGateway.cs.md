---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Gateways/PlayfabGateway.cs"
ref: HEAD
---

﻿using PlayFab;
using PlayFab.ClientModels;
using System;

public static class PlayfabGateway {
    public static void CreateAccount(string username, string email, string password, Action<Login> successCallback, Action<string> failureCallback) {
        RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest {
            Username = username,
            DisplayName = username,
            Email = email,
            Password = password,
            RequireBothUsernameAndEmail = true,
            TitleId = PlayFabSettings.TitleId
        };

        PlayFabClientAPI.RegisterPlayFabUser(
            request,
            (RegisterPlayFabUserResult result) => {
                Login login = new Login(result.PlayFabId, result.Username, result.SessionTicket);
                
                successCallback?.Invoke(login);
            },
            (PlayFabError error) => {
                failureCallback?.Invoke(error.ErrorMessage);
            }
        );
    }

    public static void Login(string username, string password, Action<Login> successCallback,
        Action<string> failureCallback) {
        LoginWithPlayFabRequest request = new LoginWithPlayFabRequest {
            Username = username,
            Password = password,
            TitleId = PlayFabSettings.TitleId,
        };
        
        PlayFabClientAPI.LoginWithPlayFab(
            request,
            (LoginResult result) => {
                Login login = new Login(result.PlayFabId, username, result.SessionTicket);
                
                successCallback?.Invoke(login);
            },
            (PlayFabError error) => {
                failureCallback?.Invoke(error.ErrorMessage);
            }
        );
    }
}