// PlayerData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Player Data")]
public class PlayerData : ScriptableObject
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
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        while (HasEnoughExperience)
        {
            experience -= RequiredExpForNextLevel;
            level++;
            availableSkillPoints += pointsPerLevel;
        }
    }

    public bool CanAssignSkillPoint(StatType stat)
    {
        return availableSkillPoints > 0;
    }

    public void AssignSkillPoint(StatType stat)
    {
        if (!CanAssignSkillPoint(stat)) return;

        availableSkillPoints--;

        switch (stat)
        {
            case StatType.Strength:
                strength++;
                break;
            case StatType.Defense:
                defense++;
                break;
            case StatType.Agility:
                agility++;
                break;
        }
    }
}

public enum StatType
{
    Strength,
    Defense,
    Agility
}