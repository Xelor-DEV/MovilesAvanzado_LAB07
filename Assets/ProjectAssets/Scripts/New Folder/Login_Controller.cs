using UnityEngine;
using System;
public class Login_Controller : MonoBehaviour
{
    [SerializeField] private UnityAuthService authService;
    [SerializeField] private CloudSaveDataService dataService;
    [SerializeField] private PlayerDataSO playerData;

    public event Action<string> OnLoginSuccess;
    public event Action<string> OnLoginFailed;

    private void OnEnable()
    {
        if (authService != null)
        {
            authService.OnLoginSuccess += HandleLoginSuccess;
            authService.OnLoginFailed += HandleLoginFailed;
        }
    }

    private void OnDisable()
    {
        if (authService != null)
        {
            authService.OnLoginSuccess -= HandleLoginSuccess;
            authService.OnLoginFailed -= HandleLoginFailed;
        }
    }

    public async void StartLogin()
    {
        await authService.LoginAsync();
    }

    private async void HandleLoginSuccess(string playerName)
    {
        try
        {
            string playerId = authService.GetPlayerId();
            var savedData = await dataService.LoadPlayerDataAsync();

            if (savedData != null)
            {
                CopyPlayerDataPreservingName(savedData, playerData, playerName);
                Debug.Log("Datos existentes cargados correctamente");
            }
            else
            {
                playerData.Initialize(playerId, playerName);
                await dataService.SavePlayerDataAsync(playerData);
                Debug.Log("Nuevo jugador inicializado");
            }

            OnLoginSuccess?.Invoke(playerData.playerName);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error en login: {ex.Message}");
            OnLoginFailed?.Invoke($"Error cargando datos: {ex.Message}");
        }
    }

    private void CopyPlayerDataPreservingName(PlayerDataSO source, PlayerDataSO target, string unityPlayerName)
    {
        target.playerId = source.playerId;
        target.playerName = unityPlayerName;
        target.level = source.level;
        target.experience = source.experience;
        target.availableSkillPoints = source.availableSkillPoints;
        target.strength = source.strength;
        target.defense = source.defense;
        target.agility = source.agility;
    }

    private void HandleLoginFailed(string error)
    {
        OnLoginFailed?.Invoke(error);
    }
}