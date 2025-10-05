using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using UnityEngine;

public class CloudSaveDataService : MonoBehaviour, IDataService
{
    public async Task<PlayerDataSO> LoadPlayerDataAsync()
    {
        try
        {
            var data = await CloudSaveService.Instance.Data.Player.LoadAllAsync();

            if (data.Count == 0)
            {
                Debug.Log("No se encontraron datos guardados, inicializando nuevo jugador");
                return null;
            }

            PlayerDataSO playerData = ScriptableObject.CreateInstance<PlayerDataSO>();

            if (data.TryGetValue("playerId", out var playerId))
                playerData.playerId = playerId.Value.GetAs<string>();

            if (data.TryGetValue("playerName", out var playerName))
                playerData.playerName = playerName.Value.GetAs<string>();

            if (data.TryGetValue("level", out var level))
                playerData.level = level.Value.GetAs<int>();

            if (data.TryGetValue("experience", out var exp))
                playerData.experience = exp.Value.GetAs<int>();

            if (data.TryGetValue("availableSkillPoints", out var points))
                playerData.availableSkillPoints = points.Value.GetAs<int>();

            if (data.TryGetValue("strength", out var strength))
                playerData.strength = strength.Value.GetAs<int>();

            if (data.TryGetValue("defense", out var defense))
                playerData.defense = defense.Value.GetAs<int>();

            if (data.TryGetValue("agility", out var agility))
                playerData.agility = agility.Value.GetAs<int>();

            Debug.Log("Datos cargados exitosamente de Cloud Save");
            return playerData;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error cargando datos: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> SavePlayerDataAsync(PlayerDataSO data)
    {
        try
        {
            var playerData = new Dictionary<string, object>
            {
                { "playerId", data.playerId },
                { "playerName", data.playerName },
                { "level", data.level },
                { "experience", data.experience },
                { "availableSkillPoints", data.availableSkillPoints },
                { "strength", data.strength },
                { "defense", data.defense },
                { "agility", data.agility }
            };

            await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
            Debug.Log("Datos guardados exitosamente en Cloud Save");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error guardando datos: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> InitializeNewPlayerAsync()
    {
        try
        {
            var defaultData = new Dictionary<string, object>
            {
                { "playerId", AuthenticationService.Instance.PlayerId },
                { "playerName", "Player" },
                { "level", 1 },
                { "experience", 0 },
                { "availableSkillPoints", 0 },
                { "strength", 10 },
                { "defense", 10 },
                { "agility", 10 }
            };

            await CloudSaveService.Instance.Data.Player.SaveAsync(defaultData);
            Debug.Log("Jugador nuevo inicializado en Cloud Save");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error inicializando nuevo jugador: {ex.Message}");
            return false;
        }
    }
}