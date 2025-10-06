using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using UnityEngine;

public class CloudSaveDataService : MonoBehaviour, IDataService
{
    private string CurrentGameVersion => Application.version;

    public async Task<PlayerDataSO> LoadPlayerDataAsync()
    {
        try
        {
            var keys = new HashSet<string> { "player_profile" };
            var data = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

            if (data == null || data.Count == 0)
            {
                Debug.Log("No se encontraron datos guardados, inicializando nuevo jugador");
                return null;
            }

            if (data.TryGetValue("player_profile", out var profile))
            {
                string json = profile.Value.GetAs<string>();
                PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
                
                // Verificar versión del juego si es necesario
                if (playerData.game_version != CurrentGameVersion)
                {
                    Debug.Log($"Datos de versión diferente: {playerData.game_version} -> {CurrentGameVersion}");
                    // Aquí podrías añadir migración de datos si es necesario
                }

                return ConvertToPlayerDataSO(playerData);
            }

            return null;
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
            PlayerData playerData = ConvertFromPlayerDataSO(data);
            playerData.game_version = CurrentGameVersion;

            string playerDataJson = JsonUtility.ToJson(playerData);
            var saveData = new Dictionary<string, object>
            {
                { "player_profile", playerDataJson }
            };

            await CloudSaveService.Instance.Data.Player.SaveAsync(saveData);
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
			PlayerDataSO defaultDataSO = ScriptableObject.CreateInstance<PlayerDataSO>();
            defaultDataSO.Initialize(AuthenticationService.Instance.PlayerId, "Player");
			
            PlayerData defaultData = ConvertFromPlayerDataSO(defaultDataSO);

            string playerDataJson = JsonUtility.ToJson(defaultData);
            var saveData = new Dictionary<string, object>
            {
                { "player_profile", playerDataJson }
            };

            await CloudSaveService.Instance.Data.Player.SaveAsync(saveData);
            Debug.Log("Jugador nuevo inicializado en Cloud Save");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error inicializando nuevo jugador: {ex.Message}");
            return false;
        }
    }

    private PlayerData ConvertFromPlayerDataSO(PlayerDataSO so)
    {
        return new PlayerData
        {
            player_id = so.playerId,
            player_name = so.playerName,
            level = so.level,
            experience = so.experience,
            available_skill_points = so.availableSkillPoints,
            strength = so.strength,
            defense = so.defense,
            agility = so.agility,
            base_exp_required = so.baseExpRequired,
            exp_multiplier = so.expMultiplier,
            points_per_level = so.pointsPerLevel
        };
    }

    private PlayerDataSO ConvertToPlayerDataSO(PlayerData data)
    {
        PlayerDataSO playerData = ScriptableObject.CreateInstance<PlayerDataSO>();
        
        playerData.playerId = data.player_id;
        playerData.playerName = data.player_name;
        playerData.level = data.level;
        playerData.experience = data.experience;
        playerData.availableSkillPoints = data.available_skill_points;
        playerData.strength = data.strength;
        playerData.defense = data.defense;
        playerData.agility = data.agility;
        playerData.baseExpRequired = data.base_exp_required;
        playerData.expMultiplier = data.exp_multiplier;
        playerData.pointsPerLevel = data.points_per_level;

        return playerData;
    }
}

[Serializable]
public class PlayerData
{
    public string player_id;
    public string player_name;
    public int level;
    public int experience;
    public int available_skill_points;
    public int strength;
    public int defense;
    public int agility;
    public int base_exp_required;
    public float exp_multiplier;
    public int points_per_level;
    public string game_version;
}