using System;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    [SerializeField] private Transform loginPanel;
    [SerializeField] private Transform userPanel;

    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_Text playerIDTxt;
    [SerializeField] private TMP_Text playerNameTxt;

    [SerializeField] private TMP_InputField updateNameIF;
    [SerializeField] private Button updateNameBtn;

    [SerializeField] private UnityPlayerAuth unityPlayerAuth;

    private void Start()
    {
        loginPanel.gameObject.SetActive(true);
        userPanel.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        loginButton?.onClick.AddListener(LoginButton);
        unityPlayerAuth.OnSignedIn += UnityPlayerOnSignedIn;

        updateNameBtn.onClick.AddListener(UpdateName);
        unityPlayerAuth.OnUpdateName += UpdateNameVisual;
    }

    private void UpdateNameVisual(string newName)
    {
        playerNameTxt.text = newName;
    }

    private async void UpdateName()
    {
        await unityPlayerAuth.UpdateName(updateNameIF.text);
    }

    private void UnityPlayerOnSignedIn(PlayerInfo playerInfo, string playerName)
    {

        loginPanel.gameObject.SetActive(false);
        userPanel.gameObject.SetActive(true);

        playerIDTxt.text = "ID: " + playerInfo.Id;
        playerNameTxt.text = playerName;
    }

    private void OnDisable()
    {
        loginButton?.onClick.RemoveListener(LoginButton);
        unityPlayerAuth.OnSignedIn -= UnityPlayerOnSignedIn;
    }

    private async void LoginButton()
    {
        await unityPlayerAuth.InitSignIn();
    }
}
