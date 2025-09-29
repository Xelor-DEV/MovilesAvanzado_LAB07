using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using System;
using Unity.Services.Authentication.PlayerAccounts;
using System.Collections.Generic;
using Unity.Services.CloudSave;

public class UnityPlayerAuth : MonoBehaviour
{
    public event Action<PlayerInfo, string> OnSignedIn;
    public event Action<String> OnUpdateName;
    private PlayerInfo playerInfo;
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        SetupEvents();
        PlayerAccountService.Instance.SignedIn += SignIn;
    }

    private void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Player ID: " + AuthenticationService.Instance.PlayerId);
            Debug.Log("Acess Token: " + AuthenticationService.Instance.AccessToken);
        };

        AuthenticationService.Instance.SignInFailed += (err) =>
        {
            Debug.Log(err);
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log("Player Log Out");
        };

        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player Session Expired");
        };
    }

    public async Task InitSignIn()
    {
        await PlayerAccountService.Instance.StartSignInAsync();
    }

    private async void SignIn()
    {
        try
        {
            await SignInWithUnityAuth();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    private async Task SignInWithUnityAuth()
    {
        try
        {
            // Obtener el token de Player Accounts en lugar de Authentication
            string accessToken = PlayerAccountService.Instance.AccessToken;

            // Usar el token correcto para autenticación
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);

            playerInfo = AuthenticationService.Instance.PlayerInfo;
            var name = await AuthenticationService.Instance.GetPlayerNameAsync();

            OnSignedIn?.Invoke(playerInfo, name);

            Debug.Log("Sign In Successful");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.Log(ex);
        }
    }

    public async Task UpdateName(string newName)
    {
        await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
        var name = await AuthenticationService.Instance.GetPlayerNameAsync();

        OnUpdateName?.Invoke(name);
    }

    public async Task DeleteAccountUnityAsync()
    {
        try
        {
            await AuthenticationService.Instance.DeleteAccountAsync();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            throw;
        }
    }

    // Cloud Save

    public async void SaveData(string key, string value)
    {
        var playerData = new Dictionary<string, object>()
        {
            {key, value }
        };

        await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
    }

    private async void LoadData(string key)
    {
        var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(
            new HashSet<string> { key }
            );

        if (playerData.TryGetValue(key, out var value))
        {
            Debug.Log(key + "value: " + value.Value.GetAs<String>());
        }
    }

    public async void DeleteData(string key)
    {
        await CloudSaveService.Instance.Data.Player.DeleteAsync(key);
    }
}
