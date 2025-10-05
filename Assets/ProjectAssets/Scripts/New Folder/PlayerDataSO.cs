using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Game/Player Data")]
public class PlayerDataSO : ScriptableObject
{
    [Header("Basic Info")]
    public string playerId;
    public string playerName;
    public int level = 1;
    public int experience;
    public int availableSkillPoints;

    [Header("Stats")]
    public int strength = 10;
    public int defense = 10;
    public int agility = 10;

    [Header("Level Settings")]
    public int baseExpRequired = 100;
    public float expMultiplier = 1.5f;
    public int pointsPerLevel = 3;

    public event Action OnDataChanged;

    public int RequiredExpForNextLevel
    {
        get
        {
            return Mathf.RoundToInt(baseExpRequired * Mathf.Pow(expMultiplier, level - 1));
        }
    }

    public bool HasEnoughExperience
    {
        get
        {
            return experience >= RequiredExpForNextLevel;
        }
    }

    public void Initialize(string id, string name)
    {
        playerId = id;
        playerName = name;
        level = 1;
        experience = 0;
        availableSkillPoints = 0;
        strength = 10;
        defense = 10;
        agility = 10;
        OnDataChanged?.Invoke();
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        CheckLevelUp();
        OnDataChanged?.Invoke();
    }

    private void CheckLevelUp()
    {
        while (HasEnoughExperience)
        {
            experience -= RequiredExpForNextLevel;
            level++;
            availableSkillPoints += pointsPerLevel;
            Debug.Log($"¡Nivel subido! Ahora eres nivel {level}. Obtienes {pointsPerLevel} puntos de habilidad.");
        }
    }

    public bool CanAssignSkillPoint(StatType stat, int pointsToAssign = 1)
    {
        return availableSkillPoints >= pointsToAssign;
    }

    public void AssignSkillPoint(StatType stat, int pointsToAssign = 1)
    {
        if (!CanAssignSkillPoint(stat, pointsToAssign))
        {
            Debug.LogWarning("No hay suficientes puntos de habilidad disponibles.");
            return;
        }

        availableSkillPoints -= pointsToAssign;

        switch (stat)
        {
            case StatType.Strength:
                strength += pointsToAssign;
                Debug.Log($"Fuerza aumentada a {strength}");
                break;
            case StatType.Defense:
                defense += pointsToAssign;
                Debug.Log($"Defensa aumentada a {defense}");
                break;
            case StatType.Agility:
                agility += pointsToAssign;
                Debug.Log($"Agilidad aumentada a {agility}");
                break;
        }
        OnDataChanged?.Invoke();
    }

    public void UpdatePlayerName(string newName)
    {
        playerName = newName;
        OnDataChanged?.Invoke();
    }
}

public enum StatType
{
    Strength,
    Defense,
    Agility
}