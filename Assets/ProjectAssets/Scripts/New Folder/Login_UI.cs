using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Login_UI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button loginButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private GameObject loadingIndicator;

    [Header("Dependencies")]
    [SerializeField] private Login_Controller loginController;

    [Header("Config")]
    [SerializeField] private string menuSceneName;

    private void Start()
    {
        SetStatus("Presione Login para comenzar");
        SetLoading(false);
    }

    private void OnEnable()
    {
        loginButton.onClick.AddListener(HandleLogin);
        exitButton.onClick.AddListener(HandleExit);

        if (loginController != null)
        {
            loginController.OnLoginSuccess += HandleLoginSuccess;
            loginController.OnLoginFailed += HandleLoginFailed;
        }
    }

    private void OnDisable()
    {
        loginButton.onClick.RemoveListener(HandleLogin);
        exitButton.onClick.RemoveListener(HandleExit);

        if (loginController != null)
        {
            loginController.OnLoginSuccess -= HandleLoginSuccess;
            loginController.OnLoginFailed -= HandleLoginFailed;
        }
    }

    private void HandleLogin()
    {
        SetStatus("Iniciando sesión...");
        SetLoading(true);
        loginButton.interactable = false;

        try
        {
            loginController.StartLogin();
        }
        catch (Exception e)
        {
            HandleLoginFailed($"Error: {e.Message}");
        }
    }

    private void HandleLoginSuccess(string playerName)
    {
        SetStatus($"¡Bienvenido {playerName}!");
        SetLoading(false);

        LoadMenuScene();
    }

    private void HandleLoginFailed(string error)
    {
        SetStatus($"Error: {error}");
        SetLoading(false);
        loginButton.interactable = true;
    }

    private void HandleExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#else
        SceneLoader.QuitGame();
#endif
    }

    private void LoadMenuScene()
    {
        SceneLoader.SimpleLoadScene(menuSceneName);
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
            
    }

    private void SetLoading(bool isLoading)
    {
        if (loadingIndicator != null)
        {
            loadingIndicator.SetActive(isLoading);
        }            
    }
}