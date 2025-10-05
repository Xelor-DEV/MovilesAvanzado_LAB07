using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField] private PlayerDataSO playerData;
    [SerializeField] private CloudSaveDataService dataService;
    [SerializeField] private UnityAuthService authService;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text experienceText;
    [SerializeField] private TMP_Text skillPointsText;
    [SerializeField] private TMP_Text strengthText;
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private TMP_Text agilityText;

    [Header("Name Update")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button updateNameButton;

    [Header("Stat Sliders")]
    [SerializeField] private Slider strengthSlider;
    [SerializeField] private TMP_Text strengthSliderValueText;
    [SerializeField] private Button confirmStrengthButton;

    [SerializeField] private Slider defenseSlider;
    [SerializeField] private TMP_Text defenseSliderValueText;
    [SerializeField] private Button confirmDefenseButton;

    [SerializeField] private Slider agilitySlider;
    [SerializeField] private TMP_Text agilitySliderValueText;
    [SerializeField] private Button confirmAgilityButton;

    [Header("Experience")]
    [SerializeField] private Button gainExperienceButton;
    [SerializeField] private SpriteAnimation trainAnimation;
    [SerializeField] private int expPerClick = 10;

    private void OnEnable()
    {
        updateNameButton.onClick.AddListener(UpdatePlayerName);
        confirmStrengthButton.onClick.AddListener(() => AssignStatPoints(StatType.Strength, (int)strengthSlider.value));
        confirmDefenseButton.onClick.AddListener(() => AssignStatPoints(StatType.Defense, (int)defenseSlider.value));
        confirmAgilityButton.onClick.AddListener(() => AssignStatPoints(StatType.Agility, (int)agilitySlider.value));
        gainExperienceButton.onClick.AddListener(GainExperience);

        strengthSlider.onValueChanged.AddListener(OnStrengthSliderChanged);
        defenseSlider.onValueChanged.AddListener(OnDefenseSliderChanged);
        agilitySlider.onValueChanged.AddListener(OnAgilitySliderChanged);

        if (playerData != null)
            playerData.OnDataChanged += OnPlayerDataChanged;
    }

    private void OnDisable()
    {
        updateNameButton.onClick.RemoveListener(UpdatePlayerName);
        confirmStrengthButton.onClick.RemoveAllListeners();
        confirmDefenseButton.onClick.RemoveAllListeners();
        confirmAgilityButton.onClick.RemoveAllListeners();
        gainExperienceButton.onClick.RemoveListener(GainExperience);

        strengthSlider.onValueChanged.RemoveListener(OnStrengthSliderChanged);
        defenseSlider.onValueChanged.RemoveListener(OnDefenseSliderChanged);
        agilitySlider.onValueChanged.RemoveListener(OnAgilitySliderChanged);

        if (playerData != null)
            playerData.OnDataChanged -= OnPlayerDataChanged;
    }

    private void Start()
    {
        InitializeUI();
        UpdateAllUI();
        LoadCurrentPlayerName();
    }

    private async void LoadCurrentPlayerName()
    {
        if (authService != null && nameInputField != null)
        {
            var currentName = await authService.GetPlayerNameAsync();
            nameInputField.text = currentName;
        }
    }

    private void InitializeUI()
    {
        strengthSlider.wholeNumbers = true;
        strengthSlider.minValue = 0;
        strengthSlider.maxValue = 10;

        defenseSlider.wholeNumbers = true;
        defenseSlider.minValue = 0;
        defenseSlider.maxValue = 10;

        agilitySlider.wholeNumbers = true;
        agilitySlider.minValue = 0;
        agilitySlider.maxValue = 10;

        strengthSlider.value = 0;
        defenseSlider.value = 0;
        agilitySlider.value = 0;
    }

    private void UpdateAllUI()
    {
        UpdatePlayerInfo();
        UpdateStatDisplays();
        UpdateSliderTexts();
    }

    private void UpdatePlayerInfo()
    {
        if (playerNameText != null)
            playerNameText.text = playerData.playerName;

        if (levelText != null)
            levelText.text = $"Nivel: {playerData.level}";

        if (experienceText != null)
            experienceText.text = $"{playerData.experience} / {playerData.RequiredExpForNextLevel}";

        if (skillPointsText != null)
            skillPointsText.text = $"Puntos: {playerData.availableSkillPoints}";
    }

    private void UpdateStatDisplays()
    {
        if (strengthText != null)
            strengthText.text = $"Fuerza: {playerData.strength}";

        if (defenseText != null)
            defenseText.text = $"Defensa: {playerData.defense}";

        if (agilityText != null)
            agilityText.text = $"Agilidad: {playerData.agility}";
    }

    private void UpdateSliderTexts()
    {
        if (strengthSliderValueText != null)
            strengthSliderValueText.text = strengthSlider.value.ToString();

        if (defenseSliderValueText != null)
            defenseSliderValueText.text = defenseSlider.value.ToString();

        if (agilitySliderValueText != null)
            agilitySliderValueText.text = agilitySlider.value.ToString();
    }

    private async void UpdatePlayerName()
    {
        if (!string.IsNullOrEmpty(nameInputField.text))
        {
            bool success = await authService.UpdatePlayerNameAsync(nameInputField.text);

            if (success)
            {
                UpdateAllUI();
                Debug.Log($"Nombre actualizado a: {playerData.playerName}");
            }
            else
            {
                Debug.LogError("Error al actualizar el nombre");
                LoadCurrentPlayerName();
            }
        }
    }

    private async void AssignStatPoints(StatType stat, int pointsToAssign)
    {
        if (pointsToAssign <= 0)
        {
            Debug.LogWarning("Debes asignar al menos 1 punto");
            return;
        }

        if (playerData.CanAssignSkillPoint(stat, pointsToAssign))
        {
            playerData.AssignSkillPoint(stat, pointsToAssign);

            if (dataService != null)
                await dataService.SavePlayerDataAsync(playerData);

            switch (stat)
            {
                case StatType.Strength:
                    strengthSlider.value = 0;
                    break;
                case StatType.Defense:
                    defenseSlider.value = 0;
                    break;
                case StatType.Agility:
                    agilitySlider.value = 0;
                    break;
            }
        }
        else
        {
            Debug.LogWarning("No tienes suficientes puntos de habilidad");
        }
    }

    private async void GainExperience()
    {
        if (trainAnimation != null)
        {
            trainAnimation.PlayAnimation();
        }

        playerData.AddExperience(expPerClick);

        if (dataService != null)
            await dataService.SavePlayerDataAsync(playerData);

        Debug.Log($"¡Ganaste {expPerClick} de experiencia!");
    }

    private void OnStrengthSliderChanged(float value)
    {
        if (strengthSliderValueText != null)
            strengthSliderValueText.text = value.ToString();
    }

    private void OnDefenseSliderChanged(float value)
    {
        if (defenseSliderValueText != null)
            defenseSliderValueText.text = value.ToString();
    }

    private void OnAgilitySliderChanged(float value)
    {
        if (agilitySliderValueText != null)
            agilitySliderValueText.text = value.ToString();
    }

    private void OnPlayerDataChanged()
    {
        UpdateAllUI();
    }
}