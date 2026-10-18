---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Systems/MB/LoginSystem/LoginSystem.cs"
ref: HEAD
---

﻿using UnityEngine;

public class LoginSystem : SingletonBehaviour<LoginSystem> {
    public bool IsLoggedIn => _activeLogin != null;
    public string ActiveUsername => IsLoggedIn ? _activeLogin.Username : "ERR";
    public string ActivePlayfabID => IsLoggedIn ? _activeLogin.PlayfabID : "ERR";
    public string ActiveAuthToken => IsLoggedIn ? _activeLogin.AuthToken : "ERR";
    private Login _activeLogin = null;

    private void Awake() {
        InitializeSingleton(this);
    }
    
    #region Login

    public void AttemptAccountLogin(string username, string password) {
        LTWLogger.Log($"Attempting account login with u:{username}, p:{password}");
        PlayfabGateway.Login(
            username,
            password,
            LoginSuccessful,
            LoginFailed
        );
    }

    public void AttemptGenericLogin() {
        // TODO: Implement
    }

    private void LoginSuccessful(Login login) {
        SetLogin(login);
    }

    private void LoginFailed(string failureReason) {
        LTWLogger.Log($"Login attempt failed: {failureReason}");
        EventBus.LoginFailed(failureReason);

        ClearLogin();
    }
    
    #endregion
    
    #region Logout

    public void AttemptLogout() {
        ClearLogin();
    }

    private void LogoutSuccessful() {
        ClearLogin();
    }
    
    #endregion
    
    #region Account Creation

    public void AttemptAccountCreation(string username, string email, string password) {
        PlayfabGateway.CreateAccount(
            username,
            email,
            password,
            AccountCreationSuccessful,
            AccountCreationFailed
        );
    }

    private void AccountCreationSuccessful(Login login) {
        EventBus.AccountCreationSuccessful();
        
        SetLogin(login);
    }

    private void AccountCreationFailed(string failureReason) {
        LTWLogger.Log($"Account creation attempt failed: {failureReason}");
        EventBus.AccountCreationFailed(failureReason);
        
        ClearLogin();
    }
    
    #endregion

    private void SetLogin(Login login) {
        _activeLogin = login;
        EventBus.LoginSuccessful();
    }

    private void ClearLogin() {
        if (!IsLoggedIn) {
            return;
        }
        
        _activeLogin = null;
        EventBus.LoggedOut();
    }
}