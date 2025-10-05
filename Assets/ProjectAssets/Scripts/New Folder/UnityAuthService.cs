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

    [SerializeField] private CloudSaveDataService dataService;
    [SerializeField] private PlayerDataSO playerData;

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
            Debug.Log("Unity Services inicializados correctamente");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Initialization failed: {ex.Message}");
            OnLoginFailed?.Invoke($"Initialization failed: {ex.Message}");
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

            await LoadOrInitializePlayerData();

            var playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
            OnLoginSuccess?.Invoke(playerName);
        }
        catch (Exception ex)
        {
            OnLoginFailed?.Invoke($"Authentication failed: {ex.Message}");
        }
    }

    private async Task LoadOrInitializePlayerData()
    {
        if (dataService == null)
        {
            Debug.LogError("DataService no asignado");
            return;
        }

        var savedData = await dataService.LoadPlayerDataAsync();

        if (savedData != null)
        {
            CopyPlayerData(savedData, playerData);
            Debug.Log("Datos existentes cargados correctamente");

            var unityPlayerName = await AuthenticationService.Instance.GetPlayerNameAsync();
            playerData.playerName = unityPlayerName;
        }
        else
        {
            var unityPlayerName = await AuthenticationService.Instance.GetPlayerNameAsync();
            playerData.Initialize(AuthenticationService.Instance.PlayerId, unityPlayerName);
            await dataService.SavePlayerDataAsync(playerData);
            Debug.Log("Nuevo jugador inicializado");
        }
    }

    private void CopyPlayerData(PlayerDataSO source, PlayerDataSO target)
    {
        target.playerId = source.playerId;
        target.level = source.level;
        target.experience = source.experience;
        target.availableSkillPoints = source.availableSkillPoints;
        target.strength = source.strength;
        target.defense = source.defense;
        target.agility = source.agility;
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

            var updatedName = await AuthenticationService.Instance.GetPlayerNameAsync();

            if (playerData != null)
            {
                playerData.playerName = updatedName;

                if (dataService != null)
                    await dataService.SavePlayerDataAsync(playerData);
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error updating name: {ex.Message}");
            return false;
        }
    }

    public string GetPlayerId()
    {
        return AuthenticationService.Instance.PlayerId;
    }
}