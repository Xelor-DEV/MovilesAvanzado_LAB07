using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using Unity.Services.Authentication.PlayerAccounts;
using System;

public class UnityAuthService : MonoBehaviour, IAuthService
{
    public event Action<string> OnLoginSuccess;
    public event Action<string> OnLoginFailed;

    private bool isInitialized = false;

    private async void Start()
    {
        await InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();
            SetupAuthenticationEvents();
            PlayerAccountService.Instance.SignedIn += HandlePlayerAccountSignedIn;
            isInitialized = true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Initialization failed: {ex.Message}");
        }
    }

    private void SetupAuthenticationEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Signed in: {AuthenticationService.Instance.PlayerId}");
        };

        AuthenticationService.Instance.SignInFailed += (error) =>
        {
            OnLoginFailed?.Invoke($"Sign in failed: {error.Message}");
        };
    }

    private async void HandlePlayerAccountSignedIn()
    {
        try
        {
            string accessToken = PlayerAccountService.Instance.AccessToken;
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);

            var playerName = await GetPlayerNameAsync();
            OnLoginSuccess?.Invoke(playerName);
        }
        catch (Exception ex)
        {
            OnLoginFailed?.Invoke($"Authentication failed: {ex.Message}");
        }
    }

    public async Task<bool> LoginAsync()
    {
        if (!isInitialized)
        {
            OnLoginFailed?.Invoke("Services not initialized");
            return false;
        }

        try
        {
            await PlayerAccountService.Instance.StartSignInAsync();
            return true;
        }
        catch (Exception ex)
        {
            OnLoginFailed?.Invoke($"Login failed: {ex.Message}");
            return false;
        }
    }

    public async Task<string> GetPlayerNameAsync()
    {
        try
        {
            return await AuthenticationService.Instance.GetPlayerNameAsync();
        }
        catch
        {
            return "Player";
        }
    }

    public async Task<bool> UpdatePlayerNameAsync(string newName)
    {
        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
            return true;
        }
        catch
        {
            return false;
        }
    }
}